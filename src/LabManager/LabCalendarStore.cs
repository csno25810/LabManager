using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace LabManager
{
    public class LabCalendarDay
    {
        public DateTime Date { get; set; }
        public bool IsClassDay { get; set; }
        public int SessionNumber { get; set; }
        public string Memo { get; set; } = "";

        public string SessionSymbol =>
            SessionNumber > 0 ? LabCalendarStore.ToCircledNumber(SessionNumber) : "";
    }

    /// <summary>
    /// 研究室カレンダー DB（授業日・予定メモ）。
    /// 編集 UI: CalendarEditor (Form10) / 表示 UI: TvCalendarPanel (Form14)
    /// </summary>
    public static class LabCalendarStore
    {
        public const int MaxMemoLength = 7;
        public const int MaxCalendarTextLength = 7;
        public const float CalendarTextFontSize = 8F;
        public const int SessionsPerCycle = 13;

        private static readonly Color SundayBackground = Color.FromArgb(255, 210, 210);
        private static readonly Color SaturdayClassBackground = Color.FromArgb(200, 220, 255);
        private static readonly Color TodayBackground = Color.FromArgb(240, 240, 240);

        private static bool schemaReady;
        private static bool schemaAttempted;

        private const string CreateTableSql =
            @"CREATE TABLE IF NOT EXISTS lab_calendar_day (
                calendar_date DATE NOT NULL,
                is_class_day TINYINT(1) NOT NULL DEFAULT 0,
                memo VARCHAR(7) NOT NULL DEFAULT '',
                PRIMARY KEY (calendar_date)
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8";

        public static string ToCircledNumber(int sessionNumber)
        {
            if (sessionNumber < 1 || sessionNumber > SessionsPerCycle)
                return sessionNumber.ToString();
            return char.ConvertFromUtf32(0x2460 + sessionNumber - 1);
        }

        /// <summary>
        /// lab_calendar_day が無ければ自動作成。MessageBox は出さない。
        /// </summary>
        public static bool EnsureSchema(out string errorMessage)
        {
            errorMessage = null;
            if (schemaReady)
                return true;

            if (schemaAttempted && !schemaReady)
            {
                errorMessage = "カレンダー用テーブルの準備に失敗しています。";
                return false;
            }

            schemaAttempted = true;
            if (!Connector.IsConnected)
            {
                errorMessage = "データベースに接続されていません。";
                return false;
            }

            if (!Connector.TryExecuteCommand(CreateTableSql, out errorMessage))
                return false;

            schemaReady = true;
            return true;
        }

        public static Dictionary<DateTime, LabCalendarDay> LoadDayMap(out string errorMessage)
        {
            errorMessage = null;
            var map = new Dictionary<DateTime, LabCalendarDay>();

            if (!EnsureSchema(out errorMessage))
                return map;

            var table = new DataTable();
            if (!Connector.TryTableReader(
                    "SELECT calendar_date, is_class_day, memo FROM lab_calendar_day",
                    table,
                    out errorMessage))
            {
                return map;
            }

            var classDates = new List<DateTime>();
            foreach (DataRow row in table.Rows)
            {
                var date = Convert.ToDateTime(row["calendar_date"]).Date;
                bool isClass = Convert.ToInt32(row["is_class_day"]) != 0;
                string memo = row["memo"]?.ToString() ?? "";

                var day = new LabCalendarDay
                {
                    Date = date,
                    IsClassDay = isClass,
                    Memo = memo
                };
                map[date] = day;
                if (isClass)
                    classDates.Add(date);
            }

            // 曜日ごとに①～⑬（月曜の1回目、火曜の1回目…をそれぞれ独立カウント）
            foreach (var group in classDates.GroupBy(date => date.DayOfWeek))
            {
                var ordered = group.OrderBy(date => date).ToList();
                for (int i = 0; i < ordered.Count; i++)
                    map[ordered[i]].SessionNumber = (i % SessionsPerCycle) + 1;
            }

            return map;
        }

        /// <summary>
        /// 指定期間の日直担当者苗字（duty_schedule + personal_info）
        /// </summary>
        public static Dictionary<DateTime, string> LoadDutySurnames(DateTime rangeStart, DateTime rangeEnd)
        {
            var result = new Dictionary<DateTime, string>();
            if (!Connector.IsConnected)
                return result;

            var dayMap = LoadDayMap(out _);
            var table = new DataTable();
            string sql =
                "SELECT ds.duty_date, pi.name FROM duty_schedule ds " +
                "INNER JOIN personal_info pi ON ds.student_id = pi.student_id " +
                $"WHERE ds.duty_date >= '{rangeStart:yyyy-MM-dd}' AND ds.duty_date <= '{rangeEnd:yyyy-MM-dd}' " +
                "ORDER BY ds.duty_date";
            if (!Connector.TryTableReader(sql, table, out _))
                return result;

            foreach (DataRow row in table.Rows)
            {
                var date = Convert.ToDateTime(row["duty_date"]).Date;
                if (!IsDutyEligibleClassDay(dayMap, date))
                    continue;

                string surname = ExtractSurname(row["name"]?.ToString());
                if (string.IsNullOrEmpty(surname))
                    continue;

                if (result.TryGetValue(date, out string existing))
                    result[date] = FormatCalendarCellText(existing + "・" + surname);
                else
                    result[date] = surname;
            }

            return result;
        }

        public static LabCalendarDay GetDay(Dictionary<DateTime, LabCalendarDay> map, DateTime date)
        {
            var key = date.Date;
            if (map.TryGetValue(key, out LabCalendarDay day))
                return day;
            return new LabCalendarDay { Date = key };
        }

        public static bool IsClassDay(Dictionary<DateTime, LabCalendarDay> map, DateTime date)
        {
            return GetDay(map, date).IsClassDay;
        }

        /// <summary>
        /// 自動日直の対象: 授業日かつ月〜金（土曜は授業日でも日直なし）
        /// </summary>
        public static bool IsDutyEligibleClassDay(Dictionary<DateTime, LabCalendarDay> map, DateTime date)
        {
            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                return false;
            return IsClassDay(map, date);
        }

        public static bool IsDutyDay(DateTime date)
        {
            var map = LoadDayMap(out _);
            return IsDutyEligibleClassDay(map, date);
        }

        public static string FormatCalendarCellText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "";

            text = text.Trim();
            if (text.Length <= MaxCalendarTextLength)
                return text;
            return text.Substring(0, MaxCalendarTextLength);
        }

        /// <summary>カレンダー表示用の苗字（最大7文字）</summary>
        public static string ExtractSurname(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return "";

            fullName = fullName.Trim();
            if (fullName.StartsWith("先生", StringComparison.Ordinal))
                return FormatCalendarCellText(fullName);

            int space = fullName.IndexOfAny(new[] { ' ', '　' });
            if (space > 0)
                return FormatCalendarCellText(fullName.Substring(0, space));

            if (fullName.Length <= 2)
                return fullName;

            return FormatCalendarCellText(fullName.Substring(0, 2));
        }

        public static string GetMemo(DateTime date)
        {
            var map = LoadDayMap(out _);
            return GetDay(map, date).Memo ?? "";
        }

        public static bool ClearMemo(DateTime date, out string errorMessage)
        {
            return SetMemo(date, "", out errorMessage);
        }

        public static bool SetMemo(DateTime date, string memo, out string errorMessage)
        {
            errorMessage = null;
            if (!EnsureSchema(out errorMessage))
                return false;

            memo = NormalizeMemo(memo);
            string dateText = date.ToString("yyyy-MM-dd");

            if (string.IsNullOrEmpty(memo))
            {
                if (!Connector.TryExecuteCommand(
                        $"UPDATE lab_calendar_day SET memo = '' WHERE calendar_date = '{dateText}'",
                        out errorMessage))
                    return false;

                return Connector.TryExecuteCommand(
                    $"DELETE FROM lab_calendar_day WHERE calendar_date = '{dateText}' AND is_class_day = 0 AND memo = ''",
                    out errorMessage);
            }

            string sql = $@"INSERT INTO lab_calendar_day (calendar_date, is_class_day, memo)
                            VALUES ('{dateText}', 0, '{EscapeSql(memo)}')
                            ON DUPLICATE KEY UPDATE memo = '{EscapeSql(memo)}'";
            return Connector.TryExecuteCommand(sql, out errorMessage);
        }

        public static bool ToggleClassDay(DateTime date, out string errorMessage)
        {
            errorMessage = null;
            if (date.DayOfWeek == DayOfWeek.Sunday)
            {
                errorMessage = "日曜日は授業日にできません。";
                return false;
            }

            if (!EnsureSchema(out errorMessage))
                return false;

            string dateText = date.ToString("yyyy-MM-dd");
            var table = new DataTable();
            if (!Connector.TryTableReader(
                    $"SELECT is_class_day FROM lab_calendar_day WHERE calendar_date = '{dateText}'",
                    table,
                    out errorMessage))
            {
                return false;
            }

            bool currentlyClass = table.Rows.Count > 0 && Convert.ToInt32(table.Rows[0]["is_class_day"]) != 0;
            if (currentlyClass)
            {
                if (!Connector.TryExecuteCommand(
                        $"UPDATE lab_calendar_day SET is_class_day = 0 WHERE calendar_date = '{dateText}'",
                        out errorMessage))
                    return false;

                return Connector.TryExecuteCommand(
                    $"DELETE FROM lab_calendar_day WHERE calendar_date = '{dateText}' AND memo = '' AND is_class_day = 0",
                    out errorMessage);
            }

            string sql = $@"INSERT INTO lab_calendar_day (calendar_date, is_class_day, memo)
                            VALUES ('{dateText}', 1, '')
                            ON DUPLICATE KEY UPDATE is_class_day = 1";
            if (!Connector.TryExecuteCommand(sql, out errorMessage))
                return false;

            DutyWeekdayRosterStore.ApplyDutyForClassDate(date, out _);
            return true;
        }

        public static void BuildCalendarCell(
            Panel cell,
            DateTime date,
            LabCalendarDay dayInfo,
            bool isToday,
            bool highlightClickable,
            string dutySurname = null)
        {
            cell.Controls.Clear();
            cell.BackColor = Color.White;
            cell.Cursor = highlightClickable ? Cursors.Hand : Cursors.Default;
            cell.Padding = new Padding(1);

            Color dayNumberColor = Color.Black;
            if (date.DayOfWeek == DayOfWeek.Sunday)
            {
                cell.BackColor = SundayBackground;
            }
            else if (date.DayOfWeek == DayOfWeek.Saturday && dayInfo.IsClassDay)
            {
                cell.BackColor = SaturdayClassBackground;
            }
            else if (isToday)
            {
                cell.BackColor = TodayBackground;
            }

            if (!string.IsNullOrWhiteSpace(dayInfo.Memo))
            {
                cell.Controls.Add(new Label
                {
                    Text = FormatCalendarCellText(dayInfo.Memo),
                    Dock = DockStyle.Bottom,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = UiFonts.Get(CalendarTextFontSize),
                    ForeColor = Color.DimGray,
                    Height = 14
                });
            }

            if (!string.IsNullOrWhiteSpace(dutySurname))
            {
                cell.Controls.Add(new Label
                {
                    Text = FormatCalendarCellText(dutySurname),
                    Dock = DockStyle.Top,
                    Height = 14,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = UiFonts.Get(CalendarTextFontSize),
                    ForeColor = Color.Black
                });
            }

            if (dayInfo.IsClassDay)
            {
                cell.Controls.Add(new Label
                {
                    Text = dayInfo.SessionSymbol,
                    Dock = DockStyle.Top,
                    Height = 16,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = UiFonts.Get(11F, FontStyle.Bold),
                    ForeColor = Color.Black
                });
            }

            cell.Controls.Add(new Label
            {
                Text = date.Day.ToString(),
                Dock = DockStyle.Top,
                Height = 15,
                TextAlign = ContentAlignment.TopLeft,
                Font = UiFonts.Get(9F, isToday ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = dayNumberColor,
                Padding = new Padding(1, 0, 0, 0)
            });
        }

        public static string NormalizeMemo(string memo)
        {
            if (string.IsNullOrWhiteSpace(memo))
                return "";

            memo = memo.Replace("\r", "").Replace("\n", "").Trim();
            if (memo.Length > MaxMemoLength)
                memo = memo.Substring(0, MaxMemoLength);
            return memo;
        }

        private static string EscapeSql(string value)
        {
            return (value ?? "").Replace("\\", "\\\\").Replace("'", "''");
        }
    }
}
