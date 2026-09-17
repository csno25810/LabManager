using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using MySql.Data.MySqlClient;

namespace LabPortal
{
    public sealed class PortalSession
    {
        public string LoginId { get; set; }
        public string StudentId { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public DateTime ExpiresUtc { get; set; }
    }

    public sealed class AttendanceRow
    {
        public string StudentId { get; set; }
        public string Name { get; set; }
        public string FirstTouch { get; set; }
        public string LastTouch { get; set; }
        public string State { get; set; }
        public bool Present { get; set; }
        public bool Teacher { get; set; }
    }

    public sealed class PortalService
    {
        public const string DefaultPassword = "lab2026";
        public const string CookieName = "labportal_session";
        private const int SessionHours = 12;

        private readonly string connectionString;
        private readonly string serverIp;
        private readonly ConcurrentDictionary<string, PortalSession> sessions =
            new ConcurrentDictionary<string, PortalSession>();

        public PortalService(string connectionString, string serverIp)
        {
            this.connectionString = connectionString;
            this.serverIp = serverIp;
        }

        public HttpResponse Handle(HttpRequest request)
        {
            SweepSessions();
            string path = request.Path ?? "/";
            if (path == "/")
            {
                return GetSession(request) != null
                    ? HttpResponse.Redirect("/attendance")
                    : HttpResponse.Redirect("/login");
            }

            if (path == "/login")
                return HandleLogin(request);
            if (path == "/logout")
                return HandleLogout();
            if (path == "/attendance")
                return HandleAttendance(request);
            if (path == "/me")
                return HandleMyHistory(request);

            return HttpResponse.Html(Wrap("見つかりません", "<p>ページがありません。</p><p><a href=\"/\">トップ</a></p>"), 404);
        }

        public void PrepareDatabase()
        {
            using (var conn = Open())
            {
                using (var cmd = new MySqlCommand(
                    @"CREATE TABLE IF NOT EXISTS lab_user (
                        login_id VARCHAR(40) NOT NULL,
                        student_id VARCHAR(20) NOT NULL,
                        password_hash VARCHAR(255) NOT NULL,
                        role VARCHAR(20) NOT NULL DEFAULT 'student',
                        PRIMARY KEY (login_id),
                        KEY idx_lab_user_student (student_id)
                    ) ENGINE=InnoDB DEFAULT CHARSET=utf8", conn))
                {
                    cmd.ExecuteNonQuery();
                }

                int userCount;
                using (var countCmd = new MySqlCommand("SELECT COUNT(*) FROM lab_user", conn))
                    userCount = Convert.ToInt32(countCmd.ExecuteScalar());

                if (userCount > 0)
                    return;

                var people = new DataTable();
                using (var adapter = new MySqlDataAdapter(
                    "SELECT student_id, name FROM personal_info ORDER BY student_id", conn))
                {
                    adapter.Fill(people);
                }

                int inserted = 0;
                foreach (DataRow row in people.Rows)
                {
                    string studentId = Convert.ToString(row["student_id"]) ?? "";
                    if (string.IsNullOrEmpty(studentId))
                        continue;

                    bool teacher = IsTeacher(studentId);
                    string loginId = teacher ? "teacher" : studentId;
                    string role = teacher ? "teacher" : "student";
                    using (var insert = new MySqlCommand(
                        "INSERT INTO lab_user (login_id, student_id, password_hash, role) VALUES (@login, @sid, @hash, @role)",
                        conn))
                    {
                        insert.Parameters.AddWithValue("@login", loginId);
                        insert.Parameters.AddWithValue("@sid", studentId);
                        insert.Parameters.AddWithValue("@hash", HashPassword(DefaultPassword));
                        insert.Parameters.AddWithValue("@role", role);
                        try
                        {
                            insert.ExecuteNonQuery();
                            inserted++;
                        }
                        catch (MySqlException)
                        {
                            // 既にあればスキップ
                        }
                    }
                }

                Console.WriteLine("lab_user を初期化しました（" + inserted + "件）。初期パスワードは " + DefaultPassword + " です。");
            }
        }

        private HttpResponse HandleLogin(HttpRequest request)
        {
            if (request.Method == "GET")
                return HttpResponse.Html(LoginPage(""));

            string loginId = (request.FormValue("login_id") ?? "").Trim();
            string password = request.FormValue("password") ?? "";
            if (loginId.Length == 0 || password.Length == 0)
                return HttpResponse.Html(LoginPage("ログインIDとパスワードを入力してください。"));

            PortalSession session;
            if (!TryLogin(loginId, password, out session))
                return HttpResponse.Html(LoginPage("ログインIDまたはパスワードが違います。"));

            string token = NewToken();
            sessions[token] = session;
            var response = HttpResponse.Redirect("/attendance");
            response.SetCookie = CookieName + "=" + token + "; HttpOnly; Path=/; SameSite=Lax; Max-Age=" + (SessionHours * 3600);
            return response;
        }

        private HttpResponse HandleLogout()
        {
            var response = HttpResponse.Redirect("/login");
            response.SetCookie = CookieName + "=; HttpOnly; Path=/; Max-Age=0";
            return response;
        }

        private HttpResponse HandleAttendance(HttpRequest request)
        {
            PortalSession session = GetSession(request);
            if (session == null)
                return HttpResponse.Redirect("/login");

            int present;
            int visitors;
            List<AttendanceRow> rows = LoadAttendance(out present, out visitors);
            return HttpResponse.Html(AttendancePage(session, rows, present, visitors));
        }

        private HttpResponse HandleMyHistory(HttpRequest request)
        {
            PortalSession session = GetSession(request);
            if (session == null)
                return HttpResponse.Redirect("/login");

            return HttpResponse.Html(MyHistoryPage(session, LoadMyHistory(session.StudentId)));
        }

        private PortalSession GetSession(HttpRequest request)
        {
            string token = request.Cookie(CookieName);
            if (string.IsNullOrEmpty(token))
                return null;

            PortalSession session;
            if (!sessions.TryGetValue(token, out session))
                return null;
            if (session.ExpiresUtc < DateTime.UtcNow)
            {
                PortalSession removed;
                sessions.TryRemove(token, out removed);
                return null;
            }

            session.ExpiresUtc = DateTime.UtcNow.AddHours(SessionHours);
            return session;
        }

        private void SweepSessions()
        {
            foreach (var pair in sessions)
            {
                if (pair.Value.ExpiresUtc < DateTime.UtcNow)
                {
                    PortalSession removed;
                    sessions.TryRemove(pair.Key, out removed);
                }
            }
        }

        private bool TryLogin(string loginId, string password, out PortalSession session)
        {
            session = null;
            using (var conn = Open())
            using (var cmd = new MySqlCommand(
                @"SELECT u.login_id, u.student_id, u.password_hash, u.role, IFNULL(p.name, '') AS name
                  FROM lab_user u
                  LEFT JOIN personal_info p ON u.student_id = p.student_id
                  WHERE u.login_id = @login
                  LIMIT 1", conn))
            {
                cmd.Parameters.AddWithValue("@login", loginId);
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                        return false;

                    string hash = Convert.ToString(reader["password_hash"]) ?? "";
                    if (!VerifyPassword(password, hash))
                        return false;

                    session = new PortalSession
                    {
                        LoginId = Convert.ToString(reader["login_id"]),
                        StudentId = Convert.ToString(reader["student_id"]),
                        Name = Convert.ToString(reader["name"]),
                        Role = Convert.ToString(reader["role"]),
                        ExpiresUtc = DateTime.UtcNow.AddHours(SessionHours)
                    };
                    return true;
                }
            }
        }

        private List<AttendanceRow> LoadAttendance(out int presentCount, out int visitorCount)
        {
            presentCount = 0;
            visitorCount = 0;
            string today = DateTime.Now.ToString("yyyy-MM-dd") + "%";
            var rows = new List<AttendanceRow>();

            using (var conn = Open())
            {
                AddAttendanceGroup(conn, today, "pi.student_id <> @teacher AND pi.student_id NOT LIKE 'T%'",
                    rows, ref presentCount, ref visitorCount, teachers: false);
                AddAttendanceGroup(conn, today, "pi.student_id = @teacher",
                    rows, ref presentCount, ref visitorCount, teachers: true);
            }

            return rows;
        }

        private void AddAttendanceGroup(
            MySqlConnection conn,
            string todayPattern,
            string where,
            List<AttendanceRow> rows,
            ref int presentCount,
            ref int visitorCount,
            bool teachers)
        {
            string sql = @"
                SELECT
                    pi.student_id,
                    pi.name,
                    COALESCE(MIN(DATE_FORMAT(tl.time_stamp, '%H:%i')), '-') AS first_touch,
                    COALESCE(MAX(DATE_FORMAT(tl.time_stamp, '%H:%i')), '-') AS last_touch,
                    COUNT(tl.chip_id) AS touch_count
                FROM personal_info pi
                LEFT JOIN chip_list cl ON pi.student_id = cl.student_id
                LEFT JOIN touch_log tl ON cl.chip_id = tl.chip_id AND tl.time_stamp LIKE @today
                WHERE " + where + @"
                GROUP BY pi.student_id, pi.name
                ORDER BY pi.student_id";

            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@today", todayPattern);
                cmd.Parameters.AddWithValue("@teacher", "@");
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int touchCount = Convert.ToInt32(reader["touch_count"]);
                        bool present = touchCount % 2 == 1;
                        if (present)
                            presentCount++;
                        if (touchCount > 0)
                            visitorCount++;

                        rows.Add(new AttendanceRow
                        {
                            StudentId = Convert.ToString(reader["student_id"]),
                            Name = Convert.ToString(reader["name"]),
                            FirstTouch = Convert.ToString(reader["first_touch"]),
                            LastTouch = Convert.ToString(reader["last_touch"]),
                            State = present ? "在席" : "不在",
                            Present = present,
                            Teacher = teachers
                        });
                    }
                }
            }
        }

