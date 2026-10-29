using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace LabManager
{
    /// <summary>
    /// タッチ履歴編集。在席は当日タッチの奇数/偶数で決まるため、
    /// 履歴の追加・時刻変更・削除がテレビ右画面とまとめページの在席表示になる。
    /// 日直担当の最初のタッチは出席状況（8:50 から 10 分）も更新する。
    /// </summary>
    public class Form17 : Form
    {
        private const string ManualTerminalId = "EDIT";

        private readonly Setting mySqlSet;
        private readonly DateTime? initialDate;
        private readonly string initialStudentId;
        private bool accessGranted;
        private bool loading;
        private bool studentPinned;

        private DateTimePicker datePicker;
        private ComboBox comboStudent;
        private Button buttonPickDuty;
        private Label labelState;
        private DataGridView gridTouches;
        private DateTimePicker timePicker;
        private DataGridView gridLog;
        private Button buttonPresent;
        private Button buttonAbsent;
        private Button buttonAdd;
        private Button buttonUpdate;
        private Button buttonDelete;
        private Button buttonClose;

        private readonly List<TouchRow> touches = new List<TouchRow>();
        private int? pickedTouchId;
        private int dutyStatus = -1;
        private string dutyNames = "";
        private bool appliedInitialStudent;

        private sealed class TouchRow
        {
            public int Id;
            public DateTime Time;
            public string TerminalId;
            public string ChipId;
        }

        public Form17(Setting settings)
            : this(settings, null, null)
        {
        }

        public Form17(Setting settings, DateTime? initialDate, string initialStudentId)
        {
            mySqlSet = settings;
            this.initialDate = initialDate;
            this.initialStudentId = initialStudentId;
            BuildLayout();
            Font = UiFonts.Get(9F);
            labelState.Font = UiFonts.Get(11F, FontStyle.Bold);
        }

        private void BuildLayout()
        {
            Text = "17:タッチ履歴編集";
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(980, 720);
            MinimumSize = new Size(860, 620);

            var table = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 6,
                Padding = new Padding(8)
            };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 78));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 58));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 78));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 42));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 64));

            var filters = new Panel { Dock = DockStyle.Fill };
            var labelDate = new Label
            {
                Text = "対象日",
                AutoSize = true,
                Location = new Point(0, 10)
            };
            datePicker = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Location = new Point(52, 6),
                Width = 130
            };
            datePicker.ValueChanged += datePicker_ValueChanged;
            var labelStudent = new Label
            {
                Text = "学生",
                AutoSize = true,
                Location = new Point(196, 10)
            };
            comboStudent = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(236, 6),
                Width = 280
            };
            comboStudent.SelectedIndexChanged += comboStudent_SelectedIndexChanged;
            buttonPickDuty = new Button
            {
                Text = "日直を選ぶ",
                Location = new Point(528, 4),
                Size = new Size(120, 28)
            };
            buttonPickDuty.Click += buttonPickDuty_Click;
            var buttonReload = new Button
            {
                Text = "再読込",
                Location = new Point(656, 4),
                Size = new Size(90, 28)
            };
            buttonReload.Click += delegate { ReloadView(); };
            filters.Controls.AddRange(new Control[]
            {
                labelDate, datePicker, labelStudent, comboStudent, buttonPickDuty, buttonReload
            });

            labelState = new Label
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(8, 4, 8, 4),
                Text = "在席表示: --"
            };

            gridTouches = new DataGridView { Dock = DockStyle.Fill };
            ConfigureGrid(gridTouches);
            gridTouches.CellClick += gridTouches_CellClick;

            var actions = new Panel { Dock = DockStyle.Fill };
            var labelTime = new Label
            {
                Text = "タッチ時刻",
                AutoSize = true,
                Location = new Point(0, 8)
            };
            timePicker = new DateTimePicker
            {
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "HH:mm:ss",
                ShowUpDown = true,
                Location = new Point(72, 4),
                Width = 110
            };
            buttonPresent = new Button
            {
                Text = "在席にする",
                Location = new Point(196, 2),
                Size = new Size(110, 30)
            };
            buttonAbsent = new Button
            {
                Text = "不在にする",
                Location = new Point(312, 2),
                Size = new Size(110, 30)
            };
            buttonAdd = new Button
            {
                Text = "追加",
                Location = new Point(428, 2),
                Size = new Size(80, 30)
            };
            buttonUpdate = new Button
            {
                Text = "時刻を変更",
                Location = new Point(514, 2),
                Size = new Size(110, 30)
            };
            buttonDelete = new Button
            {
                Text = "削除",
                Location = new Point(630, 2),
                Size = new Size(80, 30)
            };
            buttonClose = new Button
            {
                Text = "閉じる",
                Location = new Point(716, 2),
                Size = new Size(80, 30)
            };
            var labelHintActions = new Label
            {
                Text = "在席にする / 不在にする は、表示が目的の状態になるまでタッチを1件足します。",
                AutoSize = false,
                Location = new Point(0, 40),
                Size = new Size(900, 28)
            };
            buttonPresent.Click += buttonPresent_Click;
            buttonAbsent.Click += buttonAbsent_Click;
            buttonAdd.Click += buttonAdd_Click;
            buttonUpdate.Click += buttonUpdate_Click;
            buttonDelete.Click += buttonDelete_Click;
            buttonClose.Click += delegate { Close(); };
            actions.Controls.AddRange(new Control[]
            {
                labelTime, timePicker, buttonPresent, buttonAbsent, buttonAdd,
                buttonUpdate, buttonDelete, buttonClose, labelHintActions
            });

            var logHost = new Panel { Dock = DockStyle.Fill };
            var labelLog = new Label
            {
                Text = "編集ログ（この人とこの日）",
                Dock = DockStyle.Top,
                Height = 22,
                TextAlign = ContentAlignment.BottomLeft
            };
            gridLog = new DataGridView { Dock = DockStyle.Fill };
            ConfigureGrid(gridLog);
            logHost.Controls.Add(gridLog);
            logHost.Controls.Add(labelLog);

            var hint = new Label
            {
                Dock = DockStyle.Fill,
                Text =
                    "在席は当日タッチが奇数、不在は偶数です。テレビ右画面とまとめページも同じ判定です。\n" +
                    "日直の最初のタッチが 8:50 から 10 分以内なら出席、超えると遅刻（18:00 の罰直対象）です。在席の確認だけなら 8:50 で記録できます。"
            };

            table.Controls.Add(filters, 0, 0);
            table.Controls.Add(labelState, 0, 1);
            table.Controls.Add(gridTouches, 0, 2);
            table.Controls.Add(actions, 0, 3);
            table.Controls.Add(logHost, 0, 4);
            table.Controls.Add(hint, 0, 5);
            Controls.Add(table);
            CancelButton = buttonClose;
            Load += Form17_Load;
        }

        private static void ConfigureGrid(DataGridView grid)
        {
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToResizeRows = false;
            grid.ReadOnly = true;
            grid.MultiSelect = false;
            grid.RowHeadersVisible = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            grid.BackgroundColor = Color.White;
            grid.BorderStyle = BorderStyle.FixedSingle;
        }

        private void Form17_Load(object sender, EventArgs e)
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

            TouchAuditLog.EnsureTable();
            DutyAuditLog.EnsureTable();

            loading = true;
            datePicker.Value = (initialDate ?? DateTime.Today).Date;
            LoadStudents();
            SelectPreferredStudent();
            loading = false;
            ReloadView();
            ApplyDefaultTime();
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

                label.Text = "タッチ履歴編集のパスワード:";
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

        private void datePicker_ValueChanged(object sender, EventArgs e)
        {
            if (loading || !accessGranted)
                return;

            if (!studentPinned)
            {
                loading = true;
                SelectPreferredStudent();
                loading = false;
            }

            ReloadView();
            ApplyDefaultTime();
        }

        private void comboStudent_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (loading || !accessGranted)
                return;

            studentPinned = true;
            ReloadView();
            ApplyDefaultTime();
        }

        private void buttonPickDuty_Click(object sender, EventArgs e)
        {
            studentPinned = false;
            loading = true;
            string selected = SelectPreferredStudent();
            loading = false;
            ReloadView();
            ApplyDefaultTime();

            if (string.IsNullOrEmpty(selected) || dutyStatus < 0)
            {
                MessageBox.Show(
                    "この日の日直担当が見つかりません。学生を選んでタッチを追加できます。",
                    Text);
            }
        }

        private void gridTouches_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (loading || e.RowIndex < 0)
                return;

            TouchRow row = TouchAtRow(e.RowIndex);
            if (row == null)
                return;

            pickedTouchId = row.Id;
            loading = true;
            timePicker.Value = row.Time;
            loading = false;
        }

        private void LoadStudents()
        {
            var students = new DataTable();
            if (!Connector.TryTableReader(
                "SELECT student_id, name, CONCAT(student_id, ' ', name) AS display " +
                "FROM personal_info WHERE " + PersonalInfoHelper.SqlStudentsOnly + " ORDER BY student_id",
                students,
                out string error))
            {
                MessageBox.Show("学生一覧を取得できません。\n" + error, Text);
                return;
            }

            comboStudent.DataSource = students;
            comboStudent.DisplayMember = "display";
            comboStudent.ValueMember = "student_id";
        }

        /// <summary>
        /// 対象日の日直を選ぶ。氏名に「伊藤」がいる担当を優先する。
        /// </summary>
        private string SelectPreferredStudent()
        {
            string preferred = null;
            if (!appliedInitialStudent && !string.IsNullOrEmpty(initialStudentId) && ContainsStudent(initialStudentId))
            {
                preferred = initialStudentId;
                appliedInitialStudent = true;
                studentPinned = true;
            }

            if (preferred == null)
                preferred = FindDutyStudentId();

            if (!string.IsNullOrEmpty(preferred) && ContainsStudent(preferred))
            {
                comboStudent.SelectedValue = preferred;
                return preferred;
            }

            return SelectedStudentId();
        }

        private string FindDutyStudentId()
        {
            var table = new DataTable();
            string day = SqlDay(SelectedDate);
            if (!Connector.TryTableReader($@"
                SELECT ds.student_id, pi.name
                FROM duty_schedule ds
                INNER JOIN personal_info pi ON ds.student_id = pi.student_id
                WHERE ds.duty_date = '{day}'
                ORDER BY ds.student_id", table, out _))
            {
                return null;
            }

            var ids = new List<string>();
            var names = new List<string>();
            foreach (DataRow row in table.Rows)
            {
                ids.Add(Convert.ToString(row["student_id"]));
                names.Add(Convert.ToString(row["name"]));
            }

            int index = AttendanceRules.IndexOfPreferredDuty(names);
            if (index < 0 || index >= ids.Count)
                return null;
            return ids[index];
        }

        private bool ContainsStudent(string studentId)
        {
            var table = comboStudent.DataSource as DataTable;
            if (table == null)
                return false;

            foreach (DataRow row in table.Rows)
            {
                if (Convert.ToString(row["student_id"]) == studentId)
                    return true;
            }

            return false;
        }

        private void ApplyDefaultTime()
        {
            DateTime day = SelectedDate;
            bool dutyWithoutTouch = dutyStatus >= 0 && touches.Count == 0;
            DateTime value;
            if (dutyWithoutTouch || day != DateTime.Today)
                value = day.Date + AttendanceRules.DutyDeadline;
            else
                value = DateTime.Now;

            loading = true;
            timePicker.Value = value;
            loading = false;
        }

        private void ReloadView()
        {
            if (!accessGranted)
                return;

            string studentId = SelectedStudentId();
            if (string.IsNullOrEmpty(studentId))
            {
                touches.Clear();
                dutyStatus = -1;
                labelState.Text = "学生を選べません。";
                gridTouches.DataSource = null;
                gridLog.DataSource = null;
                return;
            }

            LoadTouches(studentId);
            LoadDuty(studentId);
            BindTouchGrid();
            LoadAudit(studentId);
            UpdateStateLabel();
        }

        private void LoadTouches(string studentId)
        {
            touches.Clear();
            string day = SqlDay(SelectedDate);
            string next = SqlDay(SelectedDate.AddDays(1));
            var table = new DataTable();
            if (!Connector.TryTableReader($@"
                SELECT tl.id, tl.time_stamp, tl.terminal_id, tl.chip_id
                FROM touch_log tl
                INNER JOIN chip_list cl ON tl.chip_id = cl.chip_id
                WHERE cl.student_id = '{Escape(studentId)}'
                  AND tl.time_stamp >= '{day} 00:00:00'
                  AND tl.time_stamp < '{next} 00:00:00'
                ORDER BY tl.time_stamp, tl.id", table, out string error))
            {
                MessageBox.Show("タッチ履歴を取得できません。\n" + error, Text);
                return;
            }

            foreach (DataRow row in table.Rows)
            {
                touches.Add(new TouchRow
                {
                    Id = Convert.ToInt32(row["id"]),
                    Time = Convert.ToDateTime(row["time_stamp"]),
                    TerminalId = Convert.ToString(row["terminal_id"]),
                    ChipId = Convert.ToString(row["chip_id"])
                });
            }
        }

        private void LoadDuty(string studentId)
        {
            dutyStatus = -1;
            dutyNames = "";
            string day = SqlDay(SelectedDate);
            var table = new DataTable();
            if (!Connector.TryTableReader($@"
                SELECT ds.student_id, pi.name, ds.duty_status, ds.duty_type
                FROM duty_schedule ds
                INNER JOIN personal_info pi ON ds.student_id = pi.student_id
                WHERE ds.duty_date = '{day}'
                ORDER BY ds.student_id", table, out string error))
            {
                dutyNames = "日直を取得できません（" + error + "）";
                return;
            }

            var names = new List<string>();
            foreach (DataRow row in table.Rows)
            {
                string id = Convert.ToString(row["student_id"]);
                string name = Convert.ToString(row["name"]);
                names.Add(name);
                if (id == studentId)
                    dutyStatus = Convert.ToInt32(row["duty_status"]);
            }

            dutyNames = names.Count == 0 ? "なし" : string.Join("、", names.ToArray());
        }

        private void BindTouchGrid()
        {
            pickedTouchId = null;
            var table = new DataTable();
            table.Columns.Add("id", typeof(int));
            table.Columns.Add("touched_at", typeof(string));
            table.Columns.Add("terminal_id", typeof(string));
            table.Columns.Add("chip_id", typeof(string));
            foreach (TouchRow touch in touches)
            {
                table.Rows.Add(
                    touch.Id,
                    touch.Time.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                    touch.TerminalId,
                    touch.ChipId);
            }

            gridTouches.DataSource = table;
            if (gridTouches.Columns.Count >= 4)
            {
                gridTouches.Columns["id"].HeaderText = "履歴ID";
                gridTouches.Columns["touched_at"].HeaderText = "タッチ時刻";
                gridTouches.Columns["terminal_id"].HeaderText = "端末";
                gridTouches.Columns["chip_id"].HeaderText = "カードID";
                gridTouches.Columns["id"].Width = 70;
                gridTouches.Columns["touched_at"].Width = 180;
                gridTouches.Columns["terminal_id"].Width = 80;
                gridTouches.Columns["chip_id"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
        }

        private void LoadAudit(string studentId)
        {
            string day = SqlDay(SelectedDate);
            string next = SqlDay(SelectedDate.AddDays(1));
            var table = new DataTable();
            Connector.TryTableReader($@"
                SELECT edited_at AS 編集日時,
                       action AS 操作,
                       touch_id AS 履歴ID,
                       chip_id AS カードID,
                       old_time_stamp AS 変更前,
                       new_time_stamp AS 変更後,
                       terminal_id AS 端末
                FROM touch_edit_log
                WHERE student_id = '{Escape(studentId)}'
                  AND (
                        (old_time_stamp >= '{day} 00:00:00' AND old_time_stamp < '{next} 00:00:00')
                     OR (new_time_stamp >= '{day} 00:00:00' AND new_time_stamp < '{next} 00:00:00')
                  )
                ORDER BY edited_at DESC
                LIMIT 50", table, out _);
            gridLog.DataSource = table;
            gridLog.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
        }

        private void UpdateStateLabel()
        {
            string name = SelectedStudentName();
            int count = touches.Count;
            string state = StateText(count);
            string first = count == 0 ? "-" : touches[0].Time.ToString("HH:mm");
            string last = count == 0 ? "-" : touches[count - 1].Time.ToString("HH:mm");
            string parity = count == 0 ? "タッチなし" : (AttendanceRules.IsPresent(count) ? "奇数" : "偶数");
            string dutyLine = dutyStatus < 0
                ? "この人はこの日の日直ではありません。"
                : "日直の出席状況: " + AttendanceRules.DutyStatusLabel(dutyStatus) + "。";

            labelState.Text =
                name + "　在席表示: " + state +
                "　（タッチ " + count + " 回・" + parity + "　初回 " + first + "　最終 " + last + "）\n" +
                dutyLine + "　この日の日直: " + dutyNames;

            bool present = AttendanceRules.IsPresent(count) && count > 0;
            labelState.BackColor = present ? Color.White : Color.FromArgb(225, 225, 225);
            labelState.ForeColor = present ? Color.Black : Color.FromArgb(80, 80, 80);
        }

        private void buttonPresent_Click(object sender, EventArgs e)
        {
            if (AttendanceRules.IsPresent(touches.Count))
            {
                MessageBox.Show(SelectedStudentName() + " さんはすでに在席です。", Text);
                return;
            }

            InsertTouch(announce: true);
        }

        private void buttonAbsent_Click(object sender, EventArgs e)
        {
            if (touches.Count == 0)
            {
                MessageBox.Show("タッチがありません。在席にするか、追加してください。", Text);
                return;
            }

            if (!AttendanceRules.IsPresent(touches.Count))
            {
                MessageBox.Show(SelectedStudentName() + " さんはすでに不在です。", Text);
                return;
            }

            InsertTouch(announce: true);
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            InsertTouch(announce: true);
        }

        private void InsertTouch(bool announce)
        {
            if (!TryGetSelected(out string studentId, out string name))
                return;

            DateTime? time = ConfirmTouchTime(null, SelectedTouchTime());
            if (!time.HasValue)
                return;

            string chipId = EnsureChip(studentId);
            if (string.IsNullOrEmpty(chipId))
                return;

            string sqlTime = SqlDateTime(time.Value);
            if (!Connector.TryExecuteCommand($@"
                INSERT INTO touch_log (time_stamp, terminal_id, chip_id)
                VALUES ('{sqlTime}', '{ManualTerminalId}', '{Escape(chipId)}')", out string error))
            {
                MessageBox.Show("タッチの追加に失敗しました。\n" + error, Text);
                return;
            }

            int? newId = ReadLastInsertId();
            TouchAuditLog.Record("INSERT", newId, studentId, chipId, null, time.Value, ManualTerminalId);
            SyncDutyStatus(studentId);
            ReloadView();
            LiveViewRefresh.NotifyAttendance();
            if (announce)
                AnnouncePresence(name);
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            if (!TryGetSelected(out string studentId, out _))
                return;

            TouchRow row = SelectedTouch();
            if (row == null)
            {
                MessageBox.Show("時刻を変える行をクリックしてください。", Text);
                return;
            }

            DateTime? time = ConfirmTouchTime(row.Id, SelectedTouchTime());
            if (!time.HasValue)
                return;

            if (time.Value == row.Time)
            {
                MessageBox.Show("時刻は変わっていません。", Text);
                return;
            }

            if (!Connector.TryExecuteCommand($@"
                UPDATE touch_log tl
                INNER JOIN chip_list cl ON tl.chip_id = cl.chip_id
                SET tl.time_stamp = '{SqlDateTime(time.Value)}'
                WHERE tl.id = {row.Id.ToString(CultureInfo.InvariantCulture)}
                  AND cl.student_id = '{Escape(studentId)}'", out string error))
            {
                MessageBox.Show("時刻の変更に失敗しました。\n" + error, Text);
                return;
            }

            TouchAuditLog.Record("UPDATE", row.Id, studentId, row.ChipId, row.Time, time.Value, row.TerminalId);
            SyncDutyStatus(studentId);
            ReloadView();
            LiveViewRefresh.NotifyAttendance();
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (!TryGetSelected(out string studentId, out _))
                return;

            TouchRow row = SelectedTouch();
            if (row == null)
            {
                MessageBox.Show("削除する行をクリックしてください。", Text);
                return;
            }

            string when = row.Time.ToString("HH:mm:ss");
            DateTime? remainingFirst = PreviewFirstTouch(row.Id, null);
            int statusAfterDelete = remainingFirst.HasValue
                ? AttendanceRules.DutyStatusFromFirstTouch(remainingFirst.Value)
                : 0;
            string confirm = when + " のタッチを削除します。在席表示と、日直の出席状況も変わります。";
            if (dutyStatus >= 0 && statusAfterDelete == 2 && dutyStatus != 2)
                confirm = when + " のタッチを削除すると、残った最初のタッチが遅刻になり、18:00 の罰直対象です。削除しますか？";

            if (MessageBox.Show(confirm, Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            if (!Connector.TryExecuteCommand($@"
                DELETE tl FROM touch_log tl
                INNER JOIN chip_list cl ON tl.chip_id = cl.chip_id
                WHERE tl.id = {row.Id.ToString(CultureInfo.InvariantCulture)}
                  AND cl.student_id = '{Escape(studentId)}'", out string error))
            {
                MessageBox.Show("削除に失敗しました。\n" + error, Text);
                return;
            }

            TouchAuditLog.Record("DELETE", row.Id, studentId, row.ChipId, row.Time, null, row.TerminalId);
            SyncDutyStatus(studentId);
            ReloadView();
            LiveViewRefresh.NotifyAttendance();
        }

        private DateTime? ConfirmTouchTime(int? replacingId, DateTime chosen)
        {
            if (dutyStatus < 0)
                return chosen;

            DateTime? first = PreviewFirstTouch(replacingId, chosen);
            int newStatus = first.HasValue ? AttendanceRules.DutyStatusFromFirstTouch(first.Value) : 0;
            if (newStatus != 2 || dutyStatus == 2)
                return chosen;

            string name = SelectedStudentName();
            using (var dlg = new Form())
            {
                dlg.Text = "日直の出席時刻";
                dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
                dlg.StartPosition = FormStartPosition.CenterParent;
                dlg.ClientSize = new Size(480, 200);
                dlg.MaximizeBox = false;
                dlg.MinimizeBox = false;

                var label = new Label
                {
                    Text =
                        name + " さんはこの日の日直です。\n" +
                        chosen.ToString("HH:mm") + " は 8:50 から 10 分を超えるため「遅刻」になり、18:00 の罰直対象です。\n" +
                        "在席表示の確認だけなら、8:50 の出席として記録できます。",
                    Location = new Point(12, 12),
                    Size = new Size(456, 78)
                };
                var onTime = new Button
                {
                    Text = "8:50 で出席",
                    DialogResult = DialogResult.Yes,
                    Location = new Point(12, 140),
                    Size = new Size(140, 36)
                };
                var asIs = new Button
                {
                    Text = "この時刻で記録",
                    DialogResult = DialogResult.No,
                    Location = new Point(164, 140),
                    Size = new Size(150, 36)
                };
                var cancel = new Button
                {
                    Text = "キャンセル",
                    DialogResult = DialogResult.Cancel,
                    Location = new Point(326, 140),
                    Size = new Size(140, 36)
                };
                dlg.Controls.AddRange(new Control[] { label, onTime, asIs, cancel });
                dlg.CancelButton = cancel;

                DialogResult result = dlg.ShowDialog(this);
                if (result == DialogResult.Yes)
                    return chosen.Date + AttendanceRules.DutyDeadline;
                if (result == DialogResult.No)
                    return chosen;
                return null;
            }
        }

        private DateTime? PreviewFirstTouch(int? replacingId, DateTime? newTime)
        {
            DateTime? min = null;
            foreach (TouchRow touch in touches)
            {
                if (replacingId.HasValue && touch.Id == replacingId.Value)
                    continue;

                if (!min.HasValue || touch.Time < min.Value)
                    min = touch.Time;
            }

            if (newTime.HasValue && (!min.HasValue || newTime.Value < min.Value))
                min = newTime;

            return min;
        }

        private void SyncDutyStatus(string studentId)
        {
            string day = SqlDay(SelectedDate);
            var table = new DataTable();
            if (!Connector.TryTableReader($@"
                SELECT duty_status, duty_type
                FROM duty_schedule
                WHERE duty_date = '{day}' AND student_id = '{Escape(studentId)}'", table, out _))
            {
                return;
            }

            if (table.Rows.Count == 0)
                return;

            int oldStatus = Convert.ToInt32(table.Rows[0]["duty_status"]);
            string oldType = Convert.ToString(table.Rows[0]["duty_type"]);

            var times = new DataTable();
            string next = SqlDay(SelectedDate.AddDays(1));
            if (!Connector.TryTableReader($@"
                SELECT MIN(tl.time_stamp) AS first_touch
                FROM touch_log tl
                INNER JOIN chip_list cl ON tl.chip_id = cl.chip_id
                WHERE cl.student_id = '{Escape(studentId)}'
                  AND tl.time_stamp >= '{day} 00:00:00'
                  AND tl.time_stamp < '{next} 00:00:00'", times, out _))
            {
                return;
            }

            int newStatus = 0;
            if (times.Rows.Count > 0 && times.Rows[0]["first_touch"] != DBNull.Value)
                newStatus = AttendanceRules.DutyStatusFromFirstTouch(Convert.ToDateTime(times.Rows[0]["first_touch"]));

            if (newStatus == oldStatus)
                return;

            if (!Connector.TryExecuteCommand($@"
                UPDATE duty_schedule
                SET duty_status = {newStatus.ToString(CultureInfo.InvariantCulture)}
                WHERE duty_date = '{day}' AND student_id = '{Escape(studentId)}'", out string error))
            {
                MessageBox.Show("タッチは保存しましたが、日直の出席状況を更新できませんでした。\n" + error, Text);
                return;
            }

            DutyAuditLog.Record("UPDATE", day, studentId, oldStatus, newStatus, oldType, oldType);
        }

        private string EnsureChip(string studentId)
        {
            var table = new DataTable();
            if (!Connector.TryTableReader(
                "SELECT chip_id FROM chip_list WHERE student_id = '" + Escape(studentId) + "' ORDER BY chip_id",
                table,
                out string error))
            {
                MessageBox.Show("カードの確認に失敗しました。\n" + error, Text);
                return null;
            }

            string fallback = null;
            foreach (DataRow row in table.Rows)
            {
                string chip = Convert.ToString(row["chip_id"]);
                if (fallback == null)
                    fallback = chip;
                if (!chip.StartsWith("WEB:", StringComparison.Ordinal) &&
                    !chip.StartsWith("EDIT:", StringComparison.Ordinal))
                    return chip;
            }

            if (fallback != null)
                return fallback;

            string created = "EDIT:" + studentId;
            if (created.Length > 50)
            {
                MessageBox.Show("学籍番号が長すぎて編集用カードを作れません。", Text);
                return null;
            }

            if (!Connector.TryExecuteCommand($@"
                INSERT INTO chip_list (chip_id, student_id, system_id)
                VALUES ('{Escape(created)}', '{Escape(studentId)}', 'EDIT')", out error))
            {
                MessageBox.Show("カードが未登録で、編集用カードの作成に失敗しました。\n" + error, Text);
                return null;
            }

            return created;
        }

        private static int? ReadLastInsertId()
        {
            var table = new DataTable();
            if (!Connector.TryTableReader("SELECT LAST_INSERT_ID() AS id", table, out _))
                return null;
            if (table.Rows.Count == 0 || table.Rows[0]["id"] == DBNull.Value)
                return null;

            int id = Convert.ToInt32(table.Rows[0]["id"]);
            return id > 0 ? (int?)id : null;
        }

        private void AnnouncePresence(string name)
        {
            int count = touches.Count;
            string state = StateText(count);
            string first = count == 0 ? "-" : touches[0].Time.ToString("HH:mm");
            string last = count == 0 ? "-" : touches[count - 1].Time.ToString("HH:mm");
            string duty = dutyStatus < 0
                ? "\nこの日の日直ではありません。"
                : "\n日直の出席状況: " + AttendanceRules.DutyStatusLabel(dutyStatus);

            MessageBox.Show(
                name + " さんの在席表示は「" + state + "」です。\n" +
                "タッチ " + count + " 回　初回 " + first + "　最終 " + last + duty + "\n" +
                ReflectionText(),
                Text);
        }

        private string StateText(int count)
        {
            if (SelectedDate == DateTime.Today)
                return AttendanceRules.BoardLabel(count);
            if (count <= 0)
                return "未来室";
            return AttendanceRules.IsPresent(count) ? "在席のまま" : "退室";
        }

        private string ReflectionText()
        {
            if (SelectedDate == DateTime.Today)
                return "テレビ右画面とまとめページも同じ判定です。";
            return "まとめページの対象日に反映されます。テレビ右画面は今日のタッチだけを表示します。";
        }

        private bool TryGetSelected(out string studentId, out string name)
        {
            studentId = SelectedStudentId();
            name = SelectedStudentName();
            if (!string.IsNullOrEmpty(studentId))
                return true;

            MessageBox.Show("学生を選択してください。", Text);
            return false;
        }

        private string SelectedStudentId()
        {
            return comboStudent.SelectedValue == null ? "" : comboStudent.SelectedValue.ToString();
        }

        private string SelectedStudentName()
        {
            var row = comboStudent.SelectedItem as DataRowView;
            if (row == null)
                return "";
            return Convert.ToString(row["name"]);
        }

        private TouchRow SelectedTouch()
        {
            if (!pickedTouchId.HasValue)
                return null;

            foreach (TouchRow touch in touches)
            {
                if (touch.Id == pickedTouchId.Value)
                    return touch;
            }

            return null;
        }

        private TouchRow TouchAtRow(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= gridTouches.Rows.Count)
                return null;
            if (!gridTouches.Columns.Contains("id"))
                return null;

            object value = gridTouches.Rows[rowIndex].Cells["id"].Value;
            if (value == null || value == DBNull.Value)
                return null;

            int id = Convert.ToInt32(value);
            foreach (TouchRow touch in touches)
            {
                if (touch.Id == id)
                    return touch;
            }

            return null;
        }

        private DateTime SelectedDate
        {
            get { return datePicker.Value.Date; }
        }

        private DateTime SelectedTouchTime()
        {
            DateTime day = SelectedDate;
            DateTime time = timePicker.Value;
            return new DateTime(day.Year, day.Month, day.Day, time.Hour, time.Minute, time.Second);
        }

        private static string SqlDay(DateTime day)
        {
            return day.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        private static string SqlDateTime(DateTime value)
        {
            return value.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        }

        private static string Escape(string value)
        {
            return (value ?? "").Replace("'", "''").Replace("\\", "\\\\");
        }
    }

    static class TouchAuditLog
    {
        public static void EnsureTable()
        {
            const string sql = @"
                CREATE TABLE IF NOT EXISTS touch_edit_log (
                    id INT NOT NULL AUTO_INCREMENT,
                    edited_at DATETIME NOT NULL,
                    action VARCHAR(10) NOT NULL,
                    touch_id INT NULL,
                    student_id VARCHAR(20) NOT NULL,
                    chip_id VARCHAR(50) NULL,
                    old_time_stamp DATETIME NULL,
                    new_time_stamp DATETIME NULL,
                    terminal_id VARCHAR(10) NULL,
                    PRIMARY KEY (id),
                    KEY idx_touch_edit_time (edited_at),
                    KEY idx_touch_edit_student (student_id)
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8";
            Connector.ExecuteCommand(sql);
        }

        public static void Record(
            string action,
            int? touchId,
            string studentId,
            string chipId,
            DateTime? oldTime,
            DateTime? newTime,
            string terminalId)
        {
            string touchSql = touchId.HasValue
                ? touchId.Value.ToString(CultureInfo.InvariantCulture)
                : "NULL";
            string sql = $@"
                INSERT INTO touch_edit_log
                    (edited_at, action, touch_id, student_id, chip_id,
                     old_time_stamp, new_time_stamp, terminal_id)
                VALUES
                    (NOW(), '{Escape(action)}', {touchSql}, '{Escape(studentId)}', {SqlText(chipId)},
                     {SqlTime(oldTime)}, {SqlTime(newTime)}, {SqlText(terminalId)})";
            Connector.ExecuteCommand(sql);
        }

        private static string SqlText(string value)
        {
            if (value == null)
                return "NULL";
            return "'" + Escape(value) + "'";
        }

        private static string SqlTime(DateTime? value)
        {
            if (!value.HasValue)
                return "NULL";
            return "'" + value.Value.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) + "'";
        }

        private static string Escape(string value)
        {
            return (value ?? "").Replace("\\", "\\\\").Replace("'", "''");
        }
    }
}
