using System;
using System.Data;
using System.Windows.Forms;

namespace LabManager
{
    public partial class Form11 : Form
    {
        private readonly Setting mySqlSet;
        private DataTable searchResultTable = new DataTable();

        public Form11(Setting settings)
        {
            InitializeComponent();
            mySqlSet = settings;
            comboBoxSearchField.SelectedIndex = 0;
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

        private void buttonRegister_Click(object sender, EventArgs e)
        {
            string studentId = textBoxStudentId.Text.Trim();
            string name = textBoxName.Text.Trim();
            string mail = textBoxMail.Text.Trim();

            if (string.IsNullOrWhiteSpace(studentId))
            {
                MessageBox.Show("学籍番号を入力してください。", "学生情報の追加");
                return;
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("氏名を入力してください。", "学生情報の追加");
                return;
            }

            if (string.IsNullOrWhiteSpace(mail))
            {
                MessageBox.Show("メールアドレスを入力してください。", "学生情報の追加");
                return;
            }

            if (!EnsureConnected())
                return;

            if (StudentExists(studentId))
            {
                MessageBox.Show("学籍番号がすでに登録されています。", "学生情報の追加");
                return;
            }

            var confirm = MessageBox.Show(
                $"学籍番号: {studentId}\n氏名: {name}\nメール: {mail}\n\n登録します。よろしいですか？",
                "学生情報の追加",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
                return;

            string commandText = $@"
                INSERT INTO personal_info (student_id, name, mail, penalty_count)
                VALUES ('{EscapeSql(studentId)}', '{EscapeSql(name)}', '{EscapeSql(mail)}', 0)";
            Connector.ExecuteCommand(commandText);

            MessageBox.Show("登録しました。", "学生情報の追加");
            textBoxStudentId.Clear();
            textBoxName.Clear();
            textBoxMail.Clear();
        }

        private bool StudentExists(string studentId)
        {
            var table = new DataTable();
            Connector.TableReader(
                $"SELECT 1 FROM personal_info WHERE student_id = '{EscapeSql(studentId)}' LIMIT 1",
                table);
            return table.Rows.Count > 0;
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            string keyword = textBoxSearchValue.Text.Trim();
            if (string.IsNullOrWhiteSpace(keyword))
            {
                MessageBox.Show("検索キーワードを入力してください。", "学生情報の削除");
                return;
            }

            if (!EnsureConnected())
                return;

            string column = GetSearchColumnName();
            searchResultTable = new DataTable();
            Connector.TableReader(
                $"SELECT student_id AS 学籍番号, name AS 氏名, mail AS メール FROM personal_info WHERE {column} LIKE '%{EscapeSql(keyword)}%' ORDER BY student_id",
                searchResultTable);

            dataGridViewStudents.DataSource = searchResultTable;
            dataGridViewStudents.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;

            if (searchResultTable.Rows.Count == 0)
                MessageBox.Show("学生情報が見つかりませんでした。", "学生情報の削除");
        }

        private string GetSearchColumnName()
        {
            switch (comboBoxSearchField.SelectedIndex)
            {
                case 1: return "name";
                case 2: return "mail";
                default: return "student_id";
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (dataGridViewStudents.CurrentRow == null)
            {
                MessageBox.Show("削除する学生を一覧から選択してください。", "学生情報の削除");
                return;
            }

            string studentId = dataGridViewStudents.CurrentRow.Cells["学籍番号"].Value?.ToString();
            string name = dataGridViewStudents.CurrentRow.Cells["氏名"].Value?.ToString();
            string mail = dataGridViewStudents.CurrentRow.Cells["メール"].Value?.ToString();

            if (string.IsNullOrWhiteSpace(studentId))
                return;

            var confirm = MessageBox.Show(
                $"以下の学生情報を消去します。よろしいですか？\n\n学籍番号: {studentId}\n氏名: {name}\nメール: {mail}",
                "学生情報の削除",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes)
                return;

            if (!EnsureConnected())
                return;

            Connector.ExecuteCommand($"DELETE FROM personal_info WHERE student_id = '{EscapeSql(studentId)}'");
            MessageBox.Show("消去しました。", "学生情報の削除");
            buttonSearch_Click(sender, e);
        }

        private static string EscapeSql(string value)
        {
            return (value ?? string.Empty).Replace("'", "''");
        }
    }
}
