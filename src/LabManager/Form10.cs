using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using Ical.Net.DataTypes;
using IcalCalendar = Ical.Net.Calendar;

namespace LabManager
{
    public partial class Form10 : Form
    {
        private readonly CalendarSetting calendarSet = new CalendarSetting();
        private DateTime currentMonth = DateTime.Today;
        private string cachedIcsData;
        private List<DateTime> classDays = new List<DateTime>();

        public Form10(Setting settings)
        {
            InitializeComponent();
        }

        /// <summary>
        /// 旧 GoogleCalenderReader.exe 相当：画面左半分にフル表示する。
        /// </summary>
        public void ConfigureForTvDisplay()
        {
            int winHeight = Screen.PrimaryScreen.Bounds.Height - 50;
            int winWidth = Screen.PrimaryScreen.Bounds.Width / 2;
            const int headerHeight = 40;

            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            Location = new Point(0, 0);
            Size = new Size(winWidth, winHeight);

            btnPrevMonth.Location = new Point(8, 6);
            btnPrevMonth.Size = new Size(80, headerHeight - 12);
            lblMonth.Location = new Point(96, 6);
            lblMonth.Size = new Size(winWidth - 192, headerHeight - 12);
            btnNextMonth.Location = new Point(winWidth - 88, 6);
            btnNextMonth.Size = new Size(80, headerHeight - 12);

            tableLayoutPanel1.Location = new Point(0, headerHeight);
            tableLayoutPanel1.Size = new Size(winWidth, winHeight - headerHeight);
        }

        private void Form10_Load(object sender, EventArgs e)
        {
            if (!calendarSet.ReadSetting())
            {
                MessageBox.Show(
                    "Google カレンダー設定が見つかりません。\n\n" +
                    calendarSet.ConfigPath + " を配置してください。\n" +
                    "（研究室PCの GoogleCalenderReader.ini をコピーすれば動きます）",
                    "カレンダー設定",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }

            DisplayCalendar(currentMonth);
        }

        private void DisplayCalendar(DateTime targetMonth)
        {
            currentMonth = targetMonth;
            lblMonth.Text = targetMonth.ToString("yyyy年M月", CultureInfo.GetCultureInfo("ja-JP"));

            tableLayoutPanel1.Controls.Clear();
            tableLayoutPanel1.RowCount = 6;
            tableLayoutPanel1.ColumnCount = 7;

            classDays = LoadClassDays(targetMonth);

            DateTime firstDay = new DateTime(targetMonth.Year, targetMonth.Month, 1);
            int daysInMonth = DateTime.DaysInMonth(targetMonth.Year, targetMonth.Month);
            int dayOfWeek = (int)firstDay.DayOfWeek;

            for (int day = 1; day <= daysInMonth; day++)
            {
                DateTime date = new DateTime(targetMonth.Year, targetMonth.Month, day);
                int cellPosition = dayOfWeek + day - 1;
                int row = cellPosition / 7;
                int col = cellPosition % 7;

                var cell = new Panel { BorderStyle = BorderStyle.FixedSingle };
                var label = new Label
                {
                    Text = day.ToString(),
                    Dock = DockStyle.Top,
                    TextAlign = ContentAlignment.TopLeft
                };
                cell.Controls.Add(label);

                if (classDays.Contains(date.Date))
                {
                    cell.BackColor = Color.LightBlue;
                    var classLabel = new Label
                    {
                        Text = "授業日",
                        ForeColor = Color.DarkBlue,
                        Dock = DockStyle.Bottom
                    };
                    cell.Controls.Add(classLabel);
                }

                tableLayoutPanel1.Controls.Add(cell, col, row);
            }
        }

        private List<DateTime> LoadClassDays(DateTime targetMonth)
        {
            var dates = new List<DateTime>();
            string icsUrl = calendarSet.GetIcsUrl();
            if (string.IsNullOrWhiteSpace(icsUrl))
                return dates;

            try
            {
                if (cachedIcsData == null)
                    cachedIcsData = DownloadIcs(icsUrl);

                dates = ParseIcsForEvents(cachedIcsData, targetMonth);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Google カレンダーの取得に失敗しました。\n" +
                    "URL・ネットワーク・公開設定を確認してください。\n\n" + ex.Message,
                    "カレンダー取得エラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }

            return dates;
        }

        private static string DownloadIcs(string icsUrl)
        {
            using (var client = new WebClient { Encoding = Encoding.UTF8 })
            {
                return client.DownloadString(icsUrl);
            }
        }

        /// <summary>
        /// ICS から対象月のイベント日付を抽出する（Ical.Net 使用）
        /// </summary>
        private static List<DateTime> ParseIcsForEvents(string icsData, DateTime targetMonth)
        {
            var dates = new List<DateTime>();
            if (string.IsNullOrWhiteSpace(icsData))
                return dates;

            var calendar = IcalCalendar.Load(icsData);
            var monthStart = new DateTime(targetMonth.Year, targetMonth.Month, 1);
            var monthEnd = monthStart.AddMonths(1);
            var searchStart = new CalDateTime(monthStart);

            foreach (var evt in calendar.Events)
            {
                foreach (var occurrence in evt.GetOccurrences(searchStart))
                {
                    var start = occurrence.Period.StartTime;
                    if (start == null)
                        continue;

                    var date = start.Value.Date;
                    if (date >= monthEnd)
                        break;
                    if (date >= monthStart)
                        dates.Add(date);
                }
            }

            return dates.Distinct().ToList();
        }

        private void buttonPrevMonth_Click(object sender, EventArgs e)
        {
            DisplayCalendar(currentMonth.AddMonths(-1));
        }

        private void buttonNextMonth_Click(object sender, EventArgs e)
        {
            DisplayCalendar(currentMonth.AddMonths(1));
        }
    }
}
