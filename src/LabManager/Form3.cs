using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace LabManager
{
    public partial class Form3 : Form
    {
        int winHeight = (int)(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height * 0.8);
        int winWidth = (int)(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width * 0.8);

        int ParticlePos = 240;
        int margin = 12;
        int windowHeader = 50;

        DataTable CustomTable = new DataTable();

        Setting ConnectionData = null;

        public Form3(Setting mySqlSet)
        {
            ConnectionData = mySqlSet;

            InitializeComponent();

            comboBoxPeriodReport.SelectedIndex = 0;
            textBoxBegin.Text = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).ToString("yyyy-MM-dd");
            textBoxEnd.Text = DateTime.Today.ToString("yyyy-MM-dd");

            this.Size = new System.Drawing.Size(winWidth, winHeight);
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(winWidth / 8, winHeight / 8);

            dataGridView1.Location = new Point(ParticlePos, margin);
            dataGridView1.Size = new Size(winWidth - margin - margin - ParticlePos, winHeight - windowHeader);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!EnsureConnected())
                return;

            if (radioButton1.Checked)
            {
                LoadDailySummary();
            }
            else if (radioButton2.Checked)
            {
                LoadStudentTouches();
            }
            else if (radioButton3.Checked)
            {
                LoadCustomSql();
            }
            else if (radioButton4.Checked)
            {
                LoadPeriodReport();
            }
        }

        private bool EnsureConnected()
        {
            if (Connector.IsConnected)
                return true;

            return Connector.Connect(
                ConnectionData.UserID,
                ConnectionData.PassWd,
                ConnectionData.DataBaseName,
                ConnectionData.ServerIP);
        }

        private void LoadDailySummary()
        {
            CustomTable = new DataTable();
            string selectDay = monthCalendar1.SelectionStart.ToString("yyyy-MM-dd");
            Connector.TableReader(
                "SELECT personal_info.student_id,name,MIN(date_format(time_stamp,'%H:%i')), MAX(date_format(time_stamp,'%H:%i')),COUNT(name) " +
                "FROM((touch_log INNER JOIN chip_list ON touch_log.chip_id = chip_list.chip_id) " +
                "INNER JOIN personal_info ON chip_list.student_id = personal_info.student_id) " +
                "WHERE touch_log.time_stamp LIKE \"" + selectDay + "%\" GROUP BY name",
                CustomTable);

            RenameColumns(CustomTable, "学籍番号", "氏名", "初回タッチ時刻", "最終タッチ時刻", "タッチ回数");
            dataGridView1.DataSource = CustomTable;
        }

        private void LoadStudentTouches()
        {
            CustomTable = new DataTable();
            string query = "SELECT personal_info.student_id,name,time_stamp " +
                "FROM((touch_log INNER JOIN chip_list ON touch_log.chip_id = chip_list.chip_id) " +
                "INNER JOIN personal_info ON chip_list.student_id = personal_info.student_id) " +
                "WHERE personal_info.student_id = \"" + textBox1.Text + "\"";

            Connector.TableReader(query, CustomTable);
            RenameColumns(CustomTable, "学籍番号", "氏名", "タッチ時刻");
            dataGridView1.DataSource = CustomTable;
        }

        private void LoadCustomSql()
        {
            DialogResult result = MessageBox.Show(
                "この操作を行う場合SQL文には細心の注意を払ってください。\nまた、読み取り文以外の実行は行わないでください。\n上記を踏まえ実行する場合はYesをしない場合はNoをクリックしてください。",
                "警告",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button2);

            if (result != DialogResult.Yes)
                return;

            CustomTable = new DataTable();
            Connector.TableReader(textBox2.Text, CustomTable);
            dataGridView1.DataSource = CustomTable;
        }

        private void LoadPeriodReport()
        {
            if (!TryParseDateRange(textBoxBegin.Text, textBoxEnd.Text, out string begin, out string end))
                return;

            CustomTable = new DataTable();
            string sql;

            switch (comboBoxPeriodReport.SelectedItem?.ToString())
            {
                case "出勤時刻":
                    sql = BuildFirstTouchQuery(begin, end);
                    break;
                case "全タッチ出力":
                    sql = BuildAllTouchesQuery(begin, end);
                    break;
                default:
                    sql = BuildAttendanceCountQuery(begin, end);
                    break;
            }

            if (!Connector.TableReader(sql, CustomTable))
                return;

            ApplyPeriodReportColumnNames();
            dataGridView1.DataSource = CustomTable;
        }

        private static string BuildAttendanceCountQuery(string begin, string end)
        {
            return $@"
                SELECT sub.id AS id, sub.name AS name, COUNT(*) AS count FROM (
                    SELECT personal_info.student_id AS id, personal_info.name AS name
                    FROM personal_info
                    INNER JOIN chip_list ON chip_list.student_id = personal_info.student_id
                    INNER JOIN touch_log ON touch_log.chip_id = chip_list.chip_id
                    WHERE touch_log.time_stamp >= '{begin}'
                      AND touch_log.time_stamp < DATE_ADD('{end}', INTERVAL 1 DAY)
                    GROUP BY personal_info.student_id, DATE_FORMAT(touch_log.time_stamp, '%Y%m%d')
                ) AS sub
                GROUP BY id
                ORDER BY count DESC";
        }

        private static string BuildFirstTouchQuery(string begin, string end)
        {
            return $@"
                SELECT
                    DATE_FORMAT(MIN(touch_log.time_stamp), '%Y-%m-%d') AS date,
                    DATE_FORMAT(MIN(touch_log.time_stamp), '%H:%i') AS time,
                    personal_info.student_id AS id,
                    personal_info.name AS name
                FROM personal_info
                INNER JOIN chip_list ON chip_list.student_id = personal_info.student_id
                INNER JOIN touch_log ON touch_log.chip_id = chip_list.chip_id
                WHERE touch_log.time_stamp >= '{begin}'
                  AND touch_log.time_stamp < DATE_ADD('{end}', INTERVAL 1 DAY)
                GROUP BY personal_info.student_id, DATE_FORMAT(touch_log.time_stamp, '%Y%m%d')
                ORDER BY date, time";
        }

        private static string BuildAllTouchesQuery(string begin, string end)
        {
            return $@"
                SELECT
                    DATE_FORMAT(touch_log.time_stamp, '%Y-%m-%d') AS date,
                    DATE_FORMAT(touch_log.time_stamp, '%H:%i') AS time,
                    personal_info.student_id AS id,
                    personal_info.name AS name
                FROM personal_info
                INNER JOIN chip_list ON chip_list.student_id = personal_info.student_id
                INNER JOIN touch_log ON touch_log.chip_id = chip_list.chip_id
                WHERE touch_log.time_stamp >= '{begin}'
                  AND touch_log.time_stamp < DATE_ADD('{end}', INTERVAL 1 DAY)
                ORDER BY date, time";
        }

        private void ApplyPeriodReportColumnNames()
        {
            switch (comboBoxPeriodReport.SelectedItem?.ToString())
            {
                case "出勤時刻":
                case "全タッチ出力":
                    RenameColumns(CustomTable, "日付", "時刻", "学籍番号", "氏名");
                    break;
                default:
                    RenameColumns(CustomTable, "学籍番号", "氏名", "出勤回数");
                    break;
            }
        }

        private static bool TryParseDateRange(string beginText, string endText, out string begin, out string end)
        {
            begin = null;
            end = null;

            if (!DateTime.TryParseExact(beginText.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime beginDate))
            {
                MessageBox.Show("開始日は yyyy-MM-dd 形式で入力してください。", "入力エラー");
                return false;
            }

            if (!DateTime.TryParseExact(endText.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime endDate))
            {
                MessageBox.Show("終了日は yyyy-MM-dd 形式で入力してください。", "入力エラー");
                return false;
            }

            if (beginDate > endDate)
            {
                MessageBox.Show("開始日は終了日以前にしてください。", "入力エラー");
                return false;
            }

            begin = beginDate.ToString("yyyy-MM-dd");
            end = endDate.ToString("yyyy-MM-dd");
            return true;
        }

        private static void RenameColumns(DataTable table, params string[] names)
        {
            for (int i = 0; i < names.Length && i < table.Columns.Count; i++)
                table.Columns[i].ColumnName = names[i];
        }

        private void buttonExportCsv_Click(object sender, EventArgs e)
        {
            if (CustomTable == null || CustomTable.Rows.Count == 0)
            {
                MessageBox.Show("先に「表示」でデータを取得してください。", "CSV保存");
                return;
            }

            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "CSV ファイル (*.csv)|*.csv";
                dialog.FileName = comboBoxPeriodReport.SelectedItem + "_結果.csv";

                if (dialog.ShowDialog() != DialogResult.OK)
                    return;

                ExportToCsv(CustomTable, dialog.FileName);
                MessageBox.Show("CSV を保存しました。\n" + dialog.FileName, "CSV保存");
            }
        }

        private static void ExportToCsv(DataTable table, string path)
        {
            var sb = new StringBuilder();

            for (int c = 0; c < table.Columns.Count; c++)
            {
                if (c > 0) sb.Append(',');
                sb.Append(EscapeCsv(table.Columns[c].ColumnName));
            }
            sb.AppendLine();

            foreach (DataRow row in table.Rows)
            {
                for (int c = 0; c < table.Columns.Count; c++)
                {
                    if (c > 0) sb.Append(',');
                    sb.Append(EscapeCsv(row[c]?.ToString() ?? ""));
                }
                sb.AppendLine();
            }

            File.WriteAllText(path, sb.ToString(), new UTF8Encoding(true));
        }

        private static string EscapeCsv(string value)
        {
            if (value.Contains("\"") || value.Contains(",") || value.Contains("\n"))
                return "\"" + value.Replace("\"", "\"\"") + "\"";
            return value;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
