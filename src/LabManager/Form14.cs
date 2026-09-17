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

        public Form14(Setting settings)
        {
            InitializeComponent();
            Text = "TvCalendarPanel";
        }

        public void ConfigureForTvDisplay()
        {
            int winHeight = Screen.PrimaryScreen.Bounds.Height - 50;
            int winWidth = Screen.PrimaryScreen.Bounds.Width / 2;
            todayAreaHeight = winHeight / 3;
            calendarAreaHeight = winHeight - todayAreaHeight;
            const int monthBarHeight = 36;
            const int weekdayBarHeight = 24;
            const int todayHeaderHeight = 28;

            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            Location = new Point(0, 0);
            Size = new Size(winWidth, winHeight);
            BackColor = Color.White;

            lblMonth.Font = UiFonts.Get(14F, FontStyle.Bold);
            lblMonth.ForeColor = Color.Black;
            lblMonth.Location = new Point(0, 0);
            lblMonth.Size = new Size(winWidth - 80, monthBarHeight);
            lblMonth.TextAlign = ContentAlignment.MiddleCenter;

            btnRefresh = new Button
            {
                Text = "更新",
                Location = new Point(winWidth - 72, 4),
                Size = new Size(64, 28),
                Font = UiFonts.Get(9F, FontStyle.Bold)
            };
            btnRefresh.Click += btnRefresh_Click;

            tableWeekdays.Location = new Point(0, monthBarHeight);
            tableWeekdays.Size = new Size(winWidth, weekdayBarHeight);
            BuildWeekdayHeader();

            tableCalendar.Location = new Point(0, monthBarHeight + weekdayBarHeight);
            tableCalendar.Size = new Size(winWidth, calendarAreaHeight - monthBarHeight - weekdayBarHeight);

            lblTodayHeader.Font = UiFonts.Get(11F, FontStyle.Bold);
            lblTodayHeader.ForeColor = Color.Black;
            lblTodayHeader.BackColor = Color.White;
            lblTodayHeader.BorderStyle = BorderStyle.FixedSingle;
            lblTodayHeader.Location = new Point(0, calendarAreaHeight);
            lblTodayHeader.Size = new Size(winWidth, todayHeaderHeight);

            listViewToday.Font = UiFonts.Get(11F);
            listViewToday.ForeColor = Color.Black;
            listViewToday.BackColor = Color.White;
            listViewToday.BorderStyle = BorderStyle.FixedSingle;
            listViewToday.Location = new Point(0, calendarAreaHeight + todayHeaderHeight);
            listViewToday.Size = new Size(winWidth, todayAreaHeight - todayHeaderHeight);
            listViewToday.Columns[0].Width = 72;
            listViewToday.Columns[1].Width = winWidth - 72 - 4;

            Controls.Add(btnRefresh);
            Controls.Add(lblMonth);
            Controls.Add(tableWeekdays);
            Controls.Add(tableCalendar);
            Controls.Add(lblTodayHeader);
            Controls.Add(listViewToday);
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
