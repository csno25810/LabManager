using System;
using System.Data;

namespace LabManager
{
    /// <summary>
    /// LabManager にログインした本人。認証は lab_user の既存パスワード（PBKDF2）だけ。
    /// </summary>
    public sealed class LabSession
    {
        public string LoginId { get; set; }
        public string StudentId { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }

        public bool IsTeacher
        {
            get
            {
                return string.Equals(Role, "teacher", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(Role, "admin", StringComparison.OrdinalIgnoreCase)
                    || PersonalInfoHelper.IsTeacher(StudentId);
            }
        }
    }

    public static class LabUserStore
    {
        private const string CreateTableSql =
            @"CREATE TABLE IF NOT EXISTS lab_user (
                login_id VARCHAR(40) NOT NULL,
                student_id VARCHAR(20) NOT NULL,
                password_hash VARCHAR(255) NOT NULL,
                role VARCHAR(20) NOT NULL DEFAULT 'student',
                PRIMARY KEY (login_id),
                KEY idx_lab_user_student (student_id)
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8";

        public static void EnsureSchema()
        {
            Connector.TryExecuteCommand(CreateTableSql, out _);
        }

        public static bool TryCreateForStudent(string studentId, out string error)
        {
            error = null;
            if (string.IsNullOrWhiteSpace(studentId))
            {
                error = "学籍番号が空です。";
                return false;
            }

            EnsureSchema();
            string loginId = PersonalInfoHelper.IsTeacher(studentId) ? "teacher" : studentId;
            string role = PersonalInfoHelper.IsTeacher(studentId) ? "teacher" : "student";

            var exists = new DataTable();
            if (!Connector.TryTableReader(
                    $"SELECT 1 FROM lab_user WHERE login_id = '{Escape(loginId)}' LIMIT 1",
                    exists,
                    out error))
                return false;

            if (exists.Rows.Count > 0)
                return true;

            string hash = LabCommon.PasswordHash.Hash(LabCommon.PasswordHash.DefaultPassword);
            string sql = $@"INSERT INTO lab_user (login_id, student_id, password_hash, role)
                            VALUES ('{Escape(loginId)}', '{Escape(studentId)}', '{Escape(hash)}', '{Escape(role)}')";
            return Connector.TryExecuteCommand(sql, out error);
        }

        public static void DeleteForStudent(string studentId)
        {
            if (string.IsNullOrWhiteSpace(studentId))
                return;
            EnsureSchema();
            Connector.TryExecuteCommand(
                $"DELETE FROM lab_user WHERE student_id = '{Escape(studentId)}'",
                out _);
        }

        /// <summary>
        /// 学籍番号（教員は login_id <c>teacher</c> または内部ID <c>@</c>）と
        /// lab_user.password_hash で本人を特定する。メールアドレスや OAuth は受け付けない。
        /// 行が無く personal_info にあるときだけ、既存の初期パスワード作成（TryCreateForStudent）を使う。
        /// </summary>
        public static bool TryLogin(string studentIdOrLogin, string password, out LabSession session, out string error)
        {
            session = null;
            error = null;
            string key = (studentIdOrLogin ?? "").Trim();
            if (key.Length == 0 || string.IsNullOrEmpty(password))
            {
                error = "学籍番号とパスワードを入力してください。";
                return false;
            }

            if (key != PersonalInfoHelper.TeacherStudentId && key.IndexOf('@') >= 0)
            {
                error = "学籍番号でログインしてください。メールアドレスは使えません。";
                return false;
            }

            EnsureSchema();
            string lookup = string.Equals(key, "teacher", StringComparison.OrdinalIgnoreCase)
                ? "teacher"
                : key;

            string hash;
            if (!TryReadUser(lookup, out session, out hash, out error))
            {
                if (!string.IsNullOrEmpty(error))
                {
                    session = null;
                    return false;
                }

                string personalId = ResolvePersonalStudentId(lookup);
                if (personalId == null)
                {
                    session = null;
                    error = "学籍番号またはパスワードが違います。";
                    return false;
                }

                if (!TryCreateForStudent(personalId, out error))
                {
                    session = null;
                    return false;
                }

                if (!TryReadUser(lookup, out session, out hash, out error))
                {
                    session = null;
                    error = error ?? "学籍番号またはパスワードが違います。";
                    return false;
                }
            }

            if (session == null || !LabCommon.PasswordHash.Verify(password, hash))
            {
                session = null;
                error = "学籍番号またはパスワードが違います。";
                return false;
            }

            error = null;
            return true;
        }

        private static bool TryReadUser(string lookup, out LabSession session, out string passwordHash, out string error)
        {
            session = null;
            passwordHash = null;
            error = null;

            var table = new DataTable();
            string sql = $@"
                SELECT u.login_id, u.student_id, u.password_hash, u.role, IFNULL(p.name, '') AS name
                FROM lab_user u
                LEFT JOIN personal_info p ON u.student_id = p.student_id
                WHERE u.login_id = '{Escape(lookup)}' OR u.student_id = '{Escape(lookup)}'";
            if (!Connector.TryTableReader(sql, table, out error))
                return false;

            if (table.Rows.Count == 0)
                return false;

            if (table.Rows.Count > 1)
            {
                error = "同じ学籍番号のログインが複数あります。学生情報管理で確認してください。";
                return false;
            }

            DataRow row = table.Rows[0];
            passwordHash = Convert.ToString(row["password_hash"]) ?? "";
            session = new LabSession
            {
                LoginId = Convert.ToString(row["login_id"]) ?? "",
                StudentId = Convert.ToString(row["student_id"]) ?? "",
                Name = Convert.ToString(row["name"]) ?? "",
                Role = Convert.ToString(row["role"]) ?? "student"
            };
            return true;
        }

        /// <summary>login 入力を personal_info.student_id に戻す。無いときは null。</summary>
        private static string ResolvePersonalStudentId(string lookup)
        {
            string candidate = lookup == "teacher" ? PersonalInfoHelper.TeacherStudentId : lookup;
            var table = new DataTable();
            if (!Connector.TryTableReader(
                    $"SELECT student_id FROM personal_info WHERE student_id = '{Escape(candidate)}' LIMIT 1",
                    table,
                    out _))
                return null;
            if (table.Rows.Count == 0)
                return null;
            return Convert.ToString(table.Rows[0]["student_id"]);
        }

        private static string Escape(string value)
        {
            return (value ?? "").Replace("\\", "\\\\").Replace("'", "''");
        }
    }
}
