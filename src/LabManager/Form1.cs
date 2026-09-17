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
        int TeacherSectionHeaderHeight = 24;
        int studentGridHeight;
        int teacherRowHeight;
        static readonly TimeSpan DutyDeadline = new TimeSpan(8, 50, 0);

        private Label lblOccupancy;
        private Label lblDutyHeader;
        private Label lblTeacherHeader;
        private DataGridView dataGridViewTeacher;

        // 設定ファイルをリードする
        public Setting mySqlSet = new Setting();

        string OffSeat = "不在";
        string OnSeat = "在席";
        private System.Timers.Timer dailyTimer;
        private DateTime? lastPenaltyRunDate;
        /// <summary>テレビ左半分の表示専用カレンダー (TvCalendarPanel / Form14)</summary>
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
            teacherRowHeight = CalcAttendanceRowHeight(7);
            int teacherBlockHeight = TeacherSectionHeaderHeight + teacherRowHeight + 2;
            studentGridHeight = grid1Height - teacherBlockHeight;

            lblDutyHeader.Location = new Point(0, SummaryBarHeight + grid1Height);
            lblDutyHeader.Size = new Size(winWidth, DutyHeaderHeight);

            lblTeacherHeader = new Label
            {
                Location = new Point(0, SummaryBarHeight + studentGridHeight),
                Size = new Size(winWidth, TeacherSectionHeaderHeight),
                Font = UiFonts.Get(10F, FontStyle.Bold),
                ForeColor = Color.Black,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                TextAlign = ContentAlignment.MiddleLeft,
                Text = "  【先生】"
            };

            dataGridViewTeacher = new DataGridView
            {
                Location = new Point(0, SummaryBarHeight + studentGridHeight + TeacherSectionHeaderHeight),
                Size = new Size(winWidth, teacherRowHeight + 2),
                RowHeadersVisible = false,
                ColumnHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeColumns = false,
                AllowUserToResizeRows = false,
                ReadOnly = true,
                MultiSelect = false,
                ScrollBars = ScrollBars.None,
                Font = UiFonts.Get(15F),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            dataGridViewTeacher.CellFormatting += AttendanceGrid_CellFormatting;

            // データ表示部分の初期設定
            dataGridView1.Location = new Point(0, SummaryBarHeight);
            dataGridView1.Size = new Size(winWidth, studentGridHeight);
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.ColumnHeadersVisible = true;
            dataGridView1.ColumnHeadersHeight = ColumnHeadHeight;
            dataGridView1.RowTemplate.Height = (winHeight - btn1Size) / 30;
            dataGridView1.Font = UiFonts.Get(15F);
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = UiFonts.Get(10F);
            dataGridView1.CellFormatting += AttendanceGrid_CellFormatting;

            ConfigureAttendanceGrid(dataGridView1);
            ConfigureAttendanceGrid(dataGridViewTeacher);

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
            Controls.Add(lblTeacherHeader);
            Controls.Add(dataGridViewTeacher);
            lblOccupancy.BringToFront();
            lblDutyHeader.BringToFront();
            lblTeacherHeader.BringToFront();
            dataGridViewTeacher.BringToFront();

            Shown += Form1_Shown;
        }

        private void UpdateOccupancySummary(int presentCount, int todayCount)
        {
            lblOccupancy.Text = $"在室 {presentCount} 人　|　本日来室 {todayCount} 人";
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            if (tvLeftPanel != null)
                return;

            tvLeftPanel = new Form14(mySqlSet);
            tvLeftPanel.ConfigureForTvDisplay();
            // 左半分は独立ウィンドウとして表示（Owner を付けると位置がずれることがある）
            tvLeftPanel.Show();
            FormClosed += Form1_FormClosed;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (tvLeftPanel != null && !tvLeftPanel.IsDisposed)
                tvLeftPanel.Close();
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
            DateTime dt = DateTime.Now;
            string today = dt.ToString("yyyy-MM-dd");

            string attendanceQuery = $@"
                SELECT
                    pi.student_id,
                    pi.name,
                    COALESCE(MIN(DATE_FORMAT(tl.time_stamp, '%H:%i')), '-') AS first_touch,
                    COALESCE(MAX(DATE_FORMAT(tl.time_stamp, '%H:%i')), '-') AS last_touch,
                    COUNT(tl.chip_id) AS touch_count
                FROM personal_info pi
                LEFT JOIN chip_list cl ON pi.student_id = cl.student_id
                LEFT JOIN touch_log tl ON cl.chip_id = tl.chip_id AND tl.time_stamp LIKE '{today}%'
                WHERE {PersonalInfoHelper.SqlStudentsOnlyAliased}
                GROUP BY pi.student_id, pi.name
                ORDER BY pi.student_id";
            Connector.TableReader(attendanceQuery, dataSql);

            DataTable teacherSql = new DataTable();
            string teacherQuery = $@"
                SELECT
                    pi.student_id,
                    pi.name,
                    COALESCE(MIN(DATE_FORMAT(tl.time_stamp, '%H:%i')), '-') AS first_touch,
                    COALESCE(MAX(DATE_FORMAT(tl.time_stamp, '%H:%i')), '-') AS last_touch,
                    COUNT(tl.chip_id) AS touch_count
                FROM personal_info pi
                LEFT JOIN chip_list cl ON pi.student_id = cl.student_id
                LEFT JOIN touch_log tl ON cl.chip_id = tl.chip_id AND tl.time_stamp LIKE '{today}%'
                WHERE pi.student_id = '{PersonalInfoHelper.TeacherStudentId}'
                GROUP BY pi.student_id, pi.name";
            Connector.TableReader(teacherQuery, teacherSql);

            int rowHeight = CalcAttendanceRowHeight(dataSql.Rows.Count);
            dataGridView1.RowTemplate.Height = rowHeight;
            if (dataGridViewTeacher != null)
            {
                dataGridViewTeacher.RowTemplate.Height = rowHeight;
                dataGridViewTeacher.Height = rowHeight + 2;
            }

            int presentCount = ApplyAttendanceState(dataSql, OnSeat, OffSeat);
            int todayCount = CountTodayVisitors(dataSql);
            int teacherPresent = ApplyAttendanceState(teacherSql, OnSeat, OffSeat);
            int teacherToday = CountTodayVisitors(teacherSql);
            presentCount += teacherPresent;
            todayCount += teacherToday;

            dataSql.Columns.Remove("touch_count");
            if (teacherSql.Columns.Contains("touch_count"))
                teacherSql.Columns.Remove("touch_count");

            UpdateOccupancySummary(presentCount, todayCount);

            dataGridView1.DataSource = dataSql;
            BindAttendanceColumns(dataGridView1, showHeaders: true, teacherGrid: false);
            dataGridViewTeacher.DataSource = teacherSql;
            BindAttendanceColumns(dataGridViewTeacher, showHeaders: false, teacherGrid: true);

            ApplyAttendanceGridColors(dataGridView1);
            ApplyAttendanceGridColors(dataGridViewTeacher);

            bool isDutyDay = LabCalendarStore.IsDutyDay(dt.Date);
            if (isDutyDay)
            {
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
                    ds.student_id, pi.name, ds.duty_status, ds.duty_type, pi.penalty_count";

                DataTable dataDutySchedule = new DataTable();
                Connector.TableReader(query, dataDutySchedule);
                SetupDataGridView2(dataDutySchedule);
                UpdateDutyHeader(dataDutySchedule, true);
            }
            else
            {
                dataGridView2.DataSource = null;
                UpdateDutyHeader(null, false);
            }

            timer1.Enabled = true;
        }

        private static int ApplyAttendanceState(DataTable table, string onSeat, string offSeat)
        {
            if (!table.Columns.Contains("State"))
                table.Columns.Add("State", typeof(string));

            int presentCount = 0;
            for (int i = 0; i < table.Rows.Count; i++)
            {
                int touchCount = Convert.ToInt32(table.Rows[i]["touch_count"]);
                if (touchCount % 2 == 1)
                {
                    table.Rows[i]["State"] = onSeat;
                    presentCount++;
                }
                else
                {
                    table.Rows[i]["State"] = offSeat;
                }
            }

            return presentCount;
        }

        private static int CountTodayVisitors(DataTable table)
        {
            int count = 0;
            for (int i = 0; i < table.Rows.Count; i++)
            {
                if (Convert.ToInt32(table.Rows[i]["touch_count"]) > 0)
                    count++;
            }

            return count;
        }

        private int CalcAttendanceRowHeight(int studentCount)
        {
            int attendanceAreaHeight = winHeight - btn1Size - ColumnHeadHeight - ClockMargin
                - SummaryBarHeight - DutyHeaderHeight;
            if (studentCount < 15)
                return Math.Max(40, attendanceAreaHeight / 15);
            return Math.Max(40, attendanceAreaHeight / studentCount);
        }

        private void BindAttendanceColumns(DataGridView grid, bool showHeaders, bool teacherGrid)
        {
            if (grid.Columns.Count < 5)
                return;

            const int idWidth = 130;
            const int touchWidth = 150;
            const int stateWidth = 100;
            const int fixedWithoutId = touchWidth + touchWidth + stateWidth;
            const int fixedWithId = idWidth + fixedWithoutId;

            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            grid.ColumnHeadersVisible = showHeaders;

            grid.Columns[0].HeaderText = "学籍番号";
            grid.Columns[1].HeaderText = "氏名";
            grid.Columns[2].HeaderText = "初回タッチ時刻";
            grid.Columns[3].HeaderText = "最終タッチ時刻";
            grid.Columns[4].HeaderText = "状態";

            grid.Columns[0].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            grid.Columns[1].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            grid.Columns[2].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            grid.Columns[3].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            grid.Columns[4].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            grid.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;

            grid.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            if (teacherGrid)
            {
                grid.Columns[0].Visible = false;
                grid.Columns[1].Width = winWidth - fixedWithoutId;
            }
            else
            {
                grid.Columns[0].Visible = true;
                grid.Columns[0].Width = idWidth;
                grid.Columns[1].Width = winWidth - fixedWithId;
            }

            grid.Columns[2].Width = touchWidth;
            grid.Columns[3].Width = touchWidth;
            grid.Columns[4].Width = stateWidth;
        }

        private void AttendanceGrid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            var grid = (DataGridView)sender;
            if (grid.Columns[e.ColumnIndex].Name != "student_id" && grid.Columns[e.ColumnIndex].Index != 0)
                return;

            string studentId = grid.Rows[e.RowIndex].Cells["student_id"]?.Value?.ToString();
            if (PersonalInfoHelper.IsTeacher(studentId))
                e.Value = "";
        }

        private void UpdateDutyHeader(DataTable dutyData, bool isDutyDay)
        {
            if (!isDutyDay)
            {
                lblDutyHeader.Text = "  本日の日直（本日は授業日ではありません）";
                return;
            }

            lblDutyHeader.Text = dutyData == null || dutyData.Rows.Count == 0
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




        private void ConfigureAttendanceGrid(DataGridView grid)
        {
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
            grid.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;
            grid.AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.White;
            grid.AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.Black;
            grid.DefaultCellStyle.SelectionBackColor = Color.White;
            grid.DefaultCellStyle.SelectionForeColor = Color.Black;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.DataBindingComplete += AttendanceGrid_DataBindingComplete;
        }

        private void AttendanceGrid_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            ApplyAttendanceGridColors((DataGridView)sender);
        }

        private void CellColorChange()
        {
            ApplyAttendanceGridColors(dataGridView1);
        }

        private void ApplyAttendanceGridColors(DataGridView grid)
        {
            if (grid == null || grid.RowCount == 0)
                return;

            grid.ClearSelection();
            grid.CurrentCell = null;

            DataGridViewColumn stateColumn = grid.Columns["State"];
            if (stateColumn == null)
                return;

            for (int i = 0; i < grid.RowCount; i++)
            {
                var row = grid.Rows[i];
                bool isPresent = row.Cells[stateColumn.Index].Value?.ToString() == OnSeat;
                Color backColor;
                Color foreColor;
                Font font;
                if (isPresent)
                {
                    backColor = Color.White;
                    foreColor = Color.Black;
                    font = UiFonts.Get(15F, FontStyle.Bold);
                }
                else
                {
                    backColor = Color.FromArgb(225, 225, 225);
                    foreColor = Color.FromArgb(110, 110, 110);
                    font = UiFonts.Get(15F, FontStyle.Regular);
                }

                row.DefaultCellStyle.BackColor = backColor;
                row.DefaultCellStyle.ForeColor = foreColor;
                row.DefaultCellStyle.Font = font;
                row.DefaultCellStyle.SelectionBackColor = backColor;
                row.DefaultCellStyle.SelectionForeColor = foreColor;
            }

            grid.Invalidate();
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
            if (!LabCalendarStore.IsDutyDay(DateTime.Now.Date))
                return;

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