        private MySqlConnection Open()
        {
            var conn = new MySqlConnection(connectionString);
            conn.Open();
            try
            {
                using (var modeCmd = new MySqlCommand(
                    "SET SESSION sql_mode = " +
                    "(SELECT REPLACE(REPLACE(@@sql_mode, 'ONLY_FULL_GROUP_BY,', ''), 'ONLY_FULL_GROUP_BY', ''))",
                    conn))
                {
                    modeCmd.ExecuteNonQuery();
                }
            }
            catch (MySqlException)
            {
            }

            return conn;
        }

        private sealed class DayVisit
        {
            public string Date { get; set; }
            public string FirstTouch { get; set; }
            public string LastTouch { get; set; }
            public int TouchCount { get; set; }
            public string EndState { get; set; }
        }

        private sealed class TouchEvent
        {
            public string Date { get; set; }
            public string Time { get; set; }
        }

        private sealed class DutyEvent
        {
            public string Date { get; set; }
            public string Status { get; set; }
            public string Type { get; set; }
        }

        private sealed class MyHistory
        {
            public int VisitDays { get; set; }
            public string TodayState { get; set; }
            public string TodayFirst { get; set; }
            public string TodayLast { get; set; }
            public List<DayVisit> Days { get; set; }
            public List<TouchEvent> Touches { get; set; }
            public List<DutyEvent> Duties { get; set; }
        }

