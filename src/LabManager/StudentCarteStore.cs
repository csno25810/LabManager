using System;
using System.Collections.Generic;
using System.Data;

namespace LabManager
{
    /// <summary>
    /// 個人カルテの1項目。行が無くても SourceTables に参照先を残す。
    /// </summary>
    public sealed class CarteSection
    {
        public string Title { get; set; }
        public string SourceTables { get; set; }
        public string Detail { get; set; }
        public string Error { get; set; }
        public DataTable Rows { get; set; }

        public int Count
        {
            get { return Rows == null ? 0 : Rows.Rows.Count; }
        }
    }

    public sealed class StudentCarte
    {
        public string StudentId { get; set; }
        public string Name { get; set; }
        public CarteSection Attendance { get; set; }
        public CarteSection Duty { get; set; }
        public CarteSection Progress { get; set; }
        public CarteSection Literature { get; set; }
    }

    /// <summary>
    /// 学籍番号1人分のカルテ。研究室全体の一覧は作らない。
    /// カレンダー（lab_calendar_day）と日誌（diary_log）は読まない。
    /// </summary>
    public static class StudentCarteStore
    {
        /// <summary>出席。touch_log を chip_list.student_id でその人だけに絞る。</summary>
        public const string AttendanceSources = "touch_log, chip_list";

        /// <summary>日直。duty_schedule.student_id。</summary>
        public const string DutySources = "duty_schedule";

        /// <summary>
        /// 進捗。progress_log を見る。
        /// diary_log は日誌、lab_calendar_day はカレンダーなので、ここからは参照しない。
        /// </summary>
        public const string ProgressTable = "progress_log";
        public const string ProgressSources = "progress_log";

        /// <summary>
        /// 文献。references_list（Form8）。
        /// student_id 列があるときだけその人の行を読む。列が無いときは全体を出さない。
        /// </summary>
        public const string LiteratureTable = "references_list";
        public const string LiteratureSources = "references_list";

        private static readonly string[] DemoStudentIds =
        {
            "0001", "0002", "0003", "0004", "0005", "0006", "M0007", "5810"
        };

        public static StudentCarte Load(string studentId)
        {
            EnsureSchema();
            SeedDemoIfEmpty();

            string id = (studentId ?? "").Trim();
            var carte = new StudentCarte
            {
                StudentId = id,
                Name = LookupName(id),
                Attendance = LoadAttendance(id),
                Duty = LoadDuty(id),
                Progress = LoadProgress(id),
                Literature = LoadLiterature(id)
            };
            return carte;
        }

        /// <summary>
        /// 進捗表を作り、文献に学籍番号列が無ければ足す。既存の文献登録画面は壊さない。
        /// </summary>
        public static void EnsureSchema()
        {
            Connector.TryExecuteCommand(
                @"CREATE TABLE IF NOT EXISTS progress_log (
                    id INT NOT NULL AUTO_INCREMENT,
                    student_id VARCHAR(20) NOT NULL,
                    progress_date DATE NOT NULL,
                    title VARCHAR(255) NOT NULL,
                    content TEXT NOT NULL,
                    PRIMARY KEY (id),
                    KEY idx_progress_student (student_id)
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4",
                out _);

            List<string> columns = ListColumns(LiteratureTable, out string error);
            if (columns == null || !string.IsNullOrEmpty(error))
                return;
            if (ContainsColumn(columns, "student_id"))
                return;

            Connector.TryExecuteCommand(
                "ALTER TABLE references_list ADD COLUMN student_id VARCHAR(20) NOT NULL DEFAULT '' AFTER id",
                out _);
        }

        /// <summary>
        /// 開発用の仮データ。seed 学生に進捗・文献が無いときだけ入れる。
        /// 進捗＝研究の途中経過。文献＝その人が読んだ論文・資料。日誌・カレンダーは使わない。
        /// </summary>
        public static void SeedDemoIfEmpty()
        {
            foreach (string studentId in DemoStudentIds)
            {
                if (!PersonExists(studentId))
                    continue;
                if (CountRows(ProgressTable, studentId) == 0)
                    InsertDemoProgress(studentId);
                if (CountRows(LiteratureTable, studentId) == 0)
                    InsertDemoLiterature(studentId);
            }
        }

        private static bool PersonExists(string studentId)
        {
            var table = new DataTable();
            if (!Connector.TryTableReader(
                    $"SELECT 1 FROM personal_info WHERE student_id = '{Escape(studentId)}' LIMIT 1",
                    table,
                    out _))
                return false;
            return table.Rows.Count > 0;
        }

        private static int CountRows(string tableName, string studentId)
        {
            var table = new DataTable();
            if (!Connector.TryTableReader(
                    $"SELECT COUNT(*) AS n FROM `{tableName}` WHERE student_id = '{Escape(studentId)}'",
                    table,
                    out _))
                return -1;
            if (table.Rows.Count == 0)
                return 0;
            return ToInt(table.Rows[0]["n"]);
        }

