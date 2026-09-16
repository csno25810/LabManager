using System;
using System.Data;
using System.Windows.Forms;

namespace LabManager
{
    public partial class Form12 : Form
    {
        private readonly Setting mySqlSet;
        private DataTable studentSearchTable = new DataTable();

        public Form12(Setting settings)
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

        private void buttonLoadLatestTouch_Click(object sender, EventArgs e)
        {
            if (!EnsureConnected())
                return;

            var table = new DataTable();
            Connector.TableReader(
                "SELECT chip_id, time_stamp, terminal_id FROM touch_log ORDER BY time_stamp DESC LIMIT 1",
                table);

            if (table.Rows.Count == 0)
            {
                MessageBox.Show("タッチ履歴がありません。", "カード管理");
                return;
            }

            textBoxChipId.Text = table.Rows[0]["chip_id"]?.ToString() ?? string.Empty;
            LoadChipInfo();
        }

        private void buttonLoadChip_Click(object sender, EventArgs e)
        {
            LoadChipInfo();
        }

        private void LoadChipInfo()
        {
            string chipId = textBoxChipId.Text.Trim();
            if (string.IsNullOrWhiteSpace(chipId))
            {
                MessageBox.Show("カードIDを入力してください。", "カード管理");
                return;
            }

            if (!EnsureConnected())
                return;

            var table = new DataTable();
            Connector.TableReader($@"
                SELECT
                    c.chip_id AS カードID,
                    c.student_id AS 学籍番号,
                    IFNULL(p.name, '') AS 氏名,
                    IFNULL(p.mail, '') AS メール,
                    c.system_id AS システムID
                FROM chip_list c
                LEFT JOIN personal_info p ON c.student_id = p.student_id
                WHERE c.chip_id = '{EscapeSql(chipId)}'
                LIMIT 1",
                table);

            dataGridViewChipInfo.DataSource = table;

            if (table.Rows.Count == 0)
                labelChipStatus.Text = "状態: 未登録のカード";
            else
                labelChipStatus.Text = "状態: 学生に関連付け済み";
        }

        private void buttonDeleteChip_Click(object sender, EventArgs e)
        {
            string chipId = textBoxChipId.Text.Trim();
            if (string.IsNullOrWhiteSpace(chipId))
            {
                MessageBox.Show("カードIDを入力してください。", "カード管理");
                return;
            }

            if (!EnsureConnected())
                return;

            var table = new DataTable();
            Connector.TableReader(
                $"SELECT 1 FROM chip_list WHERE chip_id = '{EscapeSql(chipId)}' LIMIT 1",
                table);

            if (table.Rows.Count == 0)
            {
                MessageBox.Show("このカードは chip_list に登録されていません。", "カード管理");
                return;
            }

            var confirm = MessageBox.Show(
                $"カードID「{chipId}」の関連付けを消去します。よろしいですか？",
                "カード管理",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes)
                return;

            Connector.ExecuteCommand($"DELETE FROM chip_list WHERE chip_id = '{EscapeSql(chipId)}'");
            MessageBox.Show("消去しました。", "カード管理");
            LoadChipInfo();
        }

        private void buttonSearchStudent_Click(object sender, EventArgs e)
        {
            string keyword = textBoxSearchValue.Text.Trim();
            if (string.IsNullOrWhiteSpace(keyword))
            {
                MessageBox.Show("検索キーワードを入力してください。", "カード管理");
                return;
            }

            if (!EnsureConnected())
                return;

            string column = GetSearchColumnName();
            studentSearchTable = new DataTable();
            Connector.TableReader(
                $"SELECT student_id AS 学籍番号, name AS 氏名, mail AS メール FROM personal_info WHERE {column} LIKE '%{EscapeSql(keyword)}%' ORDER BY student_id",
                studentSearchTable);

            dataGridViewStudents.DataSource = studentSearchTable;
            dataGridViewStudents.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;

            if (studentSearchTable.Rows.Count == 0)
                MessageBox.Show("学生情報が見つかりませんでした。", "カード管理");
        }