        private MyHistory LoadMyHistory(string studentId)
        {
            var history = new MyHistory
            {
                TodayState = "未来室",
                TodayFirst = "-",
                TodayLast = "-",
                Days = new List<DayVisit>(),
                Touches = new List<TouchEvent>(),
                Duties = new List<DutyEvent>()
            };

            DateTime from = DateTime.Today.AddDays(-30);
            string today = DateTime.Today.ToString("yyyy-MM-dd");

            using (var conn = Open())
            {
                using (var cmd = new MySqlCommand(@"
                    SELECT
                        DATE_FORMAT(tl.time_stamp, '%Y-%m-%d') AS visit_date,
                        DATE_FORMAT(MIN(tl.time_stamp), '%H:%i') AS first_touch,
                        DATE_FORMAT(MAX(tl.time_stamp), '%H:%i') AS last_touch,
                        COUNT(*) AS touch_count
                    FROM touch_log tl
                    INNER JOIN chip_list cl ON tl.chip_id = cl.chip_id
                    WHERE cl.student_id = @sid
                      AND tl.time_stamp >= @from
                    GROUP BY DATE_FORMAT(tl.time_stamp, '%Y-%m-%d')
                    ORDER BY visit_date DESC", conn))
                {
                    cmd.Parameters.AddWithValue("@sid", studentId);
                    cmd.Parameters.AddWithValue("@from", from.ToString("yyyy-MM-dd"));
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int count = Convert.ToInt32(reader["touch_count"]);
                            var day = new DayVisit
                            {
                                Date = Convert.ToString(reader["visit_date"]),
                                FirstTouch = Convert.ToString(reader["first_touch"]),
                                LastTouch = Convert.ToString(reader["last_touch"]),
                                TouchCount = count,
                                EndState = count % 2 == 1 ? "在席のまま" : "退室"
                            };
                            history.Days.Add(day);
                            if (day.Date == today)
                            {
                                history.TodayFirst = day.FirstTouch;
                                history.TodayLast = day.LastTouch;
                                history.TodayState = count % 2 == 1 ? "在席" : "不在";
                            }
                        }
                    }
                }

                history.VisitDays = history.Days.Count;

                using (var cmd = new MySqlCommand(@"
                    SELECT
                        DATE_FORMAT(tl.time_stamp, '%Y-%m-%d') AS visit_date,
                        DATE_FORMAT(tl.time_stamp, '%H:%i:%s') AS visit_time
                    FROM touch_log tl
                    INNER JOIN chip_list cl ON tl.chip_id = cl.chip_id
                    WHERE cl.student_id = @sid
                      AND tl.time_stamp >= @from
                    ORDER BY tl.time_stamp DESC
                    LIMIT 80", conn))
                {
                    cmd.Parameters.AddWithValue("@sid", studentId);
                    cmd.Parameters.AddWithValue("@from", from.ToString("yyyy-MM-dd"));
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            history.Touches.Add(new TouchEvent
                            {
                                Date = Convert.ToString(reader["visit_date"]),
                                Time = Convert.ToString(reader["visit_time"])
                            });
                        }
                    }
                }

                using (var cmd = new MySqlCommand(@"
                    SELECT
                        DATE_FORMAT(ds.duty_date, '%Y-%m-%d') AS duty_date,
                        ds.duty_status,
                        ds.duty_type
                    FROM duty_schedule ds
                    WHERE ds.student_id = @sid
                    ORDER BY ds.duty_date DESC
                    LIMIT 20", conn))
                {
                    cmd.Parameters.AddWithValue("@sid", studentId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            history.Duties.Add(new DutyEvent
                            {
                                Date = Convert.ToString(reader["duty_date"]),
                                Status = FormatDutyStatus(Convert.ToString(reader["duty_status"])),
                                Type = Convert.ToString(reader["duty_type"]) == "1" ? "罰直" : "通常"
                            });
                        }
                    }
                }
            }

            return history;
        }

        private static string FormatDutyStatus(string status)
        {
            switch (status)
            {
                case "1": return "出席";
                case "2": return "遅刻";
                default: return "未出席";
            }
        }

        private string LoginPage(string error)
        {
            string errorHtml = string.IsNullOrEmpty(error)
                ? ""
                : "<p class=\"error\">" + HttpUtility.HtmlEncode(error) + "</p>";

            return Wrap("ログイン",
                "<h1>LabPortal</h1>" +
                "<p class=\"muted\">マルチデバイス用（在席確認）。テレビ画面とは別アプリです。</p>" +
                "<p class=\"muted\">接続先: " + HttpUtility.HtmlEncode(serverIp) + "</p>" +
                errorHtml +
                "<form method=\"post\" action=\"/login\">" +
                "<label>ログインID<br><input name=\"login_id\" autocomplete=\"username\" required></label>" +
                "<label>パスワード<br><input name=\"password\" type=\"password\" autocomplete=\"current-password\" required></label>" +
                "<button type=\"submit\">ログイン</button>" +
                "</form>" +
                "<p class=\"hint\">学生は学籍番号、先生は teacher。<br>初回のパスワードは " +
                HttpUtility.HtmlEncode(DefaultPassword) + " です。</p>");
        }

        private string AttendancePage(PortalSession session, List<AttendanceRow> rows, int present, int visitors)
        {
            var sb = new StringBuilder();
            sb.Append(Nav(session, "attendance"));
            sb.Append("<p class=\"kpi\">在室 ").Append(present).Append(" 人　|　本日来室 ").Append(visitors).Append(" 人</p>");
            sb.Append("<p class=\"muted\">判定はテレビ右画面と同じです（当日タッチ奇数=在席）。30秒ごとに更新します。</p>");
            sb.Append("<table><thead><tr><th>学籍番号</th><th>氏名</th><th>初回</th><th>最終</th><th>状態</th></tr></thead><tbody>");
            foreach (AttendanceRow row in rows)
            {
                string css = row.Present ? "present" : "absent";
                string id = row.Teacher ? "" : HttpUtility.HtmlEncode(row.StudentId);
                sb.Append("<tr class=\"").Append(css).Append("\">")
                    .Append("<td>").Append(id).Append("</td>")
                    .Append("<td>").Append(HttpUtility.HtmlEncode(row.Name)).Append("</td>")
                    .Append("<td>").Append(HttpUtility.HtmlEncode(row.FirstTouch)).Append("</td>")
                    .Append("<td>").Append(HttpUtility.HtmlEncode(row.LastTouch)).Append("</td>")
                    .Append("<td>").Append(HttpUtility.HtmlEncode(row.State)).Append("</td>")
                    .Append("</tr>");
            }
            sb.Append("</tbody></table>");

            string html = Wrap("在席状況", sb.ToString());
            return html.Replace("</head>", "<meta http-equiv=\"refresh\" content=\"30\"></head>");
        }

        private string MyHistoryPage(PortalSession session, MyHistory history)
        {
            var sb = new StringBuilder();
            sb.Append(Nav(session, "me"));
            sb.Append("<p class=\"kpi\">直近30日の来室 ").Append(history.VisitDays).Append(" 日")
                .Append("　|　本日 ").Append(HttpUtility.HtmlEncode(history.TodayState))
                .Append("　").Append(HttpUtility.HtmlEncode(history.TodayFirst))
                .Append("〜").Append(HttpUtility.HtmlEncode(history.TodayLast))
                .Append("</p>");
            sb.Append("<p class=\"muted\">このページはログインした本人の記録だけを表示します。テレビには出ません。</p>");

            sb.Append("<h2>来室日（直近30日）</h2>");
            if (history.Days.Count == 0)
            {
                sb.Append("<p class=\"muted\">この期間の来室はありません。</p>");
            }
            else
            {
                sb.Append("<table><thead><tr><th>日付</th><th>初回</th><th>最終</th><th>タッチ</th><th>終了時</th></tr></thead><tbody>");
                foreach (DayVisit day in history.Days)
                {
                    sb.Append("<tr>")
                        .Append("<td>").Append(HttpUtility.HtmlEncode(day.Date)).Append("</td>")
                        .Append("<td>").Append(HttpUtility.HtmlEncode(day.FirstTouch)).Append("</td>")
                        .Append("<td>").Append(HttpUtility.HtmlEncode(day.LastTouch)).Append("</td>")
                        .Append("<td>").Append(day.TouchCount).Append("</td>")
                        .Append("<td>").Append(HttpUtility.HtmlEncode(day.EndState)).Append("</td>")
                        .Append("</tr>");
                }
                sb.Append("</tbody></table>");
            }

            sb.Append("<h2>日直（直近20件）</h2>");
            if (history.Duties.Count == 0)
            {
                sb.Append("<p class=\"muted\">日直の登録はありません。</p>");
            }
            else
            {
                sb.Append("<table><thead><tr><th>日付</th><th>種類</th><th>出席</th></tr></thead><tbody>");
                foreach (DutyEvent duty in history.Duties)
                {
                    sb.Append("<tr>")
                        .Append("<td>").Append(HttpUtility.HtmlEncode(duty.Date)).Append("</td>")
                        .Append("<td>").Append(HttpUtility.HtmlEncode(duty.Type)).Append("</td>")
                        .Append("<td>").Append(HttpUtility.HtmlEncode(duty.Status)).Append("</td>")
                        .Append("</tr>");
                }
                sb.Append("</tbody></table>");
            }

            sb.Append("<h2>タッチ履歴（直近30日・最大80件）</h2>");
            if (history.Touches.Count == 0)
            {
                sb.Append("<p class=\"muted\">タッチ記録はありません。</p>");
            }
            else
            {
                sb.Append("<table><thead><tr><th>日付</th><th>時刻</th></tr></thead><tbody>");
                foreach (TouchEvent touch in history.Touches)
                {
                    sb.Append("<tr>")
                        .Append("<td>").Append(HttpUtility.HtmlEncode(touch.Date)).Append("</td>")
                        .Append("<td>").Append(HttpUtility.HtmlEncode(touch.Time)).Append("</td>")
                        .Append("</tr>");
                }
                sb.Append("</tbody></table>");
            }

            return Wrap("自分の履歴", sb.ToString());
        }

        private static string Nav(PortalSession session, string current)
        {
            string displayId = IsTeacher(session.StudentId) ? "" : session.LoginId;
            return "<header><div><strong>" + HttpUtility.HtmlEncode(session.Name) + "</strong>" +
                   "<span class=\"muted\">　" + HttpUtility.HtmlEncode(displayId) + "</span></div></header>" +
                   "<nav>" +
                   NavLink("/attendance", "全員の在席", current == "attendance") +
                   NavLink("/me", "自分の履歴", current == "me") +
                   "<a href=\"/logout\">ログアウト</a>" +
                   "</nav>";
        }

        private static string NavLink(string href, string label, bool current)
        {
            return "<a href=\"" + href + "\"" + (current ? " class=\"current\"" : "") + ">" + label + "</a>";
        }

        private static string Wrap(string title, string inner)
        {
            return "<!doctype html><html lang=\"ja\"><head><meta charset=\"utf-8\">" +
                   "<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">" +
                   "<title>" + HttpUtility.HtmlEncode(title) + "</title>" +
                   "<style>" +
                   "body{font-family:'Yu Gothic UI','Meiryo UI',sans-serif;margin:0;background:#f4f4f4;color:#111;}" +
                   "main{max-width:720px;margin:0 auto;padding:16px;}" +
                   "h1{font-size:1.4rem;margin:0 0 12px;}" +
                   "form{display:flex;flex-direction:column;gap:12px;background:#fff;padding:16px;border:1px solid #ddd;}" +
                   "input{font-size:1rem;padding:10px;width:100%;box-sizing:border-box;}" +
                   "button{font-size:1rem;padding:12px;background:#111;color:#fff;border:0;}" +
                   ".error{color:#a40000;font-weight:bold;}" +
                   ".muted,.hint{color:#666;font-size:.9rem;}" +
                   "header{display:flex;justify-content:space-between;align-items:center;gap:12px;margin-bottom:8px;}" +
                   "nav{display:flex;gap:12px;flex-wrap:wrap;margin-bottom:16px;padding-bottom:8px;border-bottom:1px solid #ccc;}" +
                   "nav a.current{font-weight:bold;}" +
                   "h2{font-size:1.1rem;margin:20px 0 8px;}" +
                   ".kpi{background:#fff;border:1px solid #111;padding:12px;font-weight:bold;text-align:center;}" +
                   "table{width:100%;border-collapse:collapse;background:#fff;}" +
                   "th,td{border:1px solid #ccc;padding:8px;text-align:center;}" +
                   "tr.present td{background:#fff;color:#111;font-weight:bold;}" +
                   "tr.absent td{background:#e1e1e1;color:#6e6e6e;}" +
                   "a{color:#111;}" +
                   "</style></head><body><main>" + inner + "</main></body></html>";
        }

        private static bool IsTeacher(string studentId)
        {
            if (string.IsNullOrEmpty(studentId))
                return false;
            return studentId == "@" || studentId.StartsWith("T", StringComparison.Ordinal);
        }

        private static string NewToken()
        {
            byte[] bytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").TrimEnd('=');
        }

        private static string HashPassword(string password)
        {
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(salt);
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000))
            {
                byte[] hash = pbkdf2.GetBytes(32);
                return "pbkdf2$10000$" + Convert.ToBase64String(salt) + "$" + Convert.ToBase64String(hash);
            }
        }

