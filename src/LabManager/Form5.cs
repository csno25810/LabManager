using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CsvHelper;
using CsvHelper.Configuration;
using static System.Net.WebRequestMethods;

namespace LabManager
{
    public partial class Form5 : Form
    {
        private Setting mySqlSet;

        // 罰直登録用
        private string studentId;
        private List<DateTime> selectedDates = new List<DateTime>();

        // 画面サイズ調整
        int winHeight = (int)(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height * 0.8);
        int winWidth = (int)(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width * 0.8);

        // 右側DataGridView表示用
        private DataTable DutyViewTable = new DataTable();
        private DataTable GoogleFormViewTable = new DataTable();

        // Googleフォーム（公開CSV）のURL
        private readonly string googleDutyFormCsvUrl =
      "https://docs.google.com/spreadsheets/d/e/2PACX-1vQIM-HnTsbRl8uODFe1hZqGZoHNJCmZ3GjZCf3aImMqILXSQX4gPGAPBw7d_dgTOzJLAwSwdJCq8E4E/pub?output=csv";

        /* 2025年度verGoogleフォーム（公開CSV）のURL
        private readonly string googleDutyFormCsvUrl =
            "https://docs.google.com/spreadsheets/d/e/2PACX-1vRiMBq_kEpVkJC2_L_sGHoaS28OP974fq-25bfELSPwpEGXx5EHdxmpdd0x5yW9-YJ4D8vZ3y72frsT/pub?output=csv";
        */

        // Googleフォーム → 自動割り振り用（学生ごとの可否）
        private Dictionary<string, StudentAvailability> availabilityByStudent =
      new Dictionary<string, StudentAvailability>();

        private string lastLoadedMonth = "";

        //曜日
        private class StudentAvailability
        {
            public string Month;
            public string StudentId;

            // 全曜日を「登校可 / 登校不可」で統一
            public bool Mon;
            public bool Tue;
            public bool Wed;
            public bool Thu;
            public bool Fri;

            public string Note = "";
        }


        /*2025年度用曜日登録
        private class StudentAvailability
        {
            public string Month;
            public string StudentId;

            // 月火木：セルが「登校可」なら true（それ以外は false 扱い）
            public bool Mon;
            public bool Tue;
            public bool Thu;

            // 水金：入力が空欄＝制限なし
            // 入力があれば「来られない日付」の集合
            public HashSet<DateTime> WedUnavailableDates = new HashSet<DateTime>();
            public HashSet<DateTime> FriUnavailableDates = new HashSet<DateTime>();

            // 任意
            public string Note = "";
        }*/

        // コンストラクタ
        public Form5(Setting settings)
        {
            InitializeComponent();
            this.mySqlSet = settings;

            this.Size = new Size(winWidth, winHeight);
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(winWidth / 9, winHeight / 9);

            dataGridView2.Size = new Size(winWidth / 2, winHeight);

            panel1.Size = new Size(winWidth * 4 / 10, winHeight - 200);
            panel1.Location = new Point(0, 0);

            panel2.Size = new Size(winWidth * 4 / 10, winHeight - 200);
            panel2.Location = new Point(0, 0);

            panel5.Size = new Size(winWidth * 4 / 10, winHeight - 200);
            panel5.Location = new Point(0, 0);

            // 初期表示：曜日別(panel2)
            panel1.Visible = false;
            panel2.Visible = true;
            panel5.Visible = false;

            // 月度コンボ初期値
            try
            {
                if (comboBoxMonth != null)
                {
                    comboBoxMonth.SelectedIndex = DateTime.Today.Month - 1; // 0-based
                }
            }
            catch { }
        }

        // Form5 ロード
        private void Form5_Load(object sender, EventArgs e)
        {
            GoogleFormViewTable = new DataTable();
            dataGridView2.DataSource = GoogleFormViewTable;

            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            dataGridView2.RowHeadersVisible = false;
            dataGridView2.AllowUserToAddRows = false;
            dataGridView2.ReadOnly = true;
        }

