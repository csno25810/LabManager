using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace LabManager
{
    /// <summary>
    /// TvCalendarPanel（Form14）— テレビPC左半分の表示専用カレンダー。
    /// 編集は CalendarEditor (Form10) で行う。
    /// </summary>
    public partial class Form14 : Form
    {
        private Dictionary<DateTime, LabCalendarDay> dayMap = new Dictionary<DateTime, LabCalendarDay>();
        private Dictionary<DateTime, string> dutySurnames = new Dictionary<DateTime, string>();
        private DateTime displayedMonth = DateTime.Today;
        private int calendarAreaHeight;
        private int todayAreaHeight;
        private Button btnRefresh;
        private bool tvReady;

        public Form14(Setting settings)
        {
            InitializeComponent();
            Text = "TvCalendarPanel";
        }

        public void ConfigureForTvDisplay()
        {
            int winWidth = Screen.PrimaryScreen.Bounds.Width / 2;
            int winHeight = Screen.PrimaryScreen.Bounds.Height - 50;
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            Location = new Point(0, 0);
            Size = new Size(winWidth, winHeight);
            EnsureTvLayout();
            ApplyTvLayout(winWidth, winHeight);
        }

        public void ConfigureAsEmbedded()
        {
            FormBorderStyle = FormBorderStyle.None;
            ControlBox = false;
            ShowInTaskbar = false;
            Dock = DockStyle.Fill;
            EnsureTvLayout();
            Resize -= EmbeddedResize;
            Resize += EmbeddedResize;
        }

        private void EmbeddedResize(object sender, EventArgs e)
        {
            if (ClientSize.Width > 80 && ClientSize.Height > 80)
                ApplyTvLayout(ClientSize.Width, ClientSize.Height);
        }

        private void EnsureTvLayout()
        {
            if (tvReady)
                return;

            BackColor = Color.White;
            lblMonth.Font = UiFonts.Get(14F, FontStyle.Bold);
            lblMonth.ForeColor = Color.Black;
            lblMonth.TextAlign = ContentAlignment.MiddleCenter;

            btnRefresh = new Button
            {
                Text = "更新",
                Size = new Size(64, 28),
                Font = UiFonts.Get(9F, FontStyle.Bold)
            };
            btnRefresh.Click += btnRefresh_Click;

            lblTodayHeader.Font = UiFonts.Get(11F, FontStyle.Bold);
            lblTodayHeader.ForeColor = Color.Black;
            lblTodayHeader.BackColor = Color.White;
            lblTodayHeader.BorderStyle = BorderStyle.FixedSingle;

            tableToday.ColumnCount = 2;
            tableToday.RowCount = 2;
            tableToday.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            tableToday.Margin = new Padding(0);
            tableToday.Padding = new Padding(0);

            lblScheduleTitle.Text = "  本日の予定";
            lblScheduleTitle.Dock = DockStyle.Fill;
            lblScheduleTitle.TextAlign = ContentAlignment.MiddleLeft;
            lblScheduleTitle.Font = UiFonts.Get(10F, FontStyle.Bold);
            lblScheduleTitle.ForeColor = Color.Black;
            lblScheduleTitle.BackColor = Color.White;

            lblQrTitle.Text = "LabPortal";
            lblQrTitle.Dock = DockStyle.Fill;
            lblQrTitle.TextAlign = ContentAlignment.MiddleCenter;
            lblQrTitle.Font = UiFonts.Get(10F, FontStyle.Bold);
            lblQrTitle.ForeColor = Color.Black;
            lblQrTitle.BackColor = Color.White;

            listViewToday.Dock = DockStyle.Fill;
            listViewToday.Font = UiFonts.Get(12F);
            listViewToday.ForeColor = Color.Black;
            listViewToday.BackColor = Color.White;
            listViewToday.BorderStyle = BorderStyle.None;
            listViewToday.HeaderStyle = ColumnHeaderStyle.None;

            panelQr.Dock = DockStyle.Fill;
            panelQr.BackColor = Color.White;
            panelQr.Margin = new Padding(0);
            panelQr.Resize += (sender, e) => LayoutQrPanel();

            picPortalQr.SizeMode = PictureBoxSizeMode.Zoom;
            picPortalQr.BackColor = Color.White;
            picPortalQr.BorderStyle = BorderStyle.None;

            lblPortalQr.Font = UiFonts.Get(8F);
            lblPortalQr.ForeColor = Color.Black;
            lblPortalQr.TextAlign = ContentAlignment.TopCenter;

            if (picPortalQr.Parent != panelQr)
                panelQr.Controls.Add(picPortalQr);
            if (lblPortalQr.Parent != panelQr)
                panelQr.Controls.Add(lblPortalQr);

            tableToday.Controls.Add(lblScheduleTitle, 0, 0);
            tableToday.Controls.Add(lblQrTitle, 1, 0);
            tableToday.Controls.Add(listViewToday, 0, 1);
            tableToday.Controls.Add(panelQr, 1, 1);

            Controls.Add(btnRefresh);
            Controls.Add(lblMonth);
            Controls.Add(tableWeekdays);
            Controls.Add(tableCalendar);
            Controls.Add(lblTodayHeader);
            Controls.Add(tableToday);
            BuildWeekdayHeader();
            tvReady = true;
        }

        private void ApplyTvLayout(int winWidth, int winHeight)
        {
            todayAreaHeight = winHeight / 3;
            calendarAreaHeight = winHeight - todayAreaHeight;
            const int monthBarHeight = 36;
            const int weekdayBarHeight = 24;
            const int todayHeaderHeight = 28;

            lblMonth.Location = new Point(0, 0);
            lblMonth.Size = new Size(winWidth - 80, monthBarHeight);
            btnRefresh.Location = new Point(winWidth - 72, 4);

            tableWeekdays.Location = new Point(0, monthBarHeight);
            tableWeekdays.Size = new Size(winWidth, weekdayBarHeight);

            tableCalendar.Location = new Point(0, monthBarHeight + weekdayBarHeight);
            tableCalendar.Size = new Size(winWidth, calendarAreaHeight - monthBarHeight - weekdayBarHeight);

            lblTodayHeader.Location = new Point(0, calendarAreaHeight);
            lblTodayHeader.Size = new Size(winWidth, todayHeaderHeight);

            tableToday.Location = new Point(0, calendarAreaHeight + todayHeaderHeight);
            tableToday.Size = new Size(winWidth, todayAreaHeight - todayHeaderHeight);
            tableToday.ColumnStyles.Clear();
            tableToday.RowStyles.Clear();
            tableToday.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 64F));
            tableToday.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 36F));
            tableToday.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
            tableToday.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            if (listViewToday.Columns.Count >= 2)
            {
                listViewToday.Columns[0].Width = 88;
                listViewToday.Columns[1].Width = Math.Max(120, (int)(winWidth * 0.64) - 100);
            }

            LayoutQrPanel();
        }

        private void LayoutQrPanel()
        {
            if (panelQr == null || picPortalQr == null || lblPortalQr == null)
                return;

            int pad = 10;
            int labelHeight = 40;
            int side = Math.Min(panelQr.ClientSize.Width, panelQr.ClientSize.Height - labelHeight) - pad * 2;
            if (side < 72)
                side = Math.Max(48, panelQr.ClientSize.Width - pad * 2);

            picPortalQr.Size = new Size(side, side);
            picPortalQr.Location = new Point(Math.Max(0, (panelQr.ClientSize.Width - side) / 2), pad);
            lblPortalQr.Location = new Point(6, picPortalQr.Bottom + 2);
            lblPortalQr.Size = new Size(Math.Max(40, panelQr.ClientSize.Width - 12), labelHeight);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshAll();
        }

        private void BuildWeekdayHeader()
        {
            tableWeekdays.Controls.Clear();
            string[] names = { "日", "月", "火", "水", "木", "金", "土" };
            for (int i = 0; i < 7; i++)
            {
                Color foreColor = Color.Black;
                Color backColor = Color.White;
                if (i == 0)
                {
                    foreColor = Color.DarkRed;
                    backColor = Color.FromArgb(255, 210, 210);
                }
                else if (i == 6)
                {
                    foreColor = Color.DarkBlue;
                }

                var lbl = new Label
                {
                    Text = names[i],
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = UiFonts.Get(10F, FontStyle.Bold),
                    ForeColor = foreColor,
                    BackColor = backColor,
                    BorderStyle = BorderStyle.FixedSingle
                };
                tableWeekdays.Controls.Add(lbl, i, 0);
            }
        }

        private void Form14_Load(object sender, EventArgs e)
        {
            timerRefresh.Interval = 60 * 1000;
            timerRefresh.Enabled = true;
            RefreshAll();
        }

        private void timerRefresh_Tick(object sender, EventArgs e)
        {
            RefreshAll();
        }

        private void RefreshAll()
        {
            dayMap = LabCalendarStore.LoadDayMap(out _);

            DateTime today = DateTime.Today;
            if (displayedMonth.Year != today.Year || displayedMonth.Month != today.Month)
                displayedMonth = today;

            DateTime monthStart = new DateTime(displayedMonth.Year, displayedMonth.Month, 1);
            DateTime monthEnd = monthStart.AddMonths(1).AddDays(-1);
            dutySurnames = LabCalendarStore.LoadDutySurnames(monthStart, monthEnd);

            DisplayMonthCalendar(displayedMonth);
            DisplayTodayInfo(today);
            RefreshPortalQr();
        }

        private void RefreshPortalQr()
        {
            if (picPortalQr == null)
                return;

            string url = PortalLink.FindUrl();
            if (string.IsNullOrEmpty(url))
            {
                lblPortalQr.Text = "接続先がありません";
                if (picPortalQr.Image != null)
                {
                    picPortalQr.Image.Dispose();
                    picPortalQr.Image = null;
                }
                return;
            }

            Bitmap qr = PortalLink.CreateQr(url, 4);
            Image old = picPortalQr.Image;
            picPortalQr.Image = qr;
            if (old != null)
                old.Dispose();
            lblPortalQr.Text = "スマホで開く\r\n" + url;
            LayoutQrPanel();
        }

        private void DisplayMonthCalendar(DateTime targetMonth)
        {
            displayedMonth = targetMonth;
            lblMonth.Text = targetMonth.ToString("yyyy年M月", CultureInfo.GetCultureInfo("ja-JP"));

            tableCalendar.Controls.Clear();
            DateTime today = DateTime.Today;

            DateTime firstDay = new DateTime(targetMonth.Year, targetMonth.Month, 1);
            int daysInMonth = DateTime.DaysInMonth(targetMonth.Year, targetMonth.Month);
            int dayOfWeek = (int)firstDay.DayOfWeek;

            for (int day = 1; day <= daysInMonth; day++)
            {
                DateTime date = new DateTime(targetMonth.Year, targetMonth.Month, day);
                int cellPosition = dayOfWeek + day - 1;
                int row = cellPosition / 7;
                int col = cellPosition % 7;

                LabCalendarDay dayInfo = LabCalendarStore.GetDay(dayMap, date);
                dutySurnames.TryGetValue(date.Date, out string dutySurname);

                var cell = new Panel { BorderStyle = BorderStyle.FixedSingle };
                LabCalendarStore.BuildCalendarCell(
                    cell, date, dayInfo, date.Date == today, false, dutySurname);
                tableCalendar.Controls.Add(cell, col, row);
            }
        }

        private void DisplayTodayInfo(DateTime today)
        {
            LabCalendarDay dayInfo = LabCalendarStore.GetDay(dayMap, today);
            string header = "  本日（" + today.ToString("yyyy/MM/dd(ddd)", CultureInfo.GetCultureInfo("ja-JP")) + "）";
            if (dayInfo.IsClassDay)
                header += "  " + dayInfo.SessionSymbol;
            lblTodayHeader.Text = header;

            listViewToday.Items.Clear();
            if (dutySurnames.TryGetValue(today.Date, out string duty) && !string.IsNullOrWhiteSpace(duty))
            {
                listViewToday.Items.Add(new ListViewItem(new[] { "日直", duty }));
            }
            if (!string.IsNullOrWhiteSpace(dayInfo.Memo))
            {
                listViewToday.Items.Add(new ListViewItem(new[] { "予定", dayInfo.Memo }));
            }
            else if (dayInfo.IsClassDay)
            {
                listViewToday.Items.Add(new ListViewItem(new[] { "授業", dayInfo.SessionSymbol }));
            }
            else if (listViewToday.Items.Count == 0)
            {
                listViewToday.Items.Add(new ListViewItem(new[] { "-", "予定なし" }));
            }
        }
    }
}