        private static bool VerifyPassword(string password, string stored)
        {
            if (string.IsNullOrEmpty(stored))
                return false;
            string[] parts = stored.Split('$');
            if (parts.Length != 4 || parts[0] != "pbkdf2")
                return false;

            int iterations;
            if (!int.TryParse(parts[1], out iterations) || iterations < 1)
                return false;

            byte[] salt;
            byte[] expected;
            try
            {
                salt = Convert.FromBase64String(parts[2]);
                expected = Convert.FromBase64String(parts[3]);
            }
            catch
            {
                return false;
            }

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations))
            {
                byte[] actual = pbkdf2.GetBytes(expected.Length);
                return SlowEquals(expected, actual);
            }
        }

        private static bool SlowEquals(byte[] a, byte[] b)
        {
            uint diff = (uint)a.Length ^ (uint)b.Length;
            int n = Math.Min(a.Length, b.Length);
            for (int i = 0; i < n; i++)
                diff |= (uint)(a[i] ^ b[i]);
            return diff == 0;
        }
    }

    public static class IniSettings
    {
        public static bool TryRead(out string user, out string password, out string server, out string database, out string error)
        {
            user = password = server = database = null;
            error = null;
            string path = @"C:\MyReader\SQLReader.ini";
            if (!File.Exists(path))
            {
                error = path + " がありません。LabManager の設定画面で作成してください。";
                return false;
            }

            user = "userid";
            password = "passwd";
            server = "127.0.0.1";
            database = "felica";

            foreach (string raw in File.ReadAllLines(path))
            {
                int eq = raw.IndexOf('=');
                if (eq <= 0)
                    continue;
                string key = raw.Substring(0, eq).Trim();
                string value = raw.Substring(eq + 1).Trim();
                switch (key)
                {
                    case "UserID": user = value; break;
                    case "PassWd": password = value; break;
                    case "ServerIP": server = value; break;
                    case "DataBaseName": database = value; break;
                }
            }

            return true;
        }
    }
}
