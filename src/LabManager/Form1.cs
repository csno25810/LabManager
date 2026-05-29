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

        // 設定ファイルをリードする
        public Setting mySqlSet = new Setting();

        string OffSeat = "不在";
        string OnSeat = "在席";
        private System.Timers.Timer dailyTimer;

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

            // LabelTEST
            maskedTextBox1.Font = new System.Drawing.Font("メイリオ", 24F);
            maskedTextBox1.Location = new Point(1, winHeight - btn1Size - ClockMargin + 1);
            maskedTextBox1.Size = new Size(winWidth, ClockMargin - 1);
            maskedTextBox1.Text = DateTime.Now.ToString("yyyy/MM/dd(ddd) HH:mm");


            // データ表示部分の初期設定
            dataGridView1.Location = new Point(0, 0);
            dataGridView1.Size = new Size(winWidth, winHeight * 2 / 3);
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.ColumnHeadersVisible = true;
            dataGridView1.ColumnHeadersHeight = ColumnHeadHeight;
            dataGridView1.RowTemplate.Height = (winHeight - btn1Size) / 30;
            dataGridView1.Font = new System.Drawing.Font("メイリオ", 15F);
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("メイリオ", 10F);

            dataGridView2.Location = new Point(0, winHeight * 2 / 3);
            dataGridView2.Size = new Size(winWidth, winHeight * 1 / 3 - btn1Size - ClockMargin);
            dataGridView2.RowHeadersVisible = false;
            dataGridView2.ColumnHeadersVisible = true;
            dataGridView2.ColumnHeadersHeight = ColumnHeadHeight;
            dataGridView2.RowTemplate.Height = (winHeight - btn1Size) / 25;
            dataGridView2.Font = new System.Drawing.Font("メイリオ", 10F);
            dataGridView2.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("メイリオ", 10F);



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
            if (!Connector.IsConnected) return;

            timer1.Enabled = false;
            DataTable dataSql = new DataTable();
            DataTable dataSqlCount = new DataTable();
            DateTime dt = DateTime.Now;

            string today = dt.ToString("yyyy-MM-dd");
            Connector.TableReader("SELECT personal_info.student_id,name,MIN(date_format(time_stamp,'%H:%i')), MAX(date_format(time_stamp,'%H:%i')),COUNT(name) FROM((touch_log INNER JOIN chip_list ON touch_log.chip_id = chip_list.chip_id)INNER JOIN personal_info ON chip_list.student_id = personal_info.student_id) WHERE touch_log.time_stamp LIKE \"" + today + "%\"GROUP BY name", dataSql);




            // データが存在しない場合は警告して終了
            if (dataSql.Rows.Count == 0)
            {
                MessageBox.Show("データが見つかりませんでした。");
                return;
            }






            object[] Result = dataSql.Rows[0].ItemArray;

            // カラム名の変更
            dataSql.Columns[3].ColumnName = "AAA";
            dataSql.Columns[2].ColumnName = "BBB";

            // 描画領域の設定
            if (dataSql.Rows.Count < 15)
            {
                dataGridView1.RowTemplate.Height = (winHeight - btn1Size - ColumnHeadHeight - ClockMargin) / 15;
            }
            else
            {
                dataGridView1.RowTemplate.Height = (winHeight - btn1Size - ColumnHeadHeight - ClockMargin) / dataSql.Rows.Count;
            }

            DataTable newView = dataSql;
            newView.Columns.Add("State", typeof(string));

            for (int i = 0; i < dataSql.Rows.Count; i++)
            {
                if (int.Parse(dataSql.Rows[i]["COUNT(name)"].ToString()) % 2 == 0)
                {
                    newView.Rows[i]["State"] = OffSeat;
                }
                else
                {
                    newView.Rows[i]["State"] = OnSeat;
                }
            }
            newView.Columns.Remove("COUNT(name)");

            // 内容をバインドし表示する。
            dataGridView1.DataSource = newView;

            if (dataGridView1.RowCount <= 0)
            {
                return;
            }

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

            dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Selected = false;


            //DataGridView1にバインドされているDataTableを取得
            DataTable sorttable = (DataTable)dataGridView1.DataSource;
            DataView dv = sorttable.DefaultView;
            dv.Sort = "State DESC, AAA ASC";
            dataGridView1.Columns[3].HeaderCell.SortGlyphDirection = SortOrder.Descending;
            dataGridView1.Columns[4].HeaderCell.SortGlyphDirection = SortOrder.Descending;

            dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Selected = false;

            // 色換え
            CellColorChange();

            DataTable fetchedData = FetchData();
            CheckAndUpdateDutyStatus(fetchedData);





            string query = $@"
            SELECT 
                ds.student_id, 
                pi.name, 
                COALESCE(MIN(DATE_FORMAT(tl.time_stamp, '%H:%i')), '-') AS '出席時刻',
                ds.duty_status, 
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
                ds.student_id, pi.name, ds.duty_status, ds.penalty_count
            ";

            // duty_schedule テーブルからのデータ取得
            DataTable dataDutySchedule = new DataTable();
            //Connector.TableReader($"SELECT student_id, duty_date, duty_status, penalty_count FROM duty_schedule WHERE duty_date = '{NowDay}'", dataDutySchedule);
            //Connector.TableReader($"SELECT personal_info.student_id, name, MIN(DATE_FORMAT(touch_log.time_stamp, '%H:%i')) AS '出席時刻', duty_schedule.duty_status, duty_schedule.penalty_count FROM duty_schedule INNER JOIN chip_list ON duty_schedule.student_id = chip_list.student_id INNER JOIN personal_info ON chip_list.student_id = personal_info.student_id LEFT JOIN touch_log ON chip_list.chip_id = touch_log.chip_id WHERE duty_schedule.duty_date = '" + NowDay + "' GROUP BY name", dataDutySchedule);
            Connector.TableReader(query, dataDutySchedule);

            SetupDataGridView2(dataDutySchedule);
            //CheckLateArrivalsAndUpdate();//日直判定のやつ
            timer1.Enabled = true;
        }




        private void SetupDataGridView2(DataTable data)
        {
            dataGridView2.DataSource = data;
            dataGridView2.AutoGenerateColumns = true;  // Ensure this is set to manipulate columns manually


            // Set column headers text, assuming there are enough columns
            dataGridView2.Columns[0].HeaderText = "学籍番号";
            dataGridView2.Columns[1].HeaderText = "日直氏名";
            dataGridView2.Columns[2].HeaderText = "出席時刻";
            dataGridView2.Columns[3].HeaderText = "出席状況";
            dataGridView2.Columns[4].HeaderText = "罰直回数";

            // Set column widths or other properties as needed
            dataGridView2.Columns[0].Width = 130;
            dataGridView2.Columns[1].Width = winWidth - (130 + 150 + 150 + 100);
            dataGridView2.Columns[2].Width = 150;
            dataGridView2.Columns[3].Width = 150;
            dataGridView2.Columns[4].Width = 100;
        }




        private void CellColorChange()
        {
            dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Selected = false;
            // 色換え
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                if (dataGridView1[4, i].Value.ToString() == OffSeat)
                {
                    if (i % 2 == 0)
                    {
                        dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
                    }
                    else
                    {
                        dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
                    }
                }


                if (dataGridView1[4, i].Value.ToString() == OnSeat)
                {
                    if (i % 2 == 0)
                    {
                        dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(255)))), ((int)(((byte)(241)))));
                    }
                    else
                    {
                        dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(255)))), ((int)(((byte)(230)))));
                    }
                }


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

        private void UpdatePenaltyCount(DataTable results)
        {
            //日直の始業時間を8:50に設定
            DateTime dutyTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 8, 50, 0);
            string today = DateTime.Now.ToString("yyyy-MM-dd");

            foreach (DataRow row in results.Rows)
            {
                string studentId = row["student_id"].ToString();
                DateTime? actualTime = row.IsNull("actual_time_stamp") ? (DateTime?)null : DateTime.Parse(row["actual_time_stamp"].ToString());
                int penaltyCount = int.Parse(row["penalty_count"].ToString());

                int penaltyIncrement = 0;
                string duty_type = row["duty_type"].ToString();
                // actualTime が null ではない場合のみ遅延を計算
                if (duty_type == "0")
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
                if (duty_type == "1")//罰直として登録した場合
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


                UpdatePenalty(studentId, today, penaltyCount + penaltyIncrement);
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

        private void UpdatePenalty(string studentId, string date, int newPenaltyCount)
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
            // 毎日18時00分に実行するタスク
            if (DateTime.Now.Hour == 16 && DateTime.Now.Minute == 0 && DateTime.Now.Second == 0)
            {
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
            if (dataGridView2.Columns[e.ColumnIndex].Name == "duty_status" && e.Value != null)
            {
                // duty_statusの値に基づくテキストの変更と色の設定
                Color cellColor = Color.White; // デフォルト色
                switch (e.Value.ToString())
                {
                    case "0":
                        e.Value = "未出席";
                        cellColor = Color.FromArgb(255, 204, 204);  // 淡いピンクより少し濃い色
                        break;
                    case "1":
                        e.Value = "出席";
                        cellColor = Color.FromArgb(204, 255, 204);  // 淡いグリーン
                        break;
                    case "2":
                        e.Value = "遅刻";
                        cellColor = Color.FromArgb(255, 255, 204);  // 淡いイエロー
                        break;
                }

                // 行全体に色を適用
                for (int i = 0; i < dataGridView2.Columns.Count; i++)
                {
                    dataGridView2.Rows[e.RowIndex].Cells[i].Style.BackColor = cellColor;
                }
            }
        }

    }


}