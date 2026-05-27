using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO;
using System.Linq;

namespace MySQL_ConnectionTest
{
    public partial class Form10 : Form
    {
        private Setting mySqlSet;
        private DateTime currentMonth = DateTime.Today;

        // 授業日を DateTime 型で保持
        private List<DateTime> classDays = new List<DateTime>();

        public Form10(Setting settings)
        {
            InitializeComponent();
            this.mySqlSet = settings;

            // 起動時に当月カレンダーを表示
            DisplayCalendar(currentMonth);
        }

        private void Form10_Load(object sender, EventArgs e)
        {
            DisplayCalendar(currentMonth);
        }

        /// <summary>
        /// カレンダーを表示し、Googleカレンダーから授業日を抽出して反映する
        /// </summary>
        private void DisplayCalendar(DateTime targetMonth)
        {
            tableLayoutPanel1.Controls.Clear();
            tableLayoutPanel1.RowCount = 6;
            tableLayoutPanel1.ColumnCount = 7;

            DateTime firstDay = new DateTime(targetMonth.Year, targetMonth.Month, 1);
            int daysInMonth = DateTime.DaysInMonth(targetMonth.Year, targetMonth.Month);
            int dayOfWeek = (int)firstDay.DayOfWeek;

            // Google Calendar (ICS形式) を取得
            string icsUrl = "https://calendar.google.com/calendar/ical/your_calendar_id/basic.ics";
            string icsData = new WebClient().DownloadString(icsUrl);

            // 授業日を抽出（DateTime型のListを返す）
            classDays = ParseIcsForEvents(icsData, targetMonth);

            // カレンダーを構築
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

                // 授業日なら背景を青くする
                if (classDays.Contains(date))
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

        /// <summary>
        /// GoogleカレンダーのICSデータから授業日(DateTime)を抽出する
        /// </summary>
        private List<DateTime> ParseIcsForEvents(string icsData, DateTime targetMonth)
        {
            var dates = new List<DateTime>();
            var matches = Regex.Matches(icsData, @"DTSTART;[^:]*:(\d{8})");

            foreach (Match match in matches)
            {
                string dateStr = match.Groups[1].Value; // 例: 20251015
                if (DateTime.TryParseExact(dateStr, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                {
                    if (date.Year == targetMonth.Year && date.Month == targetMonth.Month)
                    {
                        dates.Add(date);
                    }
                }
            }

            // 同一日付の重複除去
            return dates.Distinct().ToList();
        }

        /// <summary>
        /// 前月へ
        /// </summary>
        private void buttonPrevMonth_Click(object sender, EventArgs e)
        {
            currentMonth = currentMonth.AddMonths(-1);
            DisplayCalendar(currentMonth);
        }

        /// <summary>
        /// 次月へ
        /// </summary>
        private void buttonNextMonth_Click(object sender, EventArgs e)
        {
            currentMonth = currentMonth.AddMonths(1);
            DisplayCalendar(currentMonth);
        }
    }
}
