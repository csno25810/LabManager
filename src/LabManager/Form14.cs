using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace LabManager
{
    /// <summary>
    /// テレビPC左半分：表示専用カレンダー（当月）＋本日の予定。
    /// 操作は不可。月移動・詳細確認は MENU の大学カレンダー（Form10）を使う。
    /// </summary>
    public partial class Form14 : Form
    {
        private readonly CalendarSetting calendarSet = new CalendarSetting();
        private string cachedIcsData;
        private DateTime displayedMonth = DateTime.Today;
        private int calendarAreaHeight;
        private int todayAreaHeight;

        public Form14(Setting settings)
        {
            InitializeComponent();
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
            lblMonth.Size = new Size(winWidth, monthBarHeight);

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

            Controls.Add(lblMonth);
            Controls.Add(tableWeekdays);
            Controls.Add(tableCalendar);
            Controls.Add(lblTodayHeader);
            Controls.Add(listViewToday);
        }

        private void BuildWeekdayHeader()
        {
            tableWeekdays.Controls.Clear();
            string[] names = { "日", "月", "火", "水", "木", "金", "土" };
            for (int i = 0; i < 7; i++)
            {
                var lbl = new Label
                {
                    Text = names[i],
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = UiFonts.Get(10F, FontStyle.Bold),
                    ForeColor = Color.Black,
                    BackColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle
                };
                tableWeekdays.Controls.Add(lbl, i, 0);
            }
        }

        private void Form14_Load(object sender, EventArgs e)
        {
            if (!calendarSet.ReadSetting())
            {
                lblMonth.Text = "カレンダー未設定";
                lblTodayHeader.Text = "  本日の予定（設定なし）";
                return;
            }

            timerRefresh.Interval = Math.Max(calendarSet.ReloadTime, 30) * 1000;
            timerRefresh.Enabled = true;
            RefreshAll();
        }

        private void timerRefresh_Tick(object sender, EventArgs e)
        {
            RefreshAll();
        }

        private void RefreshAll()
        {
            if (!calendarSet.ReadSetting())
                return;

            try
            {
                cachedIcsData = CalendarHelper.DownloadIcs(calendarSet);
            }
            catch
            {
                lblMonth.Text = DateTime.Today.ToString("yyyy年M月", CultureInfo.GetCultureInfo("ja-JP")) + "（取得失敗）";
                lblTodayHeader.Text = "  本日の予定（取得失敗）";
                listViewToday.Items.Clear();
                return;
            }

            DateTime today = DateTime.Today;
            if (displayedMonth.Year != today.Year || displayedMonth.Month != today.Month)
                displayedMonth = today;

            DisplayMonthCalendar(displayedMonth);
            DisplayTodayEvents(today);
        }

        private void DisplayMonthCalendar(DateTime targetMonth)
        {
            displayedMonth = targetMonth;
            lblMonth.Text = targetMonth.ToString("yyyy年M月", CultureInfo.GetCultureInfo("ja-JP"));

            tableCalendar.Controls.Clear();
            var classDays = CalendarHelper.GetEventDatesInMonth(cachedIcsData, targetMonth);
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

                bool isToday = date.Date == today;
                bool isClassDay = classDays.Contains(date.Date);

                var cell = new Panel
                {
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = Color.White
                };

                if (isToday)
                    cell.BackColor = Color.FromArgb(240, 240, 240);

                var label = new Label
                {
                    Text = day.ToString(),
                    Dock = DockStyle.Top,
                    TextAlign = ContentAlignment.TopLeft,
                    Font = UiFonts.Get(11F, isToday ? FontStyle.Bold : FontStyle.Regular),
                    ForeColor = Color.Black,
                    Padding = new Padding(2, 2, 0, 0)
                };
                cell.Controls.Add(label);

                if (isClassDay)
                {
                    var mark = new Label
                    {
                        Text = "授業",
                        Dock = DockStyle.Bottom,
                        TextAlign = ContentAlignment.BottomCenter,
                        Font = UiFonts.Get(9F),
                        ForeColor = Color.Black
                    };
                    cell.Controls.Add(mark);
                }

                tableCalendar.Controls.Add(cell, col, row);
            }
        }

        private void DisplayTodayEvents(DateTime today)
        {
            lblTodayHeader.Text = "  本日の予定（" + today.ToString("yyyy/MM/dd(ddd)", CultureInfo.GetCultureInfo("ja-JP")) + "）";

            listViewToday.Items.Clear();
            var events = CalendarHelper.GetEventsOnDate(cachedIcsData, today);

            if (events.Count == 0)
            {
                listViewToday.Items.Add(new ListViewItem(new[] { "-", "予定なし" }));
                return;
            }

            foreach (var evt in events)
            {
                string timeText = evt.Start.TimeOfDay.TotalMinutes > 0
                    ? evt.Start.ToString("HH:mm")
                    : "終日";
                listViewToday.Items.Add(new ListViewItem(new[] { timeText, evt.Summary }));
            }
        }
    }
}
