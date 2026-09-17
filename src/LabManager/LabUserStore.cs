using System.Data;

namespace LabManager
{
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

        private static string Escape(string value)
        {
            return (value ?? "").Replace("\\", "\\\\").Replace("'", "''");
        }
    }
}