        // Googleフォーム読込（右のDataGridViewに全行表示）
        private void buttonLoadFromGoogleForm_Click(object sender, EventArgs e)
        {
            try
            {
                // コンボで選ばれた「12月」など
                string selectedMonth = "";
                if (comboBoxMonth != null && comboBoxMonth.SelectedItem != null)
                {
                    selectedMonth = comboBoxMonth.SelectedItem.ToString().Trim();
                }

                DataTable raw = LoadGoogleFormCsvAsDataTable(googleDutyFormCsvUrl);

                // 月度列（部分一致）
                string monthColumnName = FindColumnNameContains(raw, "月度");

                // 月度列が見つからなければ全件表示
                if (string.IsNullOrWhiteSpace(monthColumnName))
                {
                    GoogleFormViewTable = raw;
                    dataGridView2.DataSource = GoogleFormViewTable;
                    MessageBox.Show("GoogleフォームCSVを読み込みました（※月度列が見つからないため全件表示）。\n件数: " + raw.Rows.Count);
                    return;
                }

                // 月度フィルタ
                DataTable filtered = raw.Clone();
                if (string.IsNullOrWhiteSpace(selectedMonth))
                {
                    foreach (DataRow r in raw.Rows) filtered.ImportRow(r);
                }
                else
                {
                    foreach (DataRow r in raw.Rows)
                    {
                        string m = (r[monthColumnName] ?? "").ToString().Trim();
                        if (m == selectedMonth)
                        {
                            filtered.ImportRow(r);
                        }
                    }
                }

                GoogleFormViewTable = filtered;
                dataGridView2.DataSource = GoogleFormViewTable;
                dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;

                MessageBox.Show(
                  "GoogleフォームCSVを読み込みました。\n" +
                  $"月度: {(string.IsNullOrWhiteSpace(selectedMonth) ? "未選択（全件）" : selectedMonth)}\n" +
                  $"表示件数: {GoogleFormViewTable.Rows.Count}"
                );
            }
            catch (WebException ex)
            {
                MessageBox.Show("GoogleフォームCSVの取得に失敗しました。\nURL/公開設定/ネットワークを確認してください。\n\n" + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Googleフォーム読込エラー\n\n" + ex.Message);
            }
        }

        // CSVを読む（Split禁止）
        private DataTable LoadGoogleFormCsvAsDataTable(string csvUrl)
        {
            using (var client = new WebClient())
            using (var stream = client.OpenRead(csvUrl))
            using (var reader = new StreamReader(stream, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false)))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                IgnoreBlankLines = true,

                DetectColumnCountChanges = false,
                MissingFieldFound = null,
                BadDataFound = null,
                HeaderValidated = null,

                PrepareHeaderForMatch = args => (args.Header ?? "").Trim(),
            }))
            {
                DataTable table = new DataTable();

                csv.Read();
                csv.ReadHeader();
                var headers = csv.HeaderRecord;
                if (headers == null || headers.Length == 0)
                {
                    return table;
                }

                foreach (var h in headers) table.Columns.Add(h);

                while (csv.Read())
                {
                    DataRow row = table.NewRow();
                    for (int i = 0; i < headers.Length; i++)
                    {
                        row[i] = csv.GetField(i) ?? "";
                    }
                    table.Rows.Add(row);
                }

                return table;
            }
        }

        private string FindColumnNameContains(DataTable table, string keyword)
        {
            if (table == null || table.Columns == null) return "";
            foreach (DataColumn col in table.Columns)
            {
                if (!string.IsNullOrWhiteSpace(col.ColumnName) && col.ColumnName.Contains(keyword))
                {
                    return col.ColumnName;
                }
            }
            return "";
        }

        private string FindColumnNameContainsAny(DataTable table, params string[] keywords)
        {
            if (table == null || table.Columns == null) return "";
            foreach (DataColumn col in table.Columns)
            {
                string name = col.ColumnName ?? "";
                foreach (var k in keywords)
                {
                    if (!string.IsNullOrWhiteSpace(k) && name.Contains(k))
                    {
                        return name;
                    }
                }
            }
            return "";
        }

        // Googleフォーム → availabilityByStudent を作る
        private bool TryImportAvailabilityFromGoogleForm(string targetMonth)
        {
            try
            {
                ImportAvailabilityFromGoogleForm(targetMonth);
                return true;
            }
            catch (WebException ex)
            {
                MessageBox.Show("GoogleフォームCSVの取得に失敗しました。\nURL/公開設定/ネットワークを確認してください。\n\n" + ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("GoogleフォームCSVの読み込み/変換に失敗しました。\n列名や入力形式を確認してください。\n\n" + ex.Message);
                return false;
            }
        }

        private void ImportAvailabilityFromGoogleForm(string targetMonth)
        {
            availabilityByStudent.Clear();
            lastLoadedMonth = targetMonth;

            DataTable raw = LoadGoogleFormCsvAsDataTable(googleDutyFormCsvUrl);

            string colMonth = FindColumnNameContains(raw, "月度");
            string colStudent = FindColumnNameContainsAny(raw, "学生番号", "学籍");

            string colMon = FindColumnNameContainsAny(raw, "月曜日");
            string colTue = FindColumnNameContainsAny(raw, "火曜日");
            string colWed = FindColumnNameContainsAny(raw, "水曜日");
            string colThu = FindColumnNameContainsAny(raw, "木曜日");
            string colFri = FindColumnNameContainsAny(raw, "金曜日");
            string colNote = FindColumnNameContainsAny(raw, "その他", "連絡", "備考");

            if (string.IsNullOrWhiteSpace(colMonth) || string.IsNullOrWhiteSpace(colStudent))
                throw new Exception("月度または学生番号の列が見つかりません。");

            foreach (DataRow r in raw.Rows)
            {
                string month = (r[colMonth] ?? "").ToString().Trim();
                if (month != targetMonth) continue;

                string sid = Regex.Match((r[colStudent] ?? "").ToString(), @"\d+").Value;
                if (string.IsNullOrWhiteSpace(sid)) continue;

                bool canMon = (r[colMon] ?? "").ToString().Contains("登校可");
                bool canTue = (r[colTue] ?? "").ToString().Contains("登校可");
                bool canWed = (r[colWed] ?? "").ToString().Contains("登校可");
                bool canThu = (r[colThu] ?? "").ToString().Contains("登校可");
                bool canFri = (r[colFri] ?? "").ToString().Contains("登校可");

                availabilityByStudent[sid] = new StudentAvailability
                {
                    Month = month,
                    StudentId = sid,
                    Mon = canMon,
                    Tue = canTue,
                    Wed = canWed,
                    Thu = canThu,
                    Fri = canFri,
                    Note = !string.IsNullOrWhiteSpace(colNote) ? (r[colNote] ?? "").ToString() : ""
                };
            }
        }


        /*2025年度用の日直登録
        private void ImportAvailabilityFromGoogleForm(string targetMonth)
        {
            if (!string.IsNullOrWhiteSpace(lastLoadedMonth) &&
                lastLoadedMonth == targetMonth &&
                availabilityByStudent.Count > 0)
            {
                return;
            }

            availabilityByStudent.Clear();
            lastLoadedMonth = targetMonth;

            DataTable raw = LoadGoogleFormCsvAsDataTable(googleDutyFormCsvUrl);

            // 「部分一致」で拾う
            string colMonth = FindColumnNameContains(raw, "月度"); 
            string colStudent = FindColumnNameContainsAny(raw, "学生番号", "学籍");
            string colMon = FindColumnNameContainsAny(raw, "月曜日");
            string colTue = FindColumnNameContainsAny(raw, "火曜日");
            string colWed = FindColumnNameContainsAny(raw, "水曜日");
            string colThu = FindColumnNameContainsAny(raw, "木曜日");
            string colFri = FindColumnNameContainsAny(raw, "金曜日");
            string colNote = FindColumnNameContainsAny(raw, "その他", "連絡", "備考");

            if (string.IsNullOrWhiteSpace(colMonth) || string.IsNullOrWhiteSpace(colStudent))
            {
                throw new Exception("月度列 または 学生番号列が見つかりません。Googleフォームの列名（質問タイトル）を確認してください。");
            }

            foreach (DataRow r in raw.Rows)
            {
                string month = (r[colMonth] ?? "").ToString().Trim();
                if (!string.IsNullOrWhiteSpace(targetMonth) && month != targetMonth) continue;

                string sidRaw = (r[colStudent] ?? "").ToString().Trim();
                if (string.IsNullOrWhiteSpace(sidRaw)) continue;

                // 学生番号だけ抜く
                string sid = Regex.Match(sidRaw, @"\d+").Value;
                if (string.IsNullOrWhiteSpace(sid)) continue;

                // 月火木は「登校可」ならtrue
                bool canMon = !string.IsNullOrWhiteSpace(colMon) && ((r[colMon] ?? "").ToString().Contains("登校可"));
                bool canTue = !string.IsNullOrWhiteSpace(colTue) && ((r[colTue] ?? "").ToString().Contains("登校可"));
                bool canThu = !string.IsNullOrWhiteSpace(colThu) && ((r[colThu] ?? "").ToString().Contains("登校可"));

                // 水金は「来られない日付」の入力欄（空欄＝制限なし）
                string wedText = !string.IsNullOrWhiteSpace(colWed) ? (r[colWed] ?? "").ToString().Trim() : "";
                string friText = !string.IsNullOrWhiteSpace(colFri) ? (r[colFri] ?? "").ToString().Trim() : "";

                string note = !string.IsNullOrWhiteSpace(colNote) ? (r[colNote] ?? "").ToString().Trim() : "";

                var avail = new StudentAvailability
                {
                    Month = month,
                    StudentId = sid,
                    Mon = canMon,
                    Tue = canTue,
                    Thu = canThu,
                    Note = note
                };

                foreach (var d in ParseDatesLoose(wedText)) avail.WedUnavailableDates.Add(d.Date);
                foreach (var d in ParseDatesLoose(friText)) avail.FriUnavailableDates.Add(d.Date);

                // 同一学生の複数回答は最後で上書き
                availabilityByStudent[sid] = avail;
            }
        }*/

        // "2025/12/25, 12/26, 12月27日" 等を吸収
        private IEnumerable<DateTime> ParseDatesLoose(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) yield break;

            string[] parts = text
              .Replace("、", ",")
              .Replace("　", " ")
              .Split(new char[] { ',', ';', '\n', '\r', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var p0 in parts)
            {
                string p = p0.Trim();
                p = Regex.Replace(p, @"(\d+)月(\d+)日", "$1/$2");

                DateTime dt;

                // 年あり
                if (DateTime.TryParse(p, out dt))
                {
                    yield return dt.Date;
                    continue;
                }

                // 年なし(M/D) → 今年扱い（必要なら startDate.Year に合わせる）
                if (Regex.IsMatch(p, @"^\d{1,2}/\d{1,2}$"))
                {
                    if (DateTime.TryParse(DateTime.Today.Year + "/" + p, out dt))
                    {
                        yield return dt.Date;
                        continue;
                    }
                }
            }
        }

        // 罰直登録（panel1）
        private void monthCalendar1_DateSelected(object sender, DateRangeEventArgs e)
        {
            for (DateTime dt = e.Start; dt <= e.End; dt = dt.AddDays(1))
            {
                if (!selectedDates.Contains(dt))
                {
                    selectedDates.Add(dt);
                }
            }
            textBox2.Text = string.Join(Environment.NewLine, selectedDates.Select(d => d.ToString("yyyy-MM-dd")));
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            studentId = textBox1.Text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("学籍番号を入力してから登録してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            foreach (var date in selectedDates)
            {
                InsertDutySchedulePunishment(studentId, date);
            }

            MessageBox.Show("罰直が登録されました。");
            selectedDates.Clear();
            textBox1.Clear();
            textBox2.Clear();
        }

        private void InsertDutySchedule(string studentId, DateTime dutyDate)
        {
            string commandText = $@"INSERT INTO duty_schedule (student_id, duty_date) VALUES ('{studentId}', '{dutyDate:yyyy-MM-dd}')";
            Connector.ExecuteCommand(commandText);
        }

        private void InsertDutySchedulePunishment(string studentId, DateTime dutyDate)
        {
            string commandText = $@"INSERT INTO duty_schedule (student_id, duty_date, duty_type) VALUES ('{studentId}', '{dutyDate:yyyy-MM-dd}', 1)";
            Connector.ExecuteCommand(commandText);
        }

        private void button2_Click(object sender, EventArgs e) { }

        private void button3_Click(object sender, EventArgs e)
        {
            selectedDates.Clear();
            textBox1.Clear();
            textBox2.Clear();
        }

        // 画面切り替え
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                panel2.Visible = true;
                panel5.Visible = false;
                panel1.Visible = false;
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                panel2.Visible = false;
                panel5.Visible = true;
                panel1.Visible = false;
            }
        }

        private void radioButton11_CheckedChanged(object sender, EventArgs e)
        {
            panel2.Visible = false;
            panel5.Visible = false;
            panel1.Visible = true;
        }

        // 右側DataGridView（日直予定表示）
        private void SetupCalendarView()
        {
            DutyViewTable = new DataTable();
            dataGridView2.Font = new Font("メイリオ", 12);

            DateTime Today = DateTime.Today;
            int addMonth = 12;
            string startDate = Today.ToString("yyyy-MM-dd");
            string endDate = Today.AddMonths(addMonth).ToString("yyyy-MM-dd");

            string dutyQuery = $@"
    SELECT
        DATE_FORMAT(duty_date, '%W') AS day_of_week,
        duty_date,
        student_id,
        duty_type
    FROM duty_schedule
    WHERE duty_date BETWEEN '{startDate}' AND '{endDate}'
    ORDER BY duty_date;
";

            Connector.TableReader(dutyQuery, DutyViewTable);
            dataGridView2.DataSource = DutyViewTable;

            dataGridView2.Columns[0].HeaderText = "曜日";
            dataGridView2.Columns[1].HeaderText = "日直の日付";
            dataGridView2.Columns[2].HeaderText = "学生番号";
            dataGridView2.Columns[3].HeaderText = "日直 OR 罰直";

            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dataGridView2.CellFormatting -= dataGridView2_CellFormatting;
            dataGridView2.CellFormatting += new DataGridViewCellFormattingEventHandler(dataGridView2_CellFormatting);
        }

        private void dataGridView2_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridView2.DataSource == null) return;

            if (DutyViewTable != null && dataGridView2.DataSource == DutyViewTable)
            {
                if (e.ColumnIndex == 0 && e.Value != null)
                {
                    switch (e.Value.ToString())
                    {
                        case "Monday": e.CellStyle.BackColor = Color.LightGray; break;
                        case "Tuesday": e.CellStyle.BackColor = Color.LightBlue; break;
                        case "Wednesday": e.CellStyle.BackColor = Color.LightGreen; break;
                        case "Thursday": e.CellStyle.BackColor = Color.LightYellow; break;
                        case "Friday": e.CellStyle.BackColor = Color.Orange; break;
                        case "Saturday": e.CellStyle.BackColor = Color.LightBlue; break;
                        case "Sunday": e.CellStyle.BackColor = Color.LightCoral; break;
                    }
                }

                if (e.ColumnIndex == 3 && e.Value != null)
                {
                    switch (e.Value.ToString())
                    {
                        case "0": e.Value = "日直"; break;
                        case "1": e.Value = "罰直"; break;
                    }
                }
            }
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e) { }

        private void button6_Click(object sender, EventArgs e)
        {
            SetupCalendarView();
        }

        // panel2：曜日別登録
        private void button7_Click(object sender, EventArgs e)
        {
            MessageBox.Show("曜日別登録へのGoogleフォーム反映は次段階で統合できます。\n（今は自動割り振り(panel5)側で可否を使う構成です）");
        }

        // panel5：自動振り分け（Googleフォーム可否を考慮）
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            string studentNumber = textBoxStudentNumber.Text.Trim();

            if (string.IsNullOrEmpty(studentNumber))
            {
                MessageBox.Show("学生番号を入力してください。");
                return;
            }

            if (listBoxStudentNumbers.Items.Contains(studentNumber))
            {
                MessageBox.Show("この学生番号は既に追加されています。");
                return;
            }

            listBoxStudentNumbers.Items.Add(studentNumber);
            textBoxStudentNumber.Clear();
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            if (listBoxStudentNumbers.SelectedItem != null)
            {
                listBoxStudentNumbers.Items.Remove(listBoxStudentNumbers.SelectedItem);
            }
            else
            {
                MessageBox.Show("削除する学生番号を選択してください。");
            }
        }

        private void buttonRegister_Click(object sender, EventArgs e)
        {
            DateTime startDate = dateTimePicker3.Value.Date;
            DateTime endDate = dateTimePicker4.Value.Date;

            // 対象月
            string targetMonth = startDate.Month + "月";

            // Googleフォームの可否を取得
            if (!TryImportAvailabilityFromGoogleForm(targetMonth))
            {
                return;
            }

            List<DayOfWeek> selectedDays = new List<DayOfWeek>();
            if (checkBox1.Checked) selectedDays.Add(DayOfWeek.Monday);
            if (checkBox2.Checked) selectedDays.Add(DayOfWeek.Tuesday);
            if (checkBox3.Checked) selectedDays.Add(DayOfWeek.Wednesday);
            if (checkBox4.Checked) selectedDays.Add(DayOfWeek.Thursday);
            if (checkBox5.Checked) selectedDays.Add(DayOfWeek.Friday);
            if (checkBox6.Checked) selectedDays.Add(DayOfWeek.Saturday);

            if (selectedDays.Count == 0)
            {
                MessageBox.Show("少なくとも1つの曜日を選択してください。");
                return;
            }

            if (listBoxStudentNumbers.Items.Count == 0)
            {
                MessageBox.Show("登録する学生番号がありません。");
                return;
            }

            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("一日あたり配置する人数を選択してください。");
                return;
            }

            int dailyCount = int.Parse(comboBox1.SelectedItem.ToString());
            List<string> studentIds = listBoxStudentNumbers.Items.Cast<string>().ToList();

            int studentIndex = 0;

            // 対象日生成
            List<DateTime> validDates = new List<DateTime>();
            for (DateTime d = startDate; d <= endDate; d = d.AddDays(1))
            {
                if (selectedDays.Contains(d.DayOfWeek))
                {
                    validDates.Add(d);
                }
            }


            foreach (DateTime date in validDates)
            {
                int assignedToday = 0;
                int tried = 0;

                while (assignedToday < dailyCount && tried < studentIds.Count * 2)
                {
                    if (studentIndex >= studentIds.Count)
                        studentIndex = 0;

                    string sid = studentIds[studentIndex];
                    studentIndex++;
                    tried++;

                    // Googleフォーム未回答はスキップ
                    if (!availabilityByStudent.TryGetValue(sid, out var avail))
                        continue;

                    bool canAssign = false;

                    switch (date.DayOfWeek)
                    {
                        case DayOfWeek.Monday:
                            canAssign = avail.Mon;
                            break;

                        case DayOfWeek.Tuesday:
                            canAssign = avail.Tue;
                            break;

                        case DayOfWeek.Wednesday:
                            canAssign = avail.Wed;
                            break;

                        case DayOfWeek.Thursday:
                            canAssign = avail.Thu;
                            break;

                        case DayOfWeek.Friday:
                            canAssign = avail.Fri;
                            break;
                    }

                    if (!canAssign)
                        continue;

                    InsertDutySchedule(sid, date);
                    assignedToday++;
                }
            }


            /*2025年度用フォーム登録内容割り振り
            foreach (DateTime date in validDates)
            {
                int assignedToday = 0;
                // その日の割当は「学生リストを循環」しつつ、条件に合う人だけ採用
                // 無限ループ防止にガード
                int tried = 0;

                while (assignedToday < dailyCount && tried < studentIds.Count * 2)
                {
                    if (studentIndex >= studentIds.Count) studentIndex = 0;

                    string sid = studentIds[studentIndex];
                    studentIndex++;
                    tried++;

                    // Googleフォーム未回答はスキップ
                    if (!availabilityByStudent.TryGetValue(sid, out var avail))
                    {
                        continue;
                    }

                    bool canAssign;

                    switch (date.DayOfWeek)
                    {
                        case DayOfWeek.Monday:
                            canAssign = avail.Mon;
                            break;

                        case DayOfWeek.Tuesday:
                            canAssign = avail.Tue;
                            break;

                        case DayOfWeek.Wednesday:
                            canAssign = !avail.WedUnavailableDates.Contains(date.Date);
                            break;

                        case DayOfWeek.Thursday:
                            canAssign = avail.Thu;
                            break;

                        case DayOfWeek.Friday:
                            canAssign = !avail.FriUnavailableDates.Contains(date.Date);
                            break;

                        default:
                            canAssign = false;
                            break;
                    }

                    if (!canAssign) continue;

                    InsertDutySchedule(sid, date);
                    assignedToday++;
                }
            }*/

            MessageBox.Show("Googleフォームの可否を考慮して自動割り振りを行いました。");
            listBoxStudentNumbers.Items.Clear();
        }

    }
}
