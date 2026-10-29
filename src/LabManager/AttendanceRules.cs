using System;

namespace LabManager
{
    /// <summary>
    /// 在席と日直出席の判定。テレビ右画面（Form1）、まとめページ（Form15）、
    /// タッチ履歴編集（Form17）で同じ基準を使う。
    /// LabPortal の在席表示も、当日タッチ回数が奇数なら在席・偶数なら不在。
    /// </summary>
    public static class AttendanceRules
    {
        /// <summary>日直の基準時刻。これより前の初回タッチは出席。</summary>
        public static readonly TimeSpan DutyDeadline = new TimeSpan(8, 50, 0);

        /// <summary>基準時刻からの猶予。これを超えた初回タッチは遅刻。</summary>
        public static readonly TimeSpan OnTimeGrace = TimeSpan.FromMinutes(10);

        public static bool IsPresent(int touchCount)
        {
            return touchCount % 2 == 1;
        }

        public static string PresenceLabel(int touchCount)
        {
            return IsPresent(touchCount) ? "在席" : "不在";
        }

        /// <summary>
        /// 在席一覧での見え方。タッチが無い人は一覧に出ない。
        /// </summary>
        public static string BoardLabel(int touchCount)
        {
            if (touchCount <= 0)
                return "未表示（タッチなし）";
            return PresenceLabel(touchCount);
        }

        /// <summary>
        /// その日の日直から編集対象を選ぶ。氏名に「伊藤」がいればその人、いなければ先頭。
        /// </summary>
        public static int IndexOfPreferredDuty(System.Collections.Generic.IList<string> names)
        {
            if (names == null || names.Count == 0)
                return -1;

            for (int i = 0; i < names.Count; i++)
            {
                string name = names[i];
                if (!string.IsNullOrEmpty(name) && name.Contains("伊藤"))
                    return i;
            }

            return 0;
        }

        /// <summary>
        /// その日の最初のタッチから日直の出席状況を決める。
        /// 8:50 から 10 分以内は出席(1)、それを超えたら遅刻(2)。
        /// </summary>
        public static int DutyStatusFromFirstTouch(DateTime firstTouch)
        {
            DateTime dutyTime = firstTouch.Date + DutyDeadline;
            TimeSpan delay = firstTouch - dutyTime;
            return delay > OnTimeGrace ? 2 : 1;
        }

        public static string DutyStatusLabel(int status)
        {
            switch (status)
            {
                case 1:
                    return "出席";
                case 2:
                    return "遅刻";
                default:
                    return "未出席";
            }
        }
    }
}