        private static void InsertDemoProgress(string studentId)
        {
            string[] titles;
            string[] contents;
            DemoProgressFor(studentId, out titles, out contents);
            string[] dates = { "2026-10-08", "2026-10-15", "2026-10-22" };
            for (int i = 0; i < titles.Length; i++)
            {
                Connector.TryExecuteCommand(
                    $@"INSERT INTO progress_log (student_id, progress_date, title, content)
                       VALUES ('{Escape(studentId)}', '{dates[i]}', '{Escape(titles[i])}', '{Escape(contents[i])}')",
                    out _);
            }
        }

        private static void InsertDemoLiterature(string studentId)
        {
            string title1;
            string author1;
            string title2;
            string author2;
            DemoLiteratureFor(studentId, out title1, out author1, out title2, out author2);
            Connector.TryExecuteCommand(
                $@"INSERT INTO references_list
                    (student_id, title, author, thesis_type, year, notes, url, site_name, open_date, reference_type, pages, genre)
                   VALUES
                    ('{Escape(studentId)}', '{Escape(title1)}', '{Escape(author1)}', '論文', '2024',
                     '個人カルテ用の仮データ', '', '', '2026-10-10', '', '1-12', '画像')",
                out _);
            Connector.TryExecuteCommand(
                $@"INSERT INTO references_list
                    (student_id, title, author, thesis_type, year, notes, url, site_name, open_date, reference_type, pages, genre)
                   VALUES
                    ('{Escape(studentId)}', '{Escape(title2)}', '{Escape(author2)}', '紀要', '2023',
                     '個人カルテ用の仮データ', '', '', '2026-10-18', '', '45-52', 'WEB')",
                out _);
        }

        private static void DemoProgressFor(string studentId, out string[] titles, out string[] contents)
        {
            titles = new[] { "テーマの整理（仮）", "関連研究（仮）", "今週の作業（仮）" };
            contents = new[]
            {
                "研究の目的と、今学期に進める範囲を書き出した。",
                "近いテーマの論文を読み、使えそうな点をメモした。",
                "実験または実装の途中経過を残した。"
            };
            if (studentId == "0001")
            {
                titles = new[] { "在席判定の検討（仮）", "関連研究の整理（仮）", "実装の確認（仮）" };
                contents = new[]
                {
                    "タッチ回数の奇数偶数で在席を出せるか検討した。",
                    "入退室管理の文献を2件読んだ。",
                    "カルテに出す出席の集計を確認した。"
                };
            }
            else if (studentId == "0002")
            {
                titles = new[] { "日直運用の確認（仮）", "罰直の扱い（仮）", "画面の見やすさ（仮）" };
                contents = new[]
                {
                    "曜日別日直が授業日だけ入るか確認した。",
                    "遅刻と罰直の記録の残り方を見た。",
                    "本日までの日直だけ出す案を試した。"
                };
            }
            else if (studentId == "5810")
            {
                titles = new[] { "まとめ画面の設計（仮）", "在席と日直の束ね（仮）", "個人カルテの試作（仮）" };
                contents = new[]
                {
                    "在席・日直・日誌を1画面に置く配置を検討した。",
                    "学籍番号を共通キーに、機能ごとの記録を一人分へ束ねる方針にした。",
                    "出席・日直・進捗・文献を個人画面に出す試作を進めた。"
                };
            }
        }

        private static void DemoLiteratureFor(
            string studentId,
            out string title1, out string author1,
            out string title2, out string author2)
        {
            title1 = "研究室における入退室管理の一手法（仮）";
            author1 = "佐藤 一郎";
            title2 = "学生カルテを用いた研究進捗の可視化（仮）";
            author2 = "鈴木 花子";
            if (studentId == "0002")
            {
                title1 = "RFID を用いた出席記録（仮）";
                author1 = "高橋 健";
                title2 = "大学研究室の運営支援システム（仮）";
                author2 = "伊藤 美咲";
            }
            else if (studentId == "5810")
            {
                title1 = "学生カルテによる研究室活動の可視化（仮）";
                author1 = "山田 太郎";
                title2 = "学籍番号をキーとした学習記録の統合（仮）";
                author2 = "中村 直子";
            }
        }

        private static string LookupName(string studentId)
        {
            if (studentId.Length == 0)
                return "";

            var table = new DataTable();
            if (!Connector.TryTableReader(
                    $"SELECT name FROM personal_info WHERE student_id = '{Escape(studentId)}' LIMIT 1",
                    table,
                    out _))
                return "";
            if (table.Rows.Count == 0)
                return "";
            return Convert.ToString(table.Rows[0]["name"]) ?? "";
        }

