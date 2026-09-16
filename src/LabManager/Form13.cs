using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace LabManager
{
    public partial class Form13 : Form
    {
        private readonly Setting mySqlSet;
        private DataTable dutyTable = new DataTable();
        private bool accessGranted;

        public Form13(Setting settings)
        {
            mySqlSet = settings;
            InitializeComponent();

            comboBoxStatus.Items.AddRange(new object[] { "0: 未出席", "1: 出席", "2: 遅刻" });
            comboBoxStatus.SelectedIndex = 0;
            comboBoxType.Items.AddRange(new object[] { "0: 通常日直", "1: 罰直" });
            comboBoxType.SelectedIndex = 0;
        }

        private void Form13_Load(object sender, EventArgs e)
        {
            if (!VerifyAccess())
            {
                accessGranted = false;
                Close();
                return;
            }

            accessGranted = true;

            if (!EnsureConnected())
            {
                MessageBox.Show("DB に接続できません。", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Close();
                return;
            }

            DutyAuditLog.EnsureTable();
            LoadStudents();
            dateTimePickerFilter.Value = DateTime.Today;
            LoadDutyRecords();
            LoadAuditLog();
        }

        private bool VerifyAccess()
        {
            if (string.IsNullOrEmpty(mySqlSet.DebugPassword))
                return true;

            using (var dlg = new Form())
            using (var label = new Label())
            using (var textBox = new TextBox())
            using (var ok = new Button())
            using (var cancel = new Button())
            {
                dlg.Text = "パスワード";
                dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
                dlg.StartPosition = FormStartPosition.CenterParent;
                dlg.ClientSize = new Size(320, 130);
                dlg.MaximizeBox = false;
                dlg.MinimizeBox = false;

                label.Text = "出席状況編集のパスワード:";
                label.Location = new Point(12, 12);
                label.Size = new Size(280, 20);

                textBox.Location = new Point(12, 40);
                textBox.Size = new Size(280, 22);
                textBox.PasswordChar = '*';

                ok.Text = "OK";
                ok.DialogResult = DialogResult.OK;
                ok.Location = new Point(132, 78);
                ok.Size = new Size(75, 28);

                cancel.Text = "キャンセル";
                cancel.DialogResult = DialogResult.Cancel;
                cancel.Location = new Point(217, 78);
                cancel.Size = new Size(75, 28);

                dlg.Controls.AddRange(new Control[] { label, textBox, ok, cancel });
                dlg.AcceptButton = ok;
                dlg.CancelButton = cancel;

                if (dlg.ShowDialog(this) != DialogResult.OK)
                    return false;

                return textBox.Text == mySqlSet.DebugPassword;
            }
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

        private void LoadStudents()
        {
            var students = new DataTable();
            Connector.TableReader("SELECT student_id, name FROM personal_info ORDER BY student_id", students);
            comboBoxStudent.DataSource = students;
            comboBoxStudent.DisplayMember = "name";
            comboBoxStudent.ValueMember = "student_id";
        }

        private string SelectedDate => dateTimePickerFilter.Value.ToString("yyyy-MM-dd");

        private void LoadDutyRecords()
        {
            string query = $@"
                SELECT ds.duty_date AS 日付,
                       ds.student_id AS 学籍番号,
                       pi.name AS 氏名,
                       ds.duty_status AS 出席状況,
                       ds.duty_type AS 種類
                FROM duty_schedule ds
                INNER JOIN personal_info pi ON ds.student_id = pi.student_id
                WHERE ds.duty_date = '{SelectedDate}'
                ORDER BY ds.student_id";

            dutyTable = new DataTable();
            Connector.TableReader(query, dutyTable);
            dataGridViewDuty.DataSource = dutyTable;
            dataGridViewDuty.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
        }

        private void LoadAuditLog()
        {
            var logTable = new DataTable();
            string query = $@"
                SELECT edited_at AS 編集日時,
                       action AS 操作,
                       duty_date AS 日付,
                       student_id AS 学籍番号,
                       old_duty_status AS 変更前状況,
                       new_duty_status AS 変更後状況,
                       old_duty_type AS 変更前種類,
                       new_duty_type AS 変更後種類
                FROM duty_edit_log
                WHERE duty_date = '{SelectedDate}'
                ORDER BY edited_at DESC
                LIMIT 100";
            Connector.TableReader(query, logTable);
            dataGridViewLog.DataSource = logTable;
            dataGridViewLog.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
        }

        private void buttonReload_Click(object sender, EventArgs e)
        {
            if (!accessGranted || !EnsureConnected())
                return;

            LoadDutyRecords();
            LoadAuditLog();
        }

        private void dataGridViewDuty_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewDuty.SelectedRows.Count == 0)
                return;

            var row = dataGridViewDuty.SelectedRows[0];
            string studentId = row.Cells["学籍番号"].Value.ToString();
            comboBoxStudent.SelectedValue = studentId;
            comboBoxStatus.SelectedIndex = ParseInt(row.Cells["出席状況"].Value, 0);
            comboBoxType.SelectedIndex = ParseInt(row.Cells["種類"].Value, 0);
        }

        private static int ParseInt(object value, int fallback)
        {
            if (value == null)
                return fallback;
            return int.TryParse(value.ToString(), out int n) ? n : fallback;
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (!EnsureConnected())
                return;

            string studentId = comboBoxStudent.SelectedValue?.ToString();
            if (string.IsNullOrEmpty(studentId))
                return;

            int status = comboBoxStatus.SelectedIndex;
            int type = comboBoxType.SelectedIndex;

            string insert = $@"
                INSERT INTO duty_schedule (duty_date, student_id, duty_status, penalty_count, duty_type)
                VALUES ('{SelectedDate}', '{EscapeSql(studentId)}', {status}, 0, '{type}')";
            Connector.ExecuteCommand(insert);

            DutyAuditLog.Record("INSERT", SelectedDate, studentId, null, status, null, type.ToString());
            LoadDutyRecords();
            LoadAuditLog();
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            if (dataGridViewDuty.SelectedRows.Count == 0)
            {
                MessageBox.Show("更新する行を選択してください。", Text);
                return;
            }

            if (!EnsureConnected())
                return;

            var row = dataGridViewDuty.SelectedRows[0];
            string studentId = row.Cells["学籍番号"].Value.ToString();
            int oldStatus = ParseInt(row.Cells["出席状況"].Value, 0);
            string oldType = row.Cells["種類"].Value.ToString();
            int newStatus = comboBoxStatus.SelectedIndex;
            int newType = comboBoxType.SelectedIndex;

            string update = $@"
                UPDATE duty_schedule
                SET duty_status = {newStatus}, duty_type = '{newType}'
                WHERE duty_date = '{SelectedDate}' AND student_id = '{EscapeSql(studentId)}'";
            Connector.ExecuteCommand(update);

            DutyAuditLog.Record("UPDATE", SelectedDate, studentId, oldStatus, newStatus, oldType, newType.ToString());
            LoadDutyRecords();
            LoadAuditLog();
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (dataGridViewDuty.SelectedRows.Count == 0)
            {
                MessageBox.Show("削除する行を選択してください。", Text);
                return;
            }

            if (MessageBox.Show("選択した出席状況データを削除します。", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            if (!EnsureConnected())
                return;

            var row = dataGridViewDuty.SelectedRows[0];
            string studentId = row.Cells["学籍番号"].Value.ToString();
            int oldStatus = ParseInt(row.Cells["出席状況"].Value, 0);
            string oldType = row.Cells["種類"].Value.ToString();

            string delete = $@"
                DELETE FROM duty_schedule
                WHERE duty_date = '{SelectedDate}' AND student_id = '{EscapeSql(studentId)}'";
            Connector.ExecuteCommand(delete);

            DutyAuditLog.Record("DELETE", SelectedDate, studentId, oldStatus, null, oldType, null);
            LoadDutyRecords();
            LoadAuditLog();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private static string EscapeSql(string value)
        {
            return (value ?? "").Replace("'", "''");
        }
    }
}
