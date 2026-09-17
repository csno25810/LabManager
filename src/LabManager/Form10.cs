using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace LabManager
{
    /// <summary>
    /// CalendarEditor（Form10）— MENU から開くカレンダー編集画面。
    /// 授業日・予定メモを DB に保存する。表示専用は TvCalendarPanel (Form14)。
    /// </summary>
    public partial class Form10 : Form
    {
        private DateTime currentMonth = DateTime.Today;
        private Dictionary<DateTime, LabCalendarDay> dayMap = new Dictionary<DateTime, LabCalendarDay>();
        private Dictionary<DateTime, string> dutySurnames = new Dictionary<DateTime, string>();
        private bool classDayMode;

        private Button btnAddMemo;
        private Button btnDeleteMemo;
        private Button btnClassDayMode;
        private Label lblStatus;
        private TableLayoutPanel tableWeekdays;

        public Form10(Setting settings)
        {
            InitializeComponent();
            Text = "CalendarEditor";
            BuildToolbar();
            BuildWeekdayHeader();
        }

        private void BuildToolbar()
        {
            btnAddMemo = new Button
            {
                Text = "予定を作る",
                Location = new Point(12, 10),
                Size = new Size(120, 32),
                Font = UiFonts.Get(10F, FontStyle.Bold)
            };
            btnAddMemo.Click += btnAddMemo_Click;

            btnDeleteMemo = new Button
            {
                Text = "予定を削除",
                Location = new Point(140, 10),
                Size = new Size(120, 32),
                Font = UiFonts.Get(10F, FontStyle.Bold)
            };
            btnDeleteMemo.Click += btnDeleteMemo_Click;

            btnClassDayMode = new Button
            {
                Text = "授業日を決める",
                Location = new Point(268, 10),
                Size = new Size(140, 32),
                Font = UiFonts.Get(10F, FontStyle.Bold)
            };
            btnClassDayMode.Click += btnClassDayMode_Click;

            lblStatus = new Label
            {
                Location = new Point(418, 14),
                Size = new Size(350, 24),
                Font = UiFonts.Get(9F),
                Text = "開発名: CalendarEditor / 表示側: TvCalendarPanel"
            };

            tableWeekdays = new TableLayoutPanel
            {
                ColumnCount = 7,
                RowCount = 1,
                Location = new Point(12, 48),
                Size = new Size(760, 24)
            };
            for (int i = 0; i < 7; i++)
            {
                tableWeekdays.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14.28F));
            }

            tableLayoutPanel1.Location = new Point(12, 74);
            tableLayoutPanel1.Size = new Size(760, 476);
            lblMonth.Location = new Point(300, 560);
            btnPrevMonth.Location = new Point(200, 556);
            btnNextMonth.Location = new Point(520, 556);
            ClientSize = new Size(784, 600);

            Controls.Add(btnAddMemo);
            Controls.Add(btnDeleteMemo);
            Controls.Add(btnClassDayMode);
            Controls.Add(lblStatus);
            Controls.Add(tableWeekdays);
        }

        private void BuildWeekdayHeader()
        {
            tableWeekdays.Controls.Clear();
            string[] names = { "日", "月", "火", "水", "木", "金", "土" };
            for (int i = 0; i < 7; i++)
            {
                Color foreColor = Color.Black;
                if (i == 0) foreColor = Color.DarkRed;
                if (i == 6) foreColor = Color.DarkBlue;

                tableWeekdays.Controls.Add(new Label
                {
                    Text = names[i],
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = UiFonts.Get(10F, FontStyle.Bold),
                    ForeColor = foreColor,
                    BackColor = i == 0 ? Color.FromArgb(255, 210, 210) : Color.White,
                    BorderStyle = BorderStyle.FixedSingle
                }, i, 0);
            }
        }

        private void Form10_Load(object sender, EventArgs e)
        {
            ReloadAndDisplay();
        }

        private void ReloadAndDisplay()
        {
            dayMap = LabCalendarStore.LoadDayMap(out string errorMessage);
            if (!string.IsNullOrWhiteSpace(errorMessage))
                lblStatus.Text = errorMessage;

            DateTime monthStart = new DateTime(currentMonth.Year, currentMonth.Month, 1);
            DateTime monthEnd = monthStart.AddMonths(1).AddDays(-1);
            dutySurnames = LabCalendarStore.LoadDutySurnames(monthStart, monthEnd);

            DisplayCalendar(currentMonth);
        }

        private void DisplayCalendar(DateTime targetMonth)
        {
            currentMonth = targetMonth;
            lblMonth.Text = targetMonth.ToString("yyyy年M月", CultureInfo.GetCultureInfo("ja-JP"));

            tableLayoutPanel1.Controls.Clear();
            tableLayoutPanel1.RowCount = 6;
            tableLayoutPanel1.ColumnCount = 7;

            DateTime firstDay = new DateTime(targetMonth.Year, targetMonth.Month, 1);
            int daysInMonth = DateTime.DaysInMonth(targetMonth.Year, targetMonth.Month);
            int dayOfWeek = (int)firstDay.DayOfWeek;
            DateTime today = DateTime.Today;

            for (int day = 1; day <= daysInMonth; day++)
            {
                DateTime date = new DateTime(targetMonth.Year, targetMonth.Month, day);
                int cellPosition = dayOfWeek + day - 1;
                int row = cellPosition / 7;
                int col = cellPosition % 7;

                LabCalendarDay dayInfo = LabCalendarStore.GetDay(dayMap, date);
                dutySurnames.TryGetValue(date.Date, out string dutySurname);
                var cell = new Panel
                {
                    BorderStyle = BorderStyle.FixedSingle,
                    Tag = date
                };

                LabCalendarStore.BuildCalendarCell(
                    cell,
                    date,
                    dayInfo,
                    date.Date == today,
                    classDayMode,
                    dutySurname);
                WireCellClick(cell);

                tableLayoutPanel1.Controls.Add(cell, col, row);
            }
        }

        private void WireCellClick(Panel cell)
        {
            cell.Click += CalendarCell_Click;
            foreach (Control child in cell.Controls)
                child.Click += CalendarCell_Click;
        }

        private void CalendarCell_Click(object sender, EventArgs e)
        {
            if (!classDayMode)
                return;

            var control = sender as Control;
            var cell = control as Panel ?? control?.Parent as Panel;
            if (cell == null || !(cell.Tag is DateTime))
                return;

            DateTime date = (DateTime)cell.Tag;

            if (!LabCalendarStore.ToggleClassDay(date, out string errorMessage))
            {
                lblStatus.Text = errorMessage ?? "授業日の更新に失敗しました。";
                return;
            }

            ReloadAndDisplay();
            lblStatus.Text = date.ToString("yyyy/MM/dd") + " の授業日を更新しました。";
        }

        private void btnAddMemo_Click(object sender, EventArgs e)
        {
            using (var dialog = new Form())
            {
                dialog.Text = "予定を作る";
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.ClientSize = new Size(360, 150);
                dialog.MaximizeBox = false;
                dialog.MinimizeBox = false;

                var lblDate = new Label { Text = "日付", Location = new Point(16, 18), AutoSize = true };
                var picker = new DateTimePicker
                {
                    Location = new Point(80, 14),
                    Size = new Size(260, 24),
                    Format = DateTimePickerFormat.Short
                };
                var lblMemo = new Label { Text = "予定", Location = new Point(16, 56), AutoSize = true };
                var textMemo = new TextBox
                {
                    Location = new Point(80, 52),
                    Size = new Size(260, 24),
                    MaxLength = LabCalendarStore.MaxMemoLength
                };
                var lblHint = new Label
                {
                    Text = $"最大 {LabCalendarStore.MaxMemoLength} 文字（改行不可）",
                    Location = new Point(80, 78),
                    AutoSize = true,
                    ForeColor = Color.DimGray,
                    Font = UiFonts.Get(8F)
                };
                var btnOk = new Button { Text = "保存", Location = new Point(170, 108), DialogResult = DialogResult.OK };
                var btnCancel = new Button { Text = "取消", Location = new Point(260, 108), DialogResult = DialogResult.Cancel };

                dialog.Controls.AddRange(new Control[]
                {
                    lblDate, picker, lblMemo, textMemo, lblHint, btnOk, btnCancel
                });
                dialog.AcceptButton = btnOk;
                dialog.CancelButton = btnCancel;

                if (dialog.ShowDialog(this) != DialogResult.OK)
                    return;

                if (!LabCalendarStore.SetMemo(picker.Value.Date, textMemo.Text, out string errorMessage))
                {
                    lblStatus.Text = errorMessage ?? "予定の保存に失敗しました。";
                    return;
                }

                ReloadAndDisplay();
                lblStatus.Text = picker.Value.Date.ToString("yyyy/MM/dd") + " の予定を保存しました。";
            }
        }

        private void btnDeleteMemo_Click(object sender, EventArgs e)
        {
            using (var dialog = new Form())
            {
                dialog.Text = "予定を削除";
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.ClientSize = new Size(360, 170);
                dialog.MaximizeBox = false;
                dialog.MinimizeBox = false;

                var lblDate = new Label { Text = "日付", Location = new Point(16, 18), AutoSize = true };
                var picker = new DateTimePicker
                {
                    Location = new Point(80, 14),
                    Size = new Size(260, 24),
                    Format = DateTimePickerFormat.Short
                };
                var lblCurrentTitle = new Label { Text = "現在の予定", Location = new Point(16, 56), AutoSize = true };
                var lblCurrent = new Label
                {
                    Location = new Point(80, 56),
                    Size = new Size(260, 40),
                    BorderStyle = BorderStyle.FixedSingle,
                    ForeColor = Color.DimGray,
                    TextAlign = ContentAlignment.MiddleLeft
                };
                picker.ValueChanged += (s, ev) => UpdateDeleteMemoPreview(lblCurrent, picker.Value.Date);
                UpdateDeleteMemoPreview(lblCurrent, picker.Value.Date);

                var btnOk = new Button { Text = "削除", Location = new Point(170, 120), DialogResult = DialogResult.OK };
                var btnCancel = new Button { Text = "取消", Location = new Point(260, 120), DialogResult = DialogResult.Cancel };

                dialog.Controls.AddRange(new Control[]
                {
                    lblDate, picker, lblCurrentTitle, lblCurrent, btnOk, btnCancel
                });
                dialog.AcceptButton = btnOk;
                dialog.CancelButton = btnCancel;

                if (dialog.ShowDialog(this) != DialogResult.OK)
                    return;

                string memo = LabCalendarStore.GetMemo(picker.Value.Date);
                if (string.IsNullOrWhiteSpace(memo))
                {
                    lblStatus.Text = picker.Value.Date.ToString("yyyy/MM/dd") + " には予定がありません。";
                    return;
                }

                if (!LabCalendarStore.ClearMemo(picker.Value.Date, out string errorMessage))
                {
                    lblStatus.Text = errorMessage ?? "予定の削除に失敗しました。";
                    return;
                }

                ReloadAndDisplay();
                lblStatus.Text = picker.Value.Date.ToString("yyyy/MM/dd") + " の予定を削除しました。";
            }
        }

        private static void UpdateDeleteMemoPreview(Label label, DateTime date)
        {
            string memo = LabCalendarStore.GetMemo(date);
            label.Text = string.IsNullOrWhiteSpace(memo) ? "（予定なし）" : memo;
        }

        private void btnClassDayMode_Click(object sender, EventArgs e)
        {
            classDayMode = !classDayMode;
            btnClassDayMode.BackColor = classDayMode ? Color.FromArgb(255, 255, 180) : SystemColors.Control;
            btnClassDayMode.Text = classDayMode ? "授業日設定中…" : "授業日を決める";
            lblStatus.Text = classDayMode
                ? "授業日にしたい日付をクリックしてください（日曜は不可）。"
                : "授業日設定を終了しました。";
            DisplayCalendar(currentMonth);
        }

        private void buttonPrevMonth_Click(object sender, EventArgs e)
        {
            currentMonth = currentMonth.AddMonths(-1);
            ReloadAndDisplay();
        }

        private void buttonNextMonth_Click(object sender, EventArgs e)
        {
            currentMonth = currentMonth.AddMonths(1);
            ReloadAndDisplay();
        }
    }
}
