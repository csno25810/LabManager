using System;
using System.Data;
using System.Drawing;
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
            SetupEditUi();
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
            string familyName = textBoxName.Text.Trim();
            string givenName = textBoxGivenName.Text.Trim();
            string mail = textBoxMail.Text.Trim();

            if (string.IsNullOrWhiteSpace(studentId))
            {
                MessageBox.Show("学籍番号を入力してください。", "学生情報の追加");
                return;
            }

            if (string.IsNullOrWhiteSpace(familyName))
            {
                MessageBox.Show("苗字を入力してください。", "学生情報の追加");
                return;
            }

            if (string.IsNullOrWhiteSpace(givenName))
            {
                MessageBox.Show("名前を入力してください。", "学生情報の追加");
                return;
            }

            string name = PersonalInfoHelper.JoinDisplayName(familyName, givenName);

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
            string portalError;
            if (!LabUserStore.TryCreateForStudent(studentId, out portalError))
            {
                MessageBox.Show(
                    "学生情報は登録しましたが、LabPortal ログインの作成に失敗しました。\n" +
                    (portalError ?? "") +
                    "\nLabPortal を再起動すると自動で作られることがあります。",
                    "学生情報の追加");
            }
            else
            {
                MessageBox.Show(
                    "登録しました。\nLabPortal の初期パスワードは " + LabCommon.PasswordHash.DefaultPassword + " です。",
                    "学生情報の追加");
            }
            textBoxStudentId.Clear();
            textBoxName.Clear();
            textBoxGivenName.Clear();
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
                MessageBox.Show("検索キーワードを入力してください。", "学生情報の検索");
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
                MessageBox.Show("学生情報が見つかりませんでした。", "学生情報の検索");
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

            LabUserStore.DeleteForStudent(studentId);
            Connector.ExecuteCommand($"DELETE FROM personal_info WHERE student_id = '{EscapeSql(studentId)}'");
            MessageBox.Show("消去しました。", "学生情報の削除");
            buttonSearch_Click(sender, e);
        }

        private TextBox textBoxEditId;
        private TextBox textBoxEditFamily;
        private TextBox textBoxEditGiven;
        private TextBox textBoxEditMail;
        private Button buttonSave;

        private void SetupEditUi()
        {
            tabPageDelete.Text = "検索・編集・削除";
            labelDeleteTitle.Text = "検索・編集・削除";
            ClientSize = new Size(584, 470);
            dataGridViewStudents.Height = 118;
            buttonDelete.Location = new Point(463, 400);

            var labelId = new Label { Text = "学籍番号", Location = new Point(20, 220), AutoSize = true };
            textBoxEditId = new TextBox { Location = new Point(120, 217), Size = new Size(120, 19), ReadOnly = true };
            var labelFamily = new Label { Text = "苗字", Location = new Point(20, 250), AutoSize = true };
            textBoxEditFamily = new TextBox { Location = new Point(120, 247), Size = new Size(180, 19) };
            var labelGiven = new Label { Text = "名前", Location = new Point(320, 250), AutoSize = true };
            textBoxEditGiven = new TextBox { Location = new Point(360, 247), Size = new Size(180, 19) };
            var labelMailEdit = new Label { Text = "メール", Location = new Point(20, 280), AutoSize = true };
            textBoxEditMail = new TextBox { Location = new Point(120, 277), Size = new Size(418, 19) };
            buttonSave = new Button { Text = "保存", Location = new Point(370, 400), Size = new Size(75, 23) };
            buttonSave.Click += buttonSave_Click;

            tabPageDelete.Controls.Add(labelId);
            tabPageDelete.Controls.Add(textBoxEditId);
            tabPageDelete.Controls.Add(labelFamily);
            tabPageDelete.Controls.Add(textBoxEditFamily);
            tabPageDelete.Controls.Add(labelGiven);
            tabPageDelete.Controls.Add(textBoxEditGiven);
            tabPageDelete.Controls.Add(labelMailEdit);
            tabPageDelete.Controls.Add(textBoxEditMail);
            tabPageDelete.Controls.Add(buttonSave);

            dataGridViewStudents.SelectionChanged += dataGridViewStudents_SelectionChanged;
        }

        private void dataGridViewStudents_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewStudents.CurrentRow == null)
                return;

            textBoxEditId.Text = dataGridViewStudents.CurrentRow.Cells["学籍番号"].Value?.ToString() ?? "";
            string fullName = dataGridViewStudents.CurrentRow.Cells["氏名"].Value?.ToString() ?? "";
            string family;
            string given;
            PersonalInfoHelper.SplitDisplayName(fullName, out family, out given);
            textBoxEditFamily.Text = family;
            textBoxEditGiven.Text = given;
            textBoxEditMail.Text = dataGridViewStudents.CurrentRow.Cells["メール"].Value?.ToString() ?? "";
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            string studentId = textBoxEditId.Text.Trim();
            string familyName = textBoxEditFamily.Text.Trim();
            string givenName = textBoxEditGiven.Text.Trim();
            string mail = textBoxEditMail.Text.Trim();

            if (string.IsNullOrWhiteSpace(studentId))
            {
                MessageBox.Show("一覧から学生を選択してください。", "学生情報の編集");
                return;
            }

            if (string.IsNullOrWhiteSpace(familyName))
            {
                MessageBox.Show("苗字を入力してください。", "学生情報の編集");
                return;
            }

            if (string.IsNullOrWhiteSpace(givenName))
            {
                MessageBox.Show("名前を入力してください。", "学生情報の編集");
                return;
            }

            if (string.IsNullOrWhiteSpace(mail))
            {
                MessageBox.Show("メールアドレスを入力してください。", "学生情報の編集");
                return;
            }

            if (!EnsureConnected())
                return;

            string name = PersonalInfoHelper.JoinDisplayName(familyName, givenName);
            Connector.ExecuteCommand(
                $"UPDATE personal_info SET name = '{EscapeSql(name)}', mail = '{EscapeSql(mail)}' WHERE student_id = '{EscapeSql(studentId)}'");
            MessageBox.Show("保存しました。", "学生情報の編集");
            if (!string.IsNullOrWhiteSpace(textBoxSearchValue.Text))
                buttonSearch_Click(sender, e);
        }

        private static string EscapeSql(string value)
        {
            return (value ?? string.Empty).Replace("'", "''");
        }
    }
}
