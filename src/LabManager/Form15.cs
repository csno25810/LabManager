using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace LabManager
{
    /// <summary>
    /// まとめページ — 在席・日直・カレンダー・日誌を1画面に集約する閲覧専用画面。
    /// 編集は下部ボタンから既存 Form へ遷移する。
    /// </summary>
    public partial class Form15 : Form
    {
        private static readonly TimeSpan DutyDeadline = new TimeSpan(8, 50, 0);
        private static readonly CultureInfo Ja = CultureInfo.GetCultureInfo("ja-JP");

        private readonly Setting mySqlSet;
        private bool loading;

        public Form15(Setting settings)
        {
            mySqlSet = settings;
            InitializeComponent();

            Font ui = UiFonts.Get(9F);
            Font = ui;
            lblTitle.Font = UiFonts.Get(14F, FontStyle.Bold);
            lblKpi.Font = UiFonts.Get(11F, FontStyle.Bold);
            lblRoster.Font = UiFonts.Get(9F);
            gridAttendance.Font = UiFonts.Get(10F);
            gridCalendar.Font = UiFonts.Get(9F);
            gridDuty.Font = UiFonts.Get(10F);
            gridDiary.Font = UiFonts.Get(9F);
            txtDiary.Font = UiFonts.Get(9F);
        }

        private void Form15_Load(object sender, EventArgs e)
        {
            loading = true;
            datePicker.Value = DateTime.Today;
            loading = false;
            ReloadAll();
        }

        private void datePicker_ValueChanged(object sender, EventArgs e)
        {
            if (!loading)
                ReloadAll();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            ReloadAll();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnOpenDuty_Click(object sender, EventArgs e)
        {
            using (var form = new Form5(mySqlSet))
                form.ShowDialog(this);
            ReloadAll();
        }

        private void btnOpenCal_Click(object sender, EventArgs e)
        {
            using (var form = new Form10(mySqlSet))
                form.ShowDialog(this);
            ReloadAll();
        }

        private void btnOpenDiary_Click(object sender, EventArgs e)
        {
            using (var form = new Form6(mySqlSet))
                form.ShowDialog(this);
            ReloadAll();
        }

        private bool EnsureConnected()
        {
            if (Connector.IsConnected)
                return true;

            return Connector.Connect(
                mySqlSet.UserID,
                mySqlSet.PassWd,
                mySqlSet.DataBaseName,
                mySqlSet.ServerIP);
        }

        private void ReloadAll()
        {
            loading = true;
            try
            {
                DateTime target = datePicker.Value.Date;
                txtDiary.Clear();

                if (!EnsureConnected())
                {
                    lblStatus.Text = "接続: 未接続（C:\\MyReader\\SQLReader.ini の ServerIP を確認）";
                    lblKpi.Text = "授業日: --　予定: --　在室 -- 人　来室 -- 人　日直 --　日誌 -- 件";
                    gridAttendance.DataSource = null;
                    gridCalendar.DataSource = null;
                    gridDuty.DataSource = null;
                    gridDiary.DataSource = null;
                    lblRoster.Text = "曜日担当: （未接続）";
                    return;
                }

                var warnings = new List<string>();
                var dayMap = LabCalendarStore.LoadDayMap(out string calendarError);
                if (!string.IsNullOrEmpty(calendarError))
                    warnings.Add(calendarError);

                LabCalendarDay dayInfo = LabCalendarStore.GetDay(dayMap, target);
                bool isDutyDay = LabCalendarStore.IsDutyEligibleClassDay(dayMap, target);

                int presentCount;
                int visitorCount;
                LoadAttendance(target, out presentCount, out visitorCount, warnings);
                int dutyCount = LoadDuty(target, isDutyDay, warnings);
                LoadCalendar(target, dayMap);
                int diaryCount = LoadDiaries(target, warnings);
                LoadWeekdayRoster(warnings);

                string classText = dayInfo.IsClassDay
                    ? "はい" + (string.IsNullOrEmpty(dayInfo.SessionSymbol) ? "" : "（" + dayInfo.SessionSymbol + "）")
                    : "いいえ";
                string memoText = string.IsNullOrWhiteSpace(dayInfo.Memo) ? "なし" : dayInfo.Memo;
                string dutyText = isDutyDay ? dutyCount + "名" : "授業日ではない";
                string occupancyLabel = target == DateTime.Today ? "在室" : "当日最終在席";

                lblKpi.Text =
                    target.ToString("yyyy/MM/dd(ddd)", Ja) +
                    "　授業日: " + classText +
                    "　予定: " + memoText +
                    "　" + occupancyLabel + " " + presentCount + " 人" +
                    "　来室 " + visitorCount + " 人" +
                    "　日直 " + dutyText +
                    "　日誌 " + diaryCount + " 件";

                lblStatus.Text = warnings.Count == 0
                    ? "接続: 接続済　閲覧専用（編集は下のボタンから）"
                    : "接続: 接続済　" + string.Join(" / ", warnings);
            }
            finally
            {
                loading = false;
            }
        }

        private void LoadAttendance(DateTime target, out int presentCount, out int visitorCount, List<string> warnings)
        {
            presentCount = 0;
            visitorCount = 0;
            string day = target.ToString("yyyy-MM-dd");
            bool isToday = target == DateTime.Today;

            DataTable students = QueryAttendance(day, PersonalInfoHelper.SqlStudentsOnlyAliased, warnings);
            DataTable teacher = QueryAttendance(day, "pi.student_id = '" + PersonalInfoHelper.TeacherStudentId + "'", warnings);
            if (students == null)
            {
                gridAttendance.DataSource = null;
                return;
            }

            if (teacher != null)
            {
                foreach (DataRow row in teacher.Rows)
                    students.ImportRow(row);
            }

            presentCount = ApplyAttendanceState(students, isToday);
            visitorCount = CountVisitors(students);

            if (students.Columns.Contains("touch_count"))
                students.Columns.Remove("touch_count");

            gridAttendance.DataSource = students;
            BindAttendanceColumns();
            ApplyAttendanceColors();
        }

        private DataTable QueryAttendance(string day, string whereClause, List<string> warnings)
        {
            string sql = $@"
                SELECT
                    pi.student_id,
                    pi.name,
                    COALESCE(MIN(DATE_FORMAT(tl.time_stamp, '%H:%i')), '-') AS first_touch,
                    COALESCE(MAX(DATE_FORMAT(tl.time_stamp, '%H:%i')), '-') AS last_touch,
                    COUNT(tl.chip_id) AS touch_count
                FROM personal_info pi
                LEFT JOIN chip_list cl ON pi.student_id = cl.student_id
                LEFT JOIN touch_log tl ON cl.chip_id = tl.chip_id AND tl.time_stamp LIKE '{day}%'
                WHERE {whereClause}
                GROUP BY pi.student_id, pi.name
                ORDER BY pi.student_id";

            var table = new DataTable();
            if (!Connector.TryTableReader(sql, table, out string error))
            {
                warnings.Add("在席: " + error);
                return null;
            }

            return table;
        }

        private static int ApplyAttendanceState(DataTable table, bool isToday)
        {
            if (!table.Columns.Contains("State"))
                table.Columns.Add("State", typeof(string));

            string onSeat = isToday ? "在席" : "来室";
            string offSeat = isToday ? "不在" : "未来室";
            int presentCount = 0;

            foreach (DataRow row in table.Rows)
            {
                int touchCount = Convert.ToInt32(row["touch_count"]);
                if (isToday)
                {
                    if (touchCount % 2 == 1)
                    {
                        row["State"] = onSeat;
                        presentCount++;
                    }
                    else
                    {
                        row["State"] = offSeat;
                    }
                }
                else
                {
                    if (touchCount > 0)
                    {
                        row["State"] = onSeat;
                        if (touchCount % 2 == 1)
                            presentCount++;
                    }
                    else
                    {
                        row["State"] = offSeat;
                    }
                }
            }

            return presentCount;
        }

        private static int CountVisitors(DataTable table)
        {
            int count = 0;
            foreach (DataRow row in table.Rows)
            {
                if (Convert.ToInt32(row["touch_count"]) > 0)
                    count++;
            }
            return count;
        }

        private void BindAttendanceColumns()
        {
            if (gridAttendance.Columns.Count < 5)
                return;

            gridAttendance.Columns[0].HeaderText = "学籍番号";
            gridAttendance.Columns[1].HeaderText = "氏名";
            gridAttendance.Columns[2].HeaderText = "初回";
            gridAttendance.Columns[3].HeaderText = "最終";
            gridAttendance.Columns[4].HeaderText = "状態";
            gridAttendance.Columns[0].Width = 110;
            gridAttendance.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            gridAttendance.Columns[2].Width = 70;
            gridAttendance.Columns[3].Width = 70;
            gridAttendance.Columns[4].Width = 70;
        }

        private void ApplyAttendanceColors()
        {
            if (gridAttendance.RowCount == 0)
                return;

            bool isToday = datePicker.Value.Date == DateTime.Today;
            string presentLabel = isToday ? "在席" : "来室";

            for (int i = 0; i < gridAttendance.RowCount; i++)
            {
                var row = gridAttendance.Rows[i];
                bool present = row.Cells["State"].Value?.ToString() == presentLabel;
                Color back = present ? Color.White : Color.FromArgb(225, 225, 225);
                Color fore = present ? Color.Black : Color.FromArgb(110, 110, 110);
                Font font = UiFonts.Get(10F, present ? FontStyle.Bold : FontStyle.Regular);
                row.DefaultCellStyle.BackColor = back;
                row.DefaultCellStyle.ForeColor = fore;
                row.DefaultCellStyle.Font = font;
                row.DefaultCellStyle.SelectionBackColor = back;
                row.DefaultCellStyle.SelectionForeColor = fore;
            }
        }

        private void gridAttendance_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || gridAttendance.Columns[e.ColumnIndex].Name != "student_id")
                return;

            string studentId = gridAttendance.Rows[e.RowIndex].Cells["student_id"]?.Value?.ToString();
            if (PersonalInfoHelper.IsTeacher(studentId))
                e.Value = "";
        }

        private int LoadDuty(DateTime target, bool isDutyDay, List<string> warnings)
        {
            if (!isDutyDay)
            {
                gridDuty.DataSource = null;
                groupDuty.Text = "日直（対象日は授業日ではないため自動日直なし）";
                return 0;
            }

            string day = target.ToString("yyyy-MM-dd");
            string sql = $@"
                SELECT
                    ds.student_id,
                    pi.name,
                    COALESCE(MIN(DATE_FORMAT(tl.time_stamp, '%H:%i')), '-') AS attendance_time,
                    ds.duty_status,
                    ds.duty_type,
                    pi.penalty_count
                FROM duty_schedule ds
                INNER JOIN personal_info pi ON ds.student_id = pi.student_id
                LEFT JOIN chip_list cl ON ds.student_id = cl.student_id
                LEFT JOIN touch_log tl ON cl.chip_id = tl.chip_id AND DATE(tl.time_stamp) = '{day}'
                WHERE ds.duty_date = '{day}'
                GROUP BY ds.student_id, pi.name, ds.duty_status, ds.duty_type, pi.penalty_count";

            var table = new DataTable();
            if (!Connector.TryTableReader(sql, table, out string error))
            {
                warnings.Add("日直: " + error);
                gridDuty.DataSource = null;
                return 0;
            }

            groupDuty.Text = table.Rows.Count == 0
                ? "日直（担当なし）"
                : "日直（" + table.Rows.Count + "名）";
            gridDuty.DataSource = table;
            BindDutyColumns();
            return table.Rows.Count;
        }

        private void BindDutyColumns()
        {
            if (gridDuty.Columns.Count < 6)
                return;

            if (gridDuty.Columns.Contains("duty_type"))
                gridDuty.Columns["duty_type"].Visible = false;

            gridDuty.Columns["student_id"].HeaderText = "学籍番号";
            gridDuty.Columns["name"].HeaderText = "氏名";
            gridDuty.Columns["attendance_time"].HeaderText = "出席時刻";
            gridDuty.Columns["duty_status"].HeaderText = "出席状況";
            gridDuty.Columns["penalty_count"].HeaderText = "罰直";
            gridDuty.Columns["student_id"].Width = 110;
            gridDuty.Columns["name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            gridDuty.Columns["attendance_time"].Width = 80;
            gridDuty.Columns["duty_status"].Width = 120;
            gridDuty.Columns["penalty_count"].Width = 50;
        }

        private void gridDuty_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            var row = gridDuty.Rows[e.RowIndex];
            string attendanceTime = row.Cells["attendance_time"]?.Value?.ToString() ?? "-";
            bool hasTouched = attendanceTime != "-";
            bool isToday = datePicker.Value.Date == DateTime.Today;
            bool beforeDeadline = isToday && DateTime.Now.TimeOfDay < DutyDeadline;
            bool isPenaltyDuty = row.Cells["duty_type"]?.Value?.ToString() == "1";

            if (gridDuty.Columns[e.ColumnIndex].Name == "name" && e.Value != null)
            {
                string name = e.Value.ToString();
                if (isPenaltyDuty && !name.Contains("罰直"))
                    e.Value = name + "（罰直）";
            }

            if (gridDuty.Columns[e.ColumnIndex].Name != "duty_status" || e.Value == null)
                return;

            switch (e.Value.ToString())
            {
                case "0":
                    if (!hasTouched && beforeDeadline)
                    {
                        var remaining = DutyDeadline - DateTime.Now.TimeOfDay;
                        e.Value = "待機中（あと" + remaining.Minutes + "分）";
                    }
                    else if (!hasTouched)
                        e.Value = "未タッチ";
                    else
                        e.Value = "未出席";
                    break;
                case "1":
                    e.Value = "出席";
                    break;
                case "2":
                    e.Value = "遅刻";
                    break;
            }
        }

        private void LoadCalendar(DateTime target, Dictionary<DateTime, LabCalendarDay> dayMap)
        {
            DateTime rangeEnd = target.AddDays(13);
            Dictionary<DateTime, string> dutySurnames = LabCalendarStore.LoadDutySurnames(target, rangeEnd);

            var table = new DataTable();
            table.Columns.Add("日付", typeof(string));
            table.Columns.Add("曜", typeof(string));
            table.Columns.Add("授業", typeof(string));
            table.Columns.Add("予定", typeof(string));
            table.Columns.Add("日直", typeof(string));

            string[] weekdayNames = { "日", "月", "火", "水", "木", "金", "土" };
            for (int i = 0; i < 14; i++)
            {
                DateTime date = target.AddDays(i);
                LabCalendarDay info = LabCalendarStore.GetDay(dayMap, date);
                dutySurnames.TryGetValue(date.Date, out string duty);
                table.Rows.Add(
                    date.ToString("MM/dd"),
                    weekdayNames[(int)date.DayOfWeek],
                    info.IsClassDay ? info.SessionSymbol : "",
                    info.Memo ?? "",
                    duty ?? "");
            }

            gridCalendar.DataSource = table;
            if (gridCalendar.Columns.Count >= 5)
            {
                gridCalendar.Columns[0].Width = 60;
                gridCalendar.Columns[1].Width = 36;
                gridCalendar.Columns[2].Width = 44;
                gridCalendar.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                gridCalendar.Columns[4].Width = 120;
            }

            if (gridCalendar.RowCount > 0)
            {
                gridCalendar.Rows[0].DefaultCellStyle.Font = UiFonts.Get(9F, FontStyle.Bold);
                gridCalendar.Rows[0].DefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            }
        }

        private int LoadDiaries(DateTime target, List<string> warnings)
        {
            DateTime from = target.AddDays(-30);
            string sql = $@"
                SELECT
                    DATE_FORMAT(d.dialy_date, '%Y-%m-%d') AS diary_date,
                    d.student_id,
                    IFNULL(p.name, '') AS name,
                    d.title,
                    d.content
                FROM diary_log d
                LEFT JOIN personal_info p ON d.student_id = p.student_id
                WHERE d.dialy_date BETWEEN '{from:yyyy-MM-dd}' AND '{target:yyyy-MM-dd}'
                ORDER BY d.dialy_date DESC, d.student_id";

            var table = new DataTable();
            if (!Connector.TryTableReader(sql, table, out string error))
            {
                warnings.Add("日誌: " + error);
                gridDiary.DataSource = null;
                txtDiary.Text = "";
                return 0;
            }

            int todayCount = table.Rows.Cast<DataRow>()
                .Count(r => string.Equals(r["diary_date"]?.ToString(), target.ToString("yyyy-MM-dd"), StringComparison.Ordinal));

            gridDiary.DataSource = table;
            if (gridDiary.Columns.Contains("content"))
                gridDiary.Columns["content"].Visible = false;
            if (gridDiary.Columns.Contains("diary_date"))
            {
                gridDiary.Columns["diary_date"].HeaderText = "日付";
                gridDiary.Columns["diary_date"].Width = 90;
            }
            if (gridDiary.Columns.Contains("student_id"))
            {
                gridDiary.Columns["student_id"].HeaderText = "学籍番号";
                gridDiary.Columns["student_id"].Width = 90;
            }
            if (gridDiary.Columns.Contains("name"))
            {
                gridDiary.Columns["name"].HeaderText = "氏名";
                gridDiary.Columns["name"].Width = 80;
            }
            if (gridDiary.Columns.Contains("title"))
            {
                gridDiary.Columns["title"].HeaderText = "タイトル";
                gridDiary.Columns["title"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }

            groupDiary.Text = "日誌（直近30日 / 対象日 " + todayCount + " 件）";
            if (gridDiary.RowCount > 0)
                ShowDiaryContent(gridDiary.Rows[0]);
            else
                txtDiary.Text = "該当する日誌はありません。";

            return todayCount;
        }

        private void LoadWeekdayRoster(List<string> warnings)
        {
            var students = DutyWeekdayRosterStore.LoadAllStudents(out string studentError);
            if (!string.IsNullOrEmpty(studentError))
                warnings.Add(studentError);

            var roster = DutyWeekdayRosterStore.LoadRoster(out string rosterError);
            if (!string.IsNullOrEmpty(rosterError))
            {
                warnings.Add(rosterError);
                lblRoster.Text = "曜日担当: 取得失敗";
                return;
            }

            lblRoster.Text = "曜日担当（月〜金）\r\n" + DutyWeekdayRosterStore.FormatRosterSummary(roster, students);
        }

        private void gridDiary_SelectionChanged(object sender, EventArgs e)
        {
            if (gridDiary.CurrentRow == null)
            {
                txtDiary.Clear();
                return;
            }

            ShowDiaryContent(gridDiary.CurrentRow);
        }

        private void ShowDiaryContent(DataGridViewRow row)
        {
            if (row == null || !gridDiary.Columns.Contains("content"))
            {
                txtDiary.Clear();
                return;
            }

            string date = row.Cells["diary_date"]?.Value?.ToString() ?? "";
            string name = row.Cells["name"]?.Value?.ToString() ?? "";
            string title = row.Cells["title"]?.Value?.ToString() ?? "";
            string content = row.Cells["content"]?.Value?.ToString() ?? "";
            txtDiary.Text = date + "  " + name + "\r\n" + title + "\r\n\r\n" + content;
        }
    }
}
