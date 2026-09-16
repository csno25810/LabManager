using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace LabManager
{
    /// <summary>
    /// MENU から開く大学カレンダー。月移動など操作可能。
    /// テレビ左画面の表示専用カレンダーは Form14。
    /// </summary>
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
            DateTime today = DateTime.Today;

            for (int day = 1; day <= daysInMonth; day++)
            {
                DateTime date = new DateTime(targetMonth.Year, targetMonth.Month, day);
                int cellPosition = dayOfWeek + day - 1;
                int row = cellPosition / 7;
                int col = cellPosition % 7;

                var cell = new Panel { BorderStyle = BorderStyle.FixedSingle, BackColor = Color.White };
                var label = new Label
                {
                    Text = day.ToString(),
                    Dock = DockStyle.Top,
                    TextAlign = ContentAlignment.TopLeft,
                    Font = UiFonts.Get(11F, date.Date == today ? FontStyle.Bold : FontStyle.Regular),
                    ForeColor = Color.Black
                };
                cell.Controls.Add(label);

                if (classDays.Contains(date.Date))
                {
                    cell.BackColor = Color.FromArgb(235, 235, 235);
                    var classLabel = new Label
                    {
                        Text = "授業日",
                        ForeColor = Color.Black,
                        Dock = DockStyle.Bottom,
                        Font = UiFonts.Get(9F)
                    };
                    cell.Controls.Add(classLabel);
                }

                tableLayoutPanel1.Controls.Add(cell, col, row);
            }
        }

        private List<DateTime> LoadClassDays(DateTime targetMonth)
        {
            try
            {
                if (cachedIcsData == null)
                    cachedIcsData = CalendarHelper.DownloadIcs(calendarSet);

                return CalendarHelper.GetEventDatesInMonth(cachedIcsData, targetMonth);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Google カレンダーの取得に失敗しました。\n" +
                    "URL・ネットワーク・公開設定を確認してください。\n\n" + ex.Message,
                    "カレンダー取得エラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return new List<DateTime>();
            }
        }

        private void buttonPrevMonth_Click(object sender, EventArgs e)
        {
            cachedIcsData = null;
            DisplayCalendar(currentMonth.AddMonths(-1));
        }

        private void buttonNextMonth_Click(object sender, EventArgs e)
        {
            cachedIcsData = null;
            DisplayCalendar(currentMonth.AddMonths(1));
        }
    }
}
