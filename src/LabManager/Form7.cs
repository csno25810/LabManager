using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace LabManager
{
    public partial class Form7 : Form
    {
        private Setting mySqlSet;
        DataTable DutyTable = new DataTable();
        int winHeight = (int)(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height * 0.8);
        int winWidth = (int)(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width * 0.8);

        public Form7(Setting settings)
        {
            this.Size = new System.Drawing.Size(winWidth, winHeight);
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(winWidth / 9, winHeight / 9);


            InitializeComponent();
            this.mySqlSet = settings;

            // DataGridViewの設定
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect; // 行単位で選択
            dataGridView1.MultiSelect = true; // 複数行選択を許可
            dataGridView1.SelectionChanged += DataGridView1_SelectionChanged; // 選択変更時のイベント

            // 変更ボタンの初期設定
            button1.Enabled = false; // 初期状態は無効
        }

        private void Form7_Load(object sender, EventArgs e)
        {
            SetUpDataGridView();
        }

        private void SetUpDataGridView()
        {
            DutyTable = new DataTable();
            DateTime Today = DateTime.Today;
            dataGridView1.Size = new Size(winWidth * 6 / 10, winHeight);
            int addMonth = 12;
            string startDate = Today.ToString("yyyy-MM-dd"); // 開始日
            string endDate = Today.AddMonths(addMonth).ToString("yyyy-MM-dd"); // 12ヶ月後の日
            dataGridView1.Font = new Font("メイリオ", 12);  // フォント種類とサイズ指定

            string dutyQuery = $@"
            SELECT 
                DATE_FORMAT(duty_date, '%W') AS day_of_week,  -- 最初に曜日名
                duty_date, 
                student_id
            FROM 
                duty_schedule
            WHERE 
                duty_date BETWEEN '{startDate}' AND '{endDate}'
                AND duty_type = '0'
            ORDER BY 
                duty_date;
            ";

            // クエリを実行してDataTableにデータを読み込む
            Connector.TableReader(dutyQuery, DutyTable);

            // DataTableをDataGridViewにバインド
            dataGridView1.DataSource = DutyTable;
            dataGridView1.Columns[0].HeaderText = "曜日";  // 曜日列のヘッダ
            dataGridView1.Columns[1].HeaderText = "日直の日付";
            dataGridView1.Columns[2].HeaderText = "学生番号";

            dataGridView1.Columns[0].Width = (int)(winWidth*0.2);
            dataGridView1.Columns[1].Width = (int)(winWidth * 0.2);
            dataGridView1.Columns[2].Width = (int)(winWidth * 0.2);
        }

        // DataGridViewで行選択が変更されたときに呼び出される
        private void DataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            // 選択行が2つの場合のみボタンを有効化
            button1.Enabled = (dataGridView1.SelectedRows.Count == 2);
        }

        // 変更ボタンがクリックされたとき
        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count != 2)
            {
                MessageBox.Show("2行を選択してください。");
                return;
            }

            // 選択された行からstudent_idを取得
            var row1 = dataGridView1.SelectedRows[0];
            var row2 = dataGridView1.SelectedRows[1];

            string studentId1 = row1.Cells["student_id"].Value.ToString();
            string studentId2 = row2.Cells["student_id"].Value.ToString();

            // 確認ダイアログ
            var confirmResult = MessageBox.Show(
                $"以下の2つのstudent_idを交換します。\n\n" +
                $"Row1: {studentId1}\n" +
                $"Row2: {studentId2}\n\n" +
                "よろしいですか？",
                "確認",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirmResult == DialogResult.Yes)
            {
                // student_idの交換
                row1.Cells["student_id"].Value = studentId2;
                row2.Cells["student_id"].Value = studentId1;

                // データベースに反映
                SwapStudentIdsInDatabase(studentId1, studentId2);

                // 表示更新
                dataGridView1.Refresh();
            }
        }

        // データベースでstudent_idを交換する処理
        private void SwapStudentIdsInDatabase(string studentId1, string studentId2)
        {
            string updateQuery1 = $@"
            UPDATE duty_schedule
            SET student_id = '{studentId2}'
            WHERE student_id = '{studentId1}'";

            string updateQuery2 = $@"
            UPDATE duty_schedule
            SET student_id = '{studentId1}'
            WHERE student_id = '{studentId2}'";

            Connector.ExecuteCommand(updateQuery1);
            Connector.ExecuteCommand(updateQuery2);
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
