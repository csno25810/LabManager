using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace LabManager
{
    public partial class Form1 : Form
    {
        int winHeight = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height - 50;
        int winWidth = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width / 2;
        //int winWidth = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
        // int AttendanceStateWidth = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width / 2;
        // int AttendanceStateHeight = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height/2;
        int btn1Size = 30;
        int ColumnHeadHeight = 40;
        int ClockMargin = 50;
        int SummaryBarHeight = 40;
        int DutyHeaderHeight = 28;
        static readonly TimeSpan DutyDeadline = new TimeSpan(8, 50, 0);

        private Label lblOccupancy;
        private Label lblDutyHeader;

        // 設定ファイルをリードする
        public Setting mySqlSet = new Setting();

        string OffSeat = "不在";
        string OnSeat = "在席";
        private System.Timers.Timer dailyTimer;
        private DateTime? lastPenaltyRunDate;
        private Form14 tvLeftPanel;

        public Form1()
        {
            // コンポーネント生成
            InitializeComponent();
            dataGridView1.ColumnHeaderMouseClick += new DataGridViewCellMouseEventHandler(this.dataGridView1_ColumnHeaderMouseClick);


            if (mySqlSet.ReadSetting())
                mySqlSet.CreateFile();

            // フォームのサイズを変更
            this.Size = new System.Drawing.Size(winWidth, winHeight);
            this.StartPosition = FormStartPosition.Manual;
            //this.Location = new Point(winWidth, 0);
            this.Location = new Point(winWidth, 0);

            // 在室人数サマリー
            lblOccupancy = new Label
            {
                Location = new Point(0, 0),
                Size = new Size(winWidth, SummaryBarHeight),
                Font = UiFonts.Get(14F, FontStyle.Bold),
                ForeColor = Color.Black,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "在室 -- 人　|　本日来室 -- 人"
            };

            lblDutyHeader = new Label
            {
                Font = UiFonts.Get(11F, FontStyle.Bold),
                ForeColor = Color.Black,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                TextAlign = ContentAlignment.MiddleLeft,
                Text = "  本日の日直"
            };

            // LabelTEST
            maskedTextBox1.Font = UiFonts.Get(24F);
            maskedTextBox1.Location = new Point(1, winHeight - btn1Size - ClockMargin + 1);
            maskedTextBox1.Size = new Size(winWidth, ClockMargin - 1);
            maskedTextBox1.Text = DateTime.Now.ToString("yyyy/MM/dd(ddd) HH:mm");

            int grid1Height = winHeight * 2 / 3 - SummaryBarHeight - DutyHeaderHeight;
            int grid2Top = winHeight * 2 / 3;
            int grid2Height = winHeight * 1 / 3 - btn1Size - ClockMargin;

            lblDutyHeader.Location = new Point(0, SummaryBarHeight + grid1Height);
            lblDutyHeader.Size = new Size(winWidth, DutyHeaderHeight);

            // データ表示部分の初期設定
            dataGridView1.Location = new Point(0, SummaryBarHeight);
            dataGridView1.Size = new Size(winWidth, grid1Height);
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.ColumnHeadersVisible = true;
            dataGridView1.ColumnHeadersHeight = ColumnHeadHeight;
            dataGridView1.RowTemplate.Height = (winHeight - btn1Size) / 30;
            dataGridView1.Font = UiFonts.Get(15F);
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = UiFonts.Get(10F);

            dataGridView2.Location = new Point(0, grid2Top);
            dataGridView2.Size = new Size(winWidth, grid2Height);
            dataGridView2.RowHeadersVisible = false;
            dataGridView2.ColumnHeadersVisible = true;
            dataGridView2.ColumnHeadersHeight = ColumnHeadHeight;
            dataGridView2.RowTemplate.Height = (winHeight - btn1Size) / 25;
            dataGridView2.Font = UiFonts.Get(10F);
            dataGridView2.ColumnHeadersDefaultCellStyle.Font = UiFonts.Get(10F);



            // ボタン設定
            button1.Size = new Size(winWidth / 2 - 80, btn1Size);
            button1.Location = new Point(0, winHeight - btn1Size);
            button1.Text = "データ獲得 / 連続獲得開始";

            button2.Size = new Size(winWidth / 2 - 100 - 80, btn1Size);
            button2.Location = new Point(winWidth / 2 + 2 + 80, winHeight - btn1Size);
            button2.Text = "設定";

            button3.Size = new Size(100 - 2, btn1Size);
            button3.Location = new Point(winWidth - 100 + 2, winHeight - btn1Size);
            button3.Text = "終了";

            button4.Size = new Size(160 + 2, btn1Size);
            button4.Location = new Point(winWidth / 2 - 80, winHeight - btn1Size);
            button4.Text = "MENU";

            //Timer1設定
            timer1.Enabled = false; // 接続できたタイミングで有効化する
            timer1.Interval = mySqlSet.ReloadTime * 1000;

            timer2.Enabled = true;
            timer2.Interval = 10 * 1000;

            // Timer3の設定
            timer3.Enabled = true;
            timer3.Interval = 1000; // 1秒ごとにTickイベントを発生させる
            timer3.Start();
            GetConnection();

            ReadAllStatments();

            Controls.Add(lblOccupancy);
            Controls.Add(lblDutyHeader);
            lblOccupancy.BringToFront();
            lblDutyHeader.BringToFront();

            Shown += Form1_Shown;
        }

        private void UpdateOccupancySummary(int presentCount, int todayCount)
        {
            lblOccupancy.Text = $"在室 {presentCount} 人　|　本日来室 {todayCount} 人";
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            if (!Program.TvMode || tvLeftPanel != null)
                return;

            tvLeftPanel = new Form14(mySqlSet);
            tvLeftPanel.ConfigureForTvDisplay();
            tvLeftPanel.Show(this);
        }

        // 接続状態をタイトルバーとタイマーに反映する
        private void UpdateConnectionStatus()
        {
            if (Connector.IsConnected)
            {
                this.Text = "LabManager";
                timer1.Enabled = true;
            }
            else
            {
                this.Text = "LabManager  [未接続]";
                timer1.Enabled = false;
            }
        }



        private void GetConnection()
        {
            // SQLサーバに接続.
            bool conectResult = Connector.Connect(mySqlSet.UserID, mySqlSet.PassWd, mySqlSet.DataBaseName, mySqlSet.ServerIP);
            if (conectResult)
            {
                UpdateConnectionStatus();
                return;
            }

            // つながらなかったら設定画面を表示する
            using (Form2 configWindow = new Form2(mySqlSet))
            {
                configWindow.ShowDialog();
            }

            // 設定が更新された可能性があるので、もう一度だけ接続を試みる。
            // ここでも失敗した場合は未接続モードのまま続行する（アプリは終了しない）。
            Connector.Connect(mySqlSet.UserID, mySqlSet.PassWd, mySqlSet.DataBaseName, mySqlSet.ServerIP);
            UpdateConnectionStatus();
        }
        //データの更新(データ連続取得)
        private void button1_Click(object sender, EventArgs e)
        {
            GetConnection();
            ReadAllStatments();
        }

        private void dataGridView1_ColumnHeaderMouseClick(object sender, EventArgs e)
        {
            CellColorChange();
        }

        //データ連続取得してdatagridviewに表示
        private void ReadAllStatments()
        {
            // 未接続のときは DB アクセスをスキップする（自宅PCでの UI 確認用）
            if (!Connector.IsConnected)
            {
                lblOccupancy.Text = "在室 -- 人　|　本日来室 -- 人　（未接続）";
                return;
            }

            timer1.Enabled = false;
            DataTable dataSql = new DataTable();
            DataTable dataSqlCount = new DataTable();
            DateTime dt = DateTime.Now;

            string today = dt.ToString("yyyy-MM-dd");
            Connector.TableReader("SELECT personal_info.student_id,name,MIN(date_format(time_stamp,'%H:%i')), MAX(date_format(time_stamp,'%H:%i')),COUNT(name) FROM((touch_log INNER JOIN chip_list ON touch_log.chip_id = chip_list.chip_id)INNER JOIN personal_info ON chip_list.student_id = personal_info.student_id) WHERE touch_log.time_stamp LIKE \"" + today + "%\"GROUP BY name", dataSql);




            if (dataSql.Rows.Count == 0)
            {
                dataGridView1.DataSource = null;
                UpdateOccupancySummary(0, 0);
                goto LoadDutySchedule;
            }






            object[] Result = dataSql.Rows[0].ItemArray;

            // カラム名の変更
            dataSql.Columns[3].ColumnName = "AAA";
            dataSql.Columns[2].ColumnName = "BBB";

            // 描画領域の設定
            int attendanceAreaHeight = winHeight - btn1Size - ColumnHeadHeight - ClockMargin - SummaryBarHeight - DutyHeaderHeight;
            if (dataSql.Rows.Count < 15)
            {
                dataGridView1.RowTemplate.Height = attendanceAreaHeight / 15;
            }
            else
            {
                dataGridView1.RowTemplate.Height = attendanceAreaHeight / dataSql.Rows.Count;
            }

            DataTable newView = dataSql;
            newView.Columns.Add("State", typeof(string));

            int presentCount = 0;
            for (int i = 0; i < dataSql.Rows.Count; i++)
            {
                if (int.Parse(dataSql.Rows[i]["COUNT(name)"].ToString()) % 2 == 0)
                {
                    newView.Rows[i]["State"] = OffSeat;
                }
                else
                {
                    newView.Rows[i]["State"] = OnSeat;
                    presentCount++;
                }
            }
            newView.Columns.Remove("COUNT(name)");
            UpdateOccupancySummary(presentCount, dataSql.Rows.Count);

            // 内容をバインドし表示する。
            dataGridView1.DataSource = newView;

            dataGridView1.Columns[0].HeaderText = "学籍番号";
            dataGridView1.Columns[1].HeaderText = "氏名";
            dataGridView1.Columns[2].HeaderText = "初回タッチ時刻";
            dataGridView1.Columns[3].HeaderText = "最終タッチ時刻";
            dataGridView1.Columns[4].HeaderText = "状態";

            dataGridView1.Columns[0].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[1].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[2].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[3].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[3].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[0].Width = 130;
            dataGridView1.Columns[1].Width = winWidth - (130 + 150 + 150 + 100);
            dataGridView1.Columns[2].Width = 150;
            dataGridView1.Columns[3].Width = 150;
            dataGridView1.Columns[4].Width = 100;

            if (dataGridView1.CurrentCell != null)
                dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Selected = false;

            //DataGridView1にバインドされているDataTableを取得
            DataTable sorttable = (DataTable)dataGridView1.DataSource;
            DataView dv = sorttable.DefaultView;
            dv.Sort = "State DESC, AAA ASC";
            dataGridView1.Columns[3].HeaderCell.SortGlyphDirection = SortOrder.Descending;
            dataGridView1.Columns[4].HeaderCell.SortGlyphDirection = SortOrder.Descending;

            if (dataGridView1.CurrentCell != null)
                dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Selected = false;

            // 色換え
            CellColorChange();

            LoadDutySchedule:

            DataTable fetchedData = FetchData();
            CheckAndUpdateDutyStatus(fetchedData);

            string query = $@"
            SELECT 
                ds.student_id, 
                pi.name, 
                COALESCE(MIN(DATE_FORMAT(tl.time_stamp, '%H:%i')), '-') AS 'attendance_time',
                ds.duty_status,
                ds.duty_type,
                pi.penalty_count
            FROM 
                duty_schedule ds
            INNER JOIN 
                personal_info pi ON ds.student_id = pi.student_id
            LEFT JOIN 
                chip_list cl ON ds.student_id = cl.student_id
            LEFT JOIN 
                touch_log tl ON cl.chip_id = tl.chip_id AND DATE(tl.time_stamp) = '{today}'
            WHERE 
                ds.duty_date = '{today}'
            GROUP BY 
                ds.student_id, pi.name, ds.duty_status, ds.duty_type, pi.penalty_count
            ";

            // duty_schedule テーブルからのデータ取得
            DataTable dataDutySchedule = new DataTable();
            //Connector.TableReader($"SELECT student_id, duty_date, duty_status, penalty_count FROM duty_schedule WHERE duty_date = '{NowDay}'", dataDutySchedule);
            //Connector.TableReader($"SELECT personal_info.student_id, name, MIN(DATE_FORMAT(touch_log.time_stamp, '%H:%i')) AS '出席時刻', duty_schedule.duty_status, duty_schedule.penalty_count FROM duty_schedule INNER JOIN chip_list ON duty_schedule.student_id = chip_list.student_id INNER JOIN personal_info ON chip_list.student_id = personal_info.student_id LEFT JOIN touch_log ON chip_list.chip_id = touch_log.chip_id WHERE duty_schedule.duty_date = '" + NowDay + "' GROUP BY name", dataDutySchedule);
            Connector.TableReader(query, dataDutySchedule);

            SetupDataGridView2(dataDutySchedule);
            UpdateDutyHeader(dataDutySchedule);
            timer1.Enabled = true;
        }

        private void UpdateDutyHeader(DataTable dutyData)
        {
            lblDutyHeader.Text = dutyData.Rows.Count == 0
                ? "  本日の日直（担当なし）"
                : $"  本日の日直（{dutyData.Rows.Count}名）";
        }




        private void SetupDataGridView2(DataTable data)
        {
            dataGridView2.DataSource = data;
            dataGridView2.AutoGenerateColumns = true;

            if (dataGridView2.Columns.Count < 6)
                return;

            dataGridView2.Columns["duty_type"].Visible = false;

            dataGridView2.Columns[0].HeaderText = "学籍番号";
            dataGridView2.Columns[1].HeaderText = "日直氏名";
            dataGridView2.Columns["attendance_time"].HeaderText = "出席時刻";
            dataGridView2.Columns["duty_status"].HeaderText = "出席状況";
            dataGridView2.Columns["penalty_count"].HeaderText = "罰直回数";

            dataGridView2.Columns[0].Width = 130;
            dataGridView2.Columns[1].Width = winWidth - (130 + 150 + 150 + 100);
            dataGridView2.Columns["attendance_time"].Width = 150;
            dataGridView2.Columns["duty_status"].Width = 150;
            dataGridView2.Columns["penalty_count"].Width = 100;

            dataGridView2.DefaultCellStyle.Font = UiFonts.Get(11F);
            dataGridView2.ColumnHeadersDefaultCellStyle.Font = UiFonts.Get(10F, FontStyle.Bold);
        }




        private void CellColorChange()
        {
            if (dataGridView1.RowCount == 0)
                return;

            if (dataGridView1.CurrentCell != null)
                dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Selected = false;

            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                var row = dataGridView1.Rows[i];
                bool isPresent = dataGridView1[4, i].Value?.ToString() == OnSeat;
                row.DefaultCellStyle.BackColor = Color.White;
                row.DefaultCellStyle.ForeColor = Color.Black;
                row.DefaultCellStyle.Font = UiFonts.Get(15F, isPresent ? FontStyle.Bold : FontStyle.Regular);
            }
            dataGridView1.Refresh();
        }





        private string EnrollmentStudents(string id)
        {
            string Lists = "";

            string[] Member = { "7011", "7111", "7041", "7026", "M9013" };

            for (int i = 0; i < Member.Length; i++)
            {
                if (i != Member.Length - 1)
                {
                    Lists += id + " =\"" + Member[i].ToString() + "\" OR ";
                }
                else
                {
                    Lists += id + " =\"" + Member[i].ToString() + "\" ";
                }
            }
            return Lists;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            ReadAllStatments();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (Form2 configWindow = new Form2(mySqlSet))
            {
                configWindow.ShowDialog();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            maskedTextBox1.Text = DateTime.Now.ToString("yyyy/MM/dd(ddd) HH:mm");
            maskedTextBox1.Refresh();

            // 8:50 前の「待機中（あとX分）」表示を更新
            if (DateTime.Now.TimeOfDay < DutyDeadline && dataGridView2.RowCount > 0)
                dataGridView2.Refresh();
        }


        private void button4_Click(object sender, EventArgs e)
        {
            Form4 menuForm = new Form4(mySqlSet);
            menuForm.ShowDialog();
        }

        //SQLクエリを作成データ取得
        private DataTable FetchData()
        {
            DateTime dutyTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 9, 0, 0);
            string today = DateTime.Now.ToString("yyyy-MM-dd");
            string query = $@"
        SELECT 
            ds.student_id,
            pi.name,
            COALESCE(MIN(tl.time_stamp), NULL) AS 'actual_time_stamp',
            ds.duty_status,
            ds.duty_type,
            pi.penalty_count
        FROM
            duty_schedule ds
        JOIN
            personal_info pi ON ds.student_id = pi.student_id
        JOIN
            chip_list cl ON ds.student_id = cl.student_id
        LEFT JOIN
            touch_log tl ON cl.chip_id = tl.chip_id AND DATE(tl.time_stamp) = '{today}'
        WHERE
            ds.duty_date = '{today}'
        GROUP BY
            ds.student_id;
    ";

            DataTable results = new DataTable();
            Connector.TableReader(query, results);
            return results;
        }
        //日直の出席状況
        private void CheckAndUpdateDutyStatus(DataTable results)
        {
            DateTime dutyTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 8, 50, 0);
            string today = DateTime.Now.ToString("yyyy-MM-dd");

            foreach (DataRow row in results.Rows)
            {
                string studentId = row["student_id"].ToString();

                DateTime? actualTime = row.IsNull("actual_time_stamp") ? (DateTime?)null : DateTime.Parse(row["actual_time_stamp"].ToString());
                if (actualTime == null)
                {
                    continue;  // 出勤時刻が記録されていない場合はスキップ
                }

                TimeSpan delay = actualTime.Value - dutyTime;
                int newStatus = 1; // Default to '出席' (1)
                if (delay > TimeSpan.FromMinutes(10))
                {
                    newStatus = 2; // '遅刻' (2)
                }
                UpdateDutyStatus(studentId, today, newStatus);


            }
        }

        // 罰直カウントは personal_info.penalty_count を正とする（duty_schedule.penalty_count は使わない）
        // 通常日直(0): 遅刻10分+1, 1h+2, 3h+3, 未タッチ+4
        // 罰直(1): 上記に加え、10分以内は -2、未タッチ+4
        private void UpdatePenaltyCount(DataTable results)
        {
            DateTime dutyTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 8, 50, 0);

            foreach (DataRow row in results.Rows)
            {
                string studentId = row["student_id"].ToString();
                DateTime? actualTime = row.IsNull("actual_time_stamp") ? (DateTime?)null : DateTime.Parse(row["actual_time_stamp"].ToString());
                int penaltyCount = int.Parse(row["penalty_count"].ToString());

                int penaltyIncrement = 0;
                bool isPenaltyDuty = row["duty_type"].ToString() == "1";
                if (!isPenaltyDuty)
                {
                    if (actualTime != null)
                    {
                        TimeSpan delay = actualTime.Value - dutyTime;

                        if (delay > TimeSpan.FromHours(3))
                        {
                            penaltyIncrement = 3;
                        }
                        else if (delay > TimeSpan.FromHours(1))
                        {
                            penaltyIncrement = 2;
                        }
                        else if (delay > TimeSpan.FromMinutes(10))
                        {
                            penaltyIncrement = 1;
                        }
                    }
                    else
                    {
                        // actualTime が null の場合、最大のペナルティを適用
                        penaltyIncrement = 4;
                    }
                }
                else
                {
                    if (actualTime != null)
                    {
                        TimeSpan delay = actualTime.Value - dutyTime;

                        if (delay > TimeSpan.FromHours(3))
                        {
                            penaltyIncrement = 3;
                        }
                        else if (delay > TimeSpan.FromHours(1))
                        {
                            penaltyIncrement = 2;
                        }
                        else if (delay > TimeSpan.FromMinutes(10))
                        {
                            penaltyIncrement = 1;
                        }
                        else
                        {
                            penaltyIncrement = -2;
                        }
                    }
                    else
                    {
                        // actualTime が null の場合、最大のペナルティを適用
                        penaltyIncrement = 4;
                    }
                }


                UpdatePenalty(studentId, penaltyCount + penaltyIncrement);
            }
        }


        private void UpdateDutyStatus(string studentId, string date, int status)
        {
            string updateQuery = $@"
        UPDATE duty_schedule
        SET duty_status = {status}
        WHERE student_id = '{studentId}' AND duty_date = '{date}';
    ";
            Connector.ExecuteCommand(updateQuery);
        }

        private void UpdatePenalty(string studentId, int newPenaltyCount)
        {
            string updateQuery = $@"
        UPDATE personal_info
        SET penalty_count = {newPenaltyCount}
        WHERE student_id = '{studentId}';
    ";
            Connector.ExecuteCommand(updateQuery);
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            // 毎日 18:00 に1回だけ罰直カウントを更新
            DateTime now = DateTime.Now;
            if (now.Hour == 18 && now.Minute == 0 && lastPenaltyRunDate != now.Date)
            {
                lastPenaltyRunDate = now.Date;
                PerformDailyTask();
            }
        }
        private void PerformDailyTask()
        {
            DataTable fetchedData = FetchData();
            UpdatePenaltyCount(fetchedData);
        }
        //日直表示部分の表示設定
        private void dataGridView2_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            var row = dataGridView2.Rows[e.RowIndex];
            string attendanceTime = row.Cells["attendance_time"]?.Value?.ToString() ?? "-";
            bool hasTouched = attendanceTime != "-";
            bool beforeDeadline = DateTime.Now.TimeOfDay < DutyDeadline;
            bool isPenaltyDuty = row.Cells["duty_type"]?.Value?.ToString() == "1";

            if (dataGridView2.Columns[e.ColumnIndex].Name == "name" && e.Value != null)
            {
                string name = e.Value.ToString();
                if (isPenaltyDuty && !name.Contains("罰直"))
                    e.Value = name + "（罰直）";
            }

            if (dataGridView2.Columns[e.ColumnIndex].Name != "duty_status" || e.Value == null)
                return;

            switch (e.Value.ToString())
            {
                case "0":
                    if (!hasTouched && beforeDeadline)
                    {
                        var remaining = DutyDeadline - DateTime.Now.TimeOfDay;
                        e.Value = $"待機中（あと{remaining.Minutes}分）";
                    }
                    else if (!hasTouched)
                        e.Value = "未タッチ";
                    else
                        e.Value = "未出席";
                    break;
                case "1":
                    e.Value = "出席";
                    break;
                case "2":
                    e.Value = "遅刻";
                    break;
            }
        }

    }


}