        private void dataGridViewStudents_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewStudents.CurrentRow == null)
                return;

            string studentId = dataGridViewStudents.CurrentRow.Cells["学籍番号"].Value?.ToString();
            if (!string.IsNullOrWhiteSpace(studentId) && string.IsNullOrWhiteSpace(textBoxSystemId.Text))
                textBoxSystemId.Text = DefaultSystemId(studentId);
        }

        private void buttonLink_Click(object sender, EventArgs e)
        {
            string chipId = textBoxLinkChipId.Text.Trim();
            string systemId = textBoxSystemId.Text.Trim();

            if (string.IsNullOrWhiteSpace(chipId))
            {
                MessageBox.Show("カードIDを入力してください。", "カード管理");
                return;
            }

            if (dataGridViewStudents.CurrentRow == null)
            {
                MessageBox.Show("関連付ける学生を一覧から選択してください。", "カード管理");
                return;
            }

            string studentId = dataGridViewStudents.CurrentRow.Cells["学籍番号"].Value?.ToString();
            string name = dataGridViewStudents.CurrentRow.Cells["氏名"].Value?.ToString();

            if (string.IsNullOrWhiteSpace(studentId))
                return;

            if (string.IsNullOrWhiteSpace(systemId))
                systemId = DefaultSystemId(studentId);

            if (!EnsureConnected())
                return;

            var studentCheck = new DataTable();
            Connector.TableReader(
                $"SELECT 1 FROM personal_info WHERE student_id = '{EscapeSql(studentId)}' LIMIT 1",
                studentCheck);

            if (studentCheck.Rows.Count == 0)
            {
                MessageBox.Show("選択した学生が personal_info に存在しません。", "カード管理");
                return;
            }

            var chipCheck = new DataTable();
            Connector.TableReader(
                $"SELECT student_id FROM chip_list WHERE chip_id = '{EscapeSql(chipId)}' LIMIT 1",
                chipCheck);

            if (chipCheck.Rows.Count > 0)
            {
                string existingStudent = chipCheck.Rows[0]["student_id"]?.ToString();
                var confirm = MessageBox.Show(
                    $"カードID「{chipId}」は既に学籍番号「{existingStudent}」に関連付けられています。\n学籍番号「{studentId}（{name}）」に上書きしますか？",
                    "カード管理",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirm != DialogResult.Yes)
                    return;

                Connector.ExecuteCommand($@"
                    UPDATE chip_list
                    SET student_id = '{EscapeSql(studentId)}', system_id = '{EscapeSql(systemId)}'
                    WHERE chip_id = '{EscapeSql(chipId)}'");
            }
            else
            {
                var confirm = MessageBox.Show(
                    $"カードID「{chipId}」を学籍番号「{studentId}（{name}）」に関連付けます。よろしいですか？",
                    "カード管理",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirm != DialogResult.Yes)
                    return;

                Connector.ExecuteCommand($@"
                    INSERT INTO chip_list (chip_id, student_id, system_id)
                    VALUES ('{EscapeSql(chipId)}', '{EscapeSql(studentId)}', '{EscapeSql(systemId)}')");
            }

            MessageBox.Show("関連付けました。", "カード管理");
            textBoxChipId.Text = chipId;
            textBoxLinkChipId.Text = chipId;
            LoadChipInfo();
        }

        private void buttonCopyChipId_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(textBoxChipId.Text))
                textBoxLinkChipId.Text = textBoxChipId.Text.Trim();
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

        private static string DefaultSystemId(string studentId)
        {
            return "S" + (studentId ?? string.Empty).PadLeft(9, '0');
        }

        private static string EscapeSql(string value)
        {
            return (value ?? string.Empty).Replace("'", "''");
        }
    }
}