        private static CarteSection LoadAttendance(string studentId)
        {
            var section = NewSection("出席", AttendanceSources);
            if (studentId.Length == 0)
            {
                section.Detail = "学籍番号が空です。";
                return section;
            }

            string sql = $@"
                SELECT
                    DATE_FORMAT(tl.time_stamp, '%Y-%m-%d') AS touch_day,
                    DATE_FORMAT(MIN(tl.time_stamp), '%H:%i') AS first_touch,
                    DATE_FORMAT(MAX(tl.time_stamp), '%H:%i') AS last_touch,
                    COUNT(*) AS touch_count
                FROM chip_list cl
                INNER JOIN touch_log tl ON tl.chip_id = cl.chip_id
                WHERE cl.student_id = '{Escape(studentId)}'
                GROUP BY DATE_FORMAT(tl.time_stamp, '%Y-%m-%d')
                ORDER BY touch_day DESC
                LIMIT 100";

            var raw = new DataTable();
            if (!Connector.TryTableReader(sql, raw, out string error))
            {
                section.Error = error;
                return section;
            }

            var view = new DataTable();
            view.Columns.Add("日付");
            view.Columns.Add("初回");
            view.Columns.Add("最終");
            view.Columns.Add("回数");
            view.Columns.Add("状態");
            string today = DateTime.Today.ToString("yyyy-MM-dd");
            foreach (DataRow row in raw.Rows)
            {
                string day = Convert.ToString(row["touch_day"]) ?? "";
                int count = ToInt(row["touch_count"]);
                string state = day == today
                    ? AttendanceRules.PresenceLabel(count)
                    : "来室";
                view.Rows.Add(day, row["first_touch"], row["last_touch"], count.ToString(), state);
            }

            section.Rows = view;
            if (view.Rows.Count == 0)
                section.Detail = "この学籍番号のタッチはありません。";
            return section;
        }

        private static CarteSection LoadDuty(string studentId)
        {
            var section = NewSection("日直", DutySources);
            if (studentId.Length == 0)
            {
                section.Detail = "学籍番号が空です。";
                return section;
            }

            string today = DateTime.Today.ToString("yyyy-MM-dd");
            string sql = $@"
                SELECT
                    DATE_FORMAT(ds.duty_date, '%Y-%m-%d') AS duty_day,
                    ds.duty_type,
                    ds.duty_status
                FROM duty_schedule ds
                WHERE ds.student_id = '{Escape(studentId)}'
                  AND ds.duty_date <= '{today}'
                ORDER BY ds.duty_date DESC
                LIMIT 100";

            var raw = new DataTable();
            if (!Connector.TryTableReader(sql, raw, out string error))
            {
                section.Error = error;
                return section;
            }

            var view = new DataTable();
            view.Columns.Add("日付");
            view.Columns.Add("種別");
            view.Columns.Add("出席状況");
            foreach (DataRow row in raw.Rows)
            {
                string type = Convert.ToString(row["duty_type"]) ?? "0";
                view.Rows.Add(
                    Convert.ToString(row["duty_day"]),
                    type == "1" ? "罰直" : "通常",
                    AttendanceRules.DutyStatusLabel(ToInt(row["duty_status"])));
            }

            section.Rows = view;
            if (view.Rows.Count == 0)
                section.Detail = "この学籍番号の日直はありません。";
            return section;
        }

        /// <summary>
        /// 進捗は progress_log だけを見る。テーブルが無いときは未登録。
        /// diary_log は日誌なので SELECT しない。
        /// </summary>
        private static CarteSection LoadProgress(string studentId)
        {
            var section = NewSection("進捗", ProgressSources);
            if (studentId.Length == 0)
            {
                section.Detail = "学籍番号が空です。";
                return section;
            }

            if (!TableExists(ProgressTable, out string error))
            {
                if (!string.IsNullOrEmpty(error))
                    section.Error = error;
                else
                    section.Detail = "progress_log は未作成です。diary_log（日誌）はカルテに使いません。";
                return section;
            }

            List<string> columns = ListColumns(ProgressTable, out error);
            if (columns == null)
            {
                section.Error = error;
                return section;
            }

            if (!ContainsColumn(columns, "student_id"))
            {
                section.Detail = "progress_log に student_id 列がないため、この人の行を特定できません。";
                return section;
            }

            string selectList = ProgressSelectList(columns);
            if (selectList == null)
            {
                section.Detail = "progress_log に表示できる列（title / content / note / status など）がありません。";
                return section;
            }

            string orderBy = ContainsColumn(columns, "progress_date")
                ? "ORDER BY progress_date DESC"
                : "";
            string sql = $@"
                SELECT {selectList}
                FROM `{ProgressTable}`
                WHERE student_id = '{Escape(studentId)}'
                {orderBy}
                LIMIT 100";
            var table = new DataTable();
            if (!Connector.TryTableReader(sql, table, out error))
            {
                section.Error = error;
                return section;
            }

            section.Rows = table;
            if (table.Rows.Count == 0)
                section.Detail = "この学籍番号の進捗はありません。";
            return section;
        }

