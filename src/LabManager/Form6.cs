using System;
using System.Data;
using System.Windows.Forms;

namespace LabManager
{
    public partial class Form6 : Form
    {
        private Setting mySqlSet;
        private DataTable diaryTable = new DataTable();

        public Form6(Setting settings)
        {
            InitializeComponent();
            this.mySqlSet = settings;

            dateTimePickerTo.Value = DateTime.Today;
            dateTimePickerFrom.Value = DateTime.Today.AddDays(-30);
        }

        private bool EnsureConnected()
        {
            if (Connector.IsConnected)
                return true;

            return Connector.Connect(
                mySqlSet.UserID,
                mySqlSet.PassWd,
                mySqlSet.DataBaseName,
                mySqlSet.ServerIP);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string title = textBox1.Text.Trim();
            string studentId = textBox2.Text.Trim();
            string content = textBox3.Text.Trim();

            if (string.IsNullOrWhiteSpace(title))
            {
                MessageBox.Show("タイトルを入力してください");
                return;
            }

            if (string.IsNullOrWhiteSpace(studentId))
            {
                MessageBox.Show("学籍番号を入力してください");
                return;
            }

            if (string.IsNullOrWhiteSpace(content))
            {
                MessageBox.Show("本文を入力してください");
                return;
            }

            if (!EnsureConnected())
                return;

            InsertDiaryEntry(title, studentId, content);
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
        }

        private void InsertDiaryEntry(string title, string studentId, string content)
        {
            DateTime today = DateTime.Today;
            string commandText = $@"
                INSERT INTO diary_log (student_id, dialy_date, title, content)
                VALUES ('{EscapeSql(studentId)}', '{today:yyyy-MM-dd}', '{EscapeSql(title)}', '{EscapeSql(content)}')";
            Connector.ExecuteCommand(commandText);
            MessageBox.Show("提出が完了しました。");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox3.Clear();
        }

        private void buttonLoadDiaries_Click(object sender, EventArgs e)
        {
            LoadDiaries();
        }

        private void LoadDiaries()
        {
            if (!EnsureConnected())
                return;

            DateTime startDate = dateTimePickerFrom.Value.Date;
            DateTime endDate = dateTimePickerTo.Value.Date;

            if (startDate > endDate)
            {
                MessageBox.Show("開始日は終了日以前にしてください。", "日誌閲覧");
                return;
            }

            string studentFilter = textBoxFilterStudent.Text.Trim();
            string studentCondition = string.IsNullOrWhiteSpace(studentFilter)
                ? string.Empty
                : $" AND d.student_id = '{EscapeSql(studentFilter)}'";

            string commandText = $@"
                SELECT
                    DATE_FORMAT(d.dialy_date, '%Y-%m-%d') AS 日付,
                    d.student_id AS 学籍番号,
                    IFNULL(p.name, '') AS 氏名,
                    d.title AS タイトル,
                    d.content AS 本文
                FROM diary_log d
                LEFT JOIN personal_info p ON d.student_id = p.student_id
                WHERE d.dialy_date BETWEEN '{startDate:yyyy-MM-dd}' AND '{endDate:yyyy-MM-dd}'
                {studentCondition}
                ORDER BY d.dialy_date DESC, d.student_id";

            diaryTable = new DataTable();
            Connector.TableReader(commandText, diaryTable);
            dataGridViewDiaries.DataSource = diaryTable;
            dataGridViewDiaries.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;

            if (diaryTable.Columns.Contains("本文"))
                dataGridViewDiaries.Columns["本文"].Visible = false;

            textBoxDiaryContent.Clear();

            if (diaryTable.Rows.Count == 0)
                MessageBox.Show("該当する日誌がありません。", "日誌閲覧");
        }

        private void dataGridViewDiaries_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewDiaries.CurrentRow == null)
            {
                textBoxDiaryContent.Clear();
                return;
            }

            object content = dataGridViewDiaries.CurrentRow.Cells["本文"].Value;
            textBoxDiaryContent.Text = content?.ToString() ?? string.Empty;
        }

        private static string EscapeSql(string value)
        {
            return (value ?? string.Empty).Replace("'", "''");
        }
    }
}
