using System;

namespace LabManager
{
    /// <summary>
    /// personal_info の役職区分（学生 / 先生）。
    /// 先生は内部 ID '@' のみ（学籍番号は画面に表示しない）。
    /// </summary>
    public static class PersonalInfoHelper
    {
        public const string TeacherStudentId = "@";

        public static bool IsTeacher(string studentId)
        {
            if (string.IsNullOrEmpty(studentId))
                return false;
            return studentId == TeacherStudentId || studentId.StartsWith("T", StringComparison.Ordinal);
        }

        /// <summary>personal_info 単体クエリ用</summary>
        public const string SqlStudentsOnly = "student_id <> '@' AND student_id NOT LIKE 'T%'";

        /// <summary>personal_info を pi エイリアスで JOIN するクエリ用</summary>
        public const string SqlStudentsOnlyAliased = "pi.student_id <> '@' AND pi.student_id NOT LIKE 'T%'";

        public static string FormatStudentIdDisplay(string studentId)
        {
            return IsTeacher(studentId) ? "" : studentId ?? "";
        }
    }
}
