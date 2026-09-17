using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LabManager
{
    public class StudentListItem
    {
        public string StudentId { get; set; } = "";
        public string Name { get; set; } = "";

        public string Display =>
            string.IsNullOrEmpty(StudentId) ? "(なし)" : StudentId + " " + Name;
    }

    /// <summary>
    /// 曜日別日直担当（Form5）と授業日追加時の自動日直登録。
    /// </summary>
    public static class DutyWeekdayRosterStore
    {
        private const string CreateTableSql =
            @"CREATE TABLE IF NOT EXISTS duty_weekday_roster (
                weekday TINYINT NOT NULL,
                slot TINYINT NOT NULL,
                student_id VARCHAR(20) NOT NULL DEFAULT '',
                PRIMARY KEY (weekday, slot)
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8";

        private static bool schemaReady;
        private static bool schemaAttempted;

        public static bool EnsureSchema(out string errorMessage)
        {
            errorMessage = null;
            if (schemaReady)
                return true;

            if (schemaAttempted && !schemaReady)
            {
                errorMessage = "曜日担当テーブルの準備に失敗しています。";
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

        public static List<StudentListItem> LoadAllStudents(out string errorMessage)
        {
            errorMessage = null;
            var list = new List<StudentListItem> { new StudentListItem() };

            if (!Connector.IsConnected)
            {
                errorMessage = "データベースに接続されていません。";
                return list;
            }

            var table = new DataTable();
            if (!Connector.TryTableReader(
                    $"SELECT student_id, name FROM personal_info WHERE {PersonalInfoHelper.SqlStudentsOnly} ORDER BY student_id",
                    table,
                    out errorMessage))
            {
                return list;
            }

            foreach (DataRow row in table.Rows)
            {
                list.Add(new StudentListItem
                {
                    StudentId = row["student_id"]?.ToString()?.Trim(),
                    Name = row["name"]?.ToString()?.Trim()
                });
            }

            return list;
        }

        public static Dictionary<DayOfWeek, List<string>> LoadRoster(out string errorMessage)
        {
            errorMessage = null;
            var roster = new Dictionary<DayOfWeek, List<string>>();
            foreach (DayOfWeek dow in new[] {
                DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday,
                DayOfWeek.Thursday, DayOfWeek.Friday })
            {
                roster[dow] = new List<string>();
            }

            if (!EnsureSchema(out errorMessage))
                return roster;

            var table = new DataTable();
            if (!Connector.TryTableReader(
                    "SELECT weekday, slot, student_id FROM duty_weekday_roster ORDER BY weekday, slot",
                    table,
                    out errorMessage))
            {
                return roster;
            }

            foreach (DataRow row in table.Rows)
            {
                int weekday = Convert.ToInt32(row["weekday"]);
                string studentId = row["student_id"]?.ToString()?.Trim();
                if (string.IsNullOrEmpty(studentId))
                    continue;

                var dow = (DayOfWeek)weekday;
                if (!roster.ContainsKey(dow))
                    roster[dow] = new List<string>();

                if (!roster[dow].Contains(studentId))
                    roster[dow].Add(studentId);
            }

            return roster;
        }

        public static bool SaveRoster(Dictionary<DayOfWeek, List<string>> roster, out string errorMessage)
        {
            errorMessage = null;
            if (!EnsureSchema(out errorMessage))
                return false;

            if (!Connector.TryExecuteCommand("DELETE FROM duty_weekday_roster", out errorMessage))
                return false;

            foreach (var pair in roster)
            {
                int weekday = (int)pair.Key;
                var ids = pair.Value.Where(id => !string.IsNullOrWhiteSpace(id)).Take(2).ToList();
                for (int slot = 1; slot <= 2; slot++)
                {
                    string studentId = slot <= ids.Count ? ids[slot - 1] : "";
                    string sql = $@"INSERT INTO duty_weekday_roster (weekday, slot, student_id)
                                    VALUES ({weekday}, {slot}, '{EscapeSql(studentId)}')";
                    if (!Connector.TryExecuteCommand(sql, out errorMessage))
                        return false;
                }
            }

            return true;
        }

        public static string FormatRosterSummary(
            Dictionary<DayOfWeek, List<string>> roster,
            IList<StudentListItem> students)
        {
            var nameById = students
                .Where(s => !string.IsNullOrEmpty(s.StudentId))
                .ToDictionary(s => s.StudentId, s => s.Name);

            string[] dayNames = { "月", "火", "水", "木", "金" };
            DayOfWeek[] days = {
                DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday,
                DayOfWeek.Thursday, DayOfWeek.Friday
            };

            var lines = new List<string>();
            for (int i = 0; i < days.Length; i++)
            {
                if (!roster.TryGetValue(days[i], out List<string> ids) || ids.Count == 0)
                {
                    lines.Add(dayNames[i] + ": （未設定）");
                    continue;
                }

                var parts = ids.Select(id =>
                {
                    nameById.TryGetValue(id, out string name);
                    return string.IsNullOrEmpty(name) ? id : id + " " + name;
                });
                lines.Add(dayNames[i] + ": " + string.Join(" / ", parts));
            }

            return string.Join("\r\n", lines);
        }

        /// <summary>
        /// 授業日が追加されたとき、その曜日の担当者を duty_schedule に登録する。
        /// </summary>
        public static int ApplyDutyForClassDate(DateTime date, out string errorMessage)
        {
            errorMessage = null;
            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                return 0;

            var roster = LoadRoster(out errorMessage);
            if (!string.IsNullOrEmpty(errorMessage))
                return 0;

            if (!roster.TryGetValue(date.DayOfWeek, out List<string> studentIds) || studentIds.Count == 0)
                return 0;

            int inserted = 0;
            foreach (string studentId in studentIds)
            {
                if (DutyScheduleStore.TryInsertIfMissing(studentId, date, out _))
                    inserted++;
            }

            return inserted;
        }

        private static string EscapeSql(string value)
        {
            return (value ?? "").Replace("\\", "\\\\").Replace("'", "''");
        }
    }

    public static class DutyScheduleStore
    {
        public static bool TryInsertIfMissing(string studentId, DateTime dutyDate, out string errorMessage)
        {
            errorMessage = null;
            if (string.IsNullOrWhiteSpace(studentId) || !Connector.IsConnected)
                return false;

            string dateText = dutyDate.ToString("yyyy-MM-dd");
            var table = new DataTable();
            if (!Connector.TryTableReader(
                    $"SELECT 1 FROM duty_schedule WHERE duty_date = '{dateText}' AND student_id = '{EscapeSql(studentId)}' LIMIT 1",
                    table,
                    out errorMessage))
                return false;

            if (table.Rows.Count > 0)
                return false;

            string sql = $@"INSERT INTO duty_schedule (duty_date, student_id, duty_status, penalty_count, duty_type)
                            VALUES ('{dateText}', '{EscapeSql(studentId)}', 0, 0, '0')";
            if (!Connector.TryExecuteCommand(sql, out errorMessage))
                return false;

            return true;
        }

        private static string EscapeSql(string value)
        {
            return (value ?? "").Replace("\\", "\\\\").Replace("'", "''");
        }
    }
}