        /// <summary>
        /// 文献は references_list だけを見る。
        /// student_id 列が無い既存スキーマでは、全件を個人の記録として出さない。
        /// </summary>
        private static CarteSection LoadLiterature(string studentId)
        {
            var section = NewSection("文献", LiteratureSources);
            if (studentId.Length == 0)
            {
                section.Detail = "学籍番号が空です。";
                return section;
            }

            if (!TableExists(LiteratureTable, out string error))
            {
                if (!string.IsNullOrEmpty(error))
                    section.Error = error;
                else
                    section.Detail = "references_list がありません。";
                return section;
            }

            List<string> columns = ListColumns(LiteratureTable, out error);
            if (columns == null)
            {
                section.Error = error;
                return section;
            }

            if (!ContainsColumn(columns, "student_id"))
            {
                section.Detail = "references_list に student_id 列がないため、この人の文献としては出せません。";
                return section;
            }

            string selectList = LiteratureSelectList(columns);
            string sql = $@"
                SELECT {selectList}
                FROM `{LiteratureTable}`
                WHERE student_id = '{Escape(studentId)}'
                ORDER BY id DESC
                LIMIT 100";
            if (!ContainsColumn(columns, "id"))
            {
                sql = $@"
                    SELECT {selectList}
                    FROM `{LiteratureTable}`
                    WHERE student_id = '{Escape(studentId)}'
                    LIMIT 100";
            }

            var table = new DataTable();
            if (!Connector.TryTableReader(sql, table, out error))
            {
                section.Error = error;
                return section;
            }

            section.Rows = table;
            if (table.Rows.Count == 0)
                section.Detail = "この学籍番号の文献はありません。";
            return section;
        }

        private static string ProgressSelectList(List<string> columns)
        {
            var parts = new List<string>();
            if (!AddIfPresent(parts, columns, "progress_date", "日付"))
                AddIfPresent(parts, columns, "recorded_on", "日付");
            AddIfPresent(parts, columns, "title", "タイトル");
            AddIfPresent(parts, columns, "content", "内容");
            AddIfPresent(parts, columns, "note", "メモ");
            AddIfPresent(parts, columns, "status", "状態");
            AddIfPresent(parts, columns, "summary", "概要");
            if (parts.Count == 0)
                return null;
            return string.Join(", ", parts.ToArray());
        }

        private static string LiteratureSelectList(List<string> columns)
        {
            var parts = new List<string>();
            AddIfPresent(parts, columns, "title", "タイトル");
            AddIfPresent(parts, columns, "author", "著者");
            AddIfPresent(parts, columns, "year", "年");
            AddIfPresent(parts, columns, "thesis_type", "種類");
            AddIfPresent(parts, columns, "genre", "ジャンル");
            AddIfPresent(parts, columns, "open_date", "参照日");
            if (parts.Count == 0)
                parts.Add("student_id AS 学籍番号");
            return string.Join(", ", parts.ToArray());
        }

        private static bool AddIfPresent(List<string> parts, List<string> columns, string column, string alias)
        {
            if (!ContainsColumn(columns, column))
                return false;
            parts.Add("`" + column + "` AS `" + alias + "`");
            return true;
        }

        private static bool TableExists(string table, out string error)
        {
            var found = new DataTable();
            if (!Connector.TryTableReader("SHOW TABLES LIKE '" + Escape(table) + "'", found, out error))
                return false;
            error = null;
            return found.Rows.Count > 0;
        }

        private static List<string> ListColumns(string table, out string error)
        {
            var found = new DataTable();
            if (!Connector.TryTableReader("SHOW COLUMNS FROM `" + table + "`", found, out error))
                return null;

            var names = new List<string>();
            foreach (DataRow row in found.Rows)
            {
                string name = Convert.ToString(row["Field"]);
                if (!string.IsNullOrEmpty(name))
                    names.Add(name);
            }

            error = null;
            return names;
        }

        private static bool ContainsColumn(List<string> columns, string name)
        {
            foreach (string column in columns)
            {
                if (string.Equals(column, name, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        private static CarteSection NewSection(string title, string sources)
        {
            return new CarteSection
            {
                Title = title,
                SourceTables = sources,
                Rows = new DataTable()
            };
        }

        private static int ToInt(object value)
        {
            if (value == null || value == DBNull.Value)
                return 0;
            int parsed;
            return int.TryParse(Convert.ToString(value), out parsed) ? parsed : 0;
        }

        private static string Escape(string value)
        {
            return (value ?? "").Replace("\\", "\\\\").Replace("'", "''");
        }
    }
}
