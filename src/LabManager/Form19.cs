using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace LabManager
{
    /// <summary>
    /// 個人カルテ。ログインした学籍番号、または教員が指定した学籍番号の
    /// 出席・日直・進捗・文献だけを出す。まとめページ（Form15）とは別画面。
    /// </summary>
    public class Form19 : Form
    {
        private sealed class StudentChoice
        {
            public string StudentId;
            public string Name;

            public override string ToString()
            {
                return StudentId + "  " + Name;
            }
        }

        private readonly Setting settings;
        private readonly LabSession session;
        private readonly Label labelPerson;
        private readonly ComboBox comboStudent;
        private readonly Label labelAttendance;
        private readonly Label labelDuty;
        private readonly Label labelProgress;
        private readonly Label labelLiterature;
        private readonly DataGridView gridAttendance;
        private readonly DataGridView gridDuty;
        private readonly DataGridView gridProgress;
        private readonly DataGridView gridLiterature;
        private bool loadingStudents;

        public Form19(Setting settings, LabSession session)
        {
            this.settings = settings ?? new Setting();
            this.session = session ?? new LabSession();

            Text = "個人カルテ";
            Font = UiFonts.Get(9F);
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(980, 700);
            MinimumSize = new Size(800, 560);

            var title = new Label
            {
                Dock = DockStyle.Fill,
                Font = UiFonts.Get(14F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Text = "個人カルテ"
            };
            var help = new Label
            {
                Dock = DockStyle.Fill,
                Text =
                    "ログインした人の記録だけを表示します。教員は下で学籍番号を指定します。\r\n" +
                    "進捗は研究の途中経過、文献はその人が読んだ論文・資料です（いまは仮データ）。日直は本日まで。\r\n" +
                    "カレンダーと日誌は出しません。研究室全体の一覧は MENU の「15:まとめページ」です。"
            };
            labelPerson = new Label
            {
                Dock = DockStyle.Fill,
                Font = UiFonts.Get(11F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Text = ""
            };

            var picker = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false
            };
            comboStudent = new ComboBox
            {
                Width = 280,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Visible = session.IsTeacher
            };
            var buttonShow = new Button
            {
                Text = "表示",
                Width = 80,
                Visible = session.IsTeacher
            };
            var pickerHint = new Label
            {
                AutoSize = true,
                Padding = new Padding(8, 6, 0, 0),
                Text = session.IsTeacher ? "見る学籍番号を選んで「表示」を押します。" : "",
                Visible = session.IsTeacher
            };
            buttonShow.Click += (sender, e) => ReloadSelected();
            comboStudent.SelectedIndexChanged += (sender, e) =>
            {
                if (!loadingStudents)
                    ReloadSelected();
            };
            picker.Controls.Add(comboStudent);
            picker.Controls.Add(buttonShow);
            picker.Controls.Add(pickerHint);

            gridAttendance = MakeGrid();
            gridDuty = MakeGrid();
            gridProgress = MakeGrid();
            gridLiterature = MakeGrid();
            labelAttendance = MakeStatus();
            labelDuty = MakeStatus();
            labelProgress = MakeStatus();
            labelLiterature = MakeStatus();

            var body = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2
            };
            body.ColumnStyles.Clear();
            body.RowStyles.Clear();
            body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            body.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            body.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            body.Controls.Add(MakeGroup("出席", labelAttendance, gridAttendance), 0, 0);
            body.Controls.Add(MakeGroup("日直（本日まで）", labelDuty, gridDuty), 1, 0);
            body.Controls.Add(MakeGroup("進捗（研究の途中経過）", labelProgress, gridProgress), 0, 1);
            body.Controls.Add(MakeGroup("文献（読んだ論文・資料）", labelLiterature, gridLiterature), 1, 1);

            var buttonClose = new Button
            {
                Text = "閉じる",
                Dock = DockStyle.Right,
                Width = 100
            };
            buttonClose.Click += (sender, e) => Close();
            var closeBar = new Panel { Dock = DockStyle.Fill, Height = 36 };
            closeBar.Controls.Add(buttonClose);

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 6,
                Padding = new Padding(8)
            };
            layout.RowStyles.Clear();
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 68F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, session.IsTeacher ? 36F : 0F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            layout.Controls.Add(title, 0, 0);
            layout.Controls.Add(help, 0, 1);
            layout.Controls.Add(labelPerson, 0, 2);
            layout.Controls.Add(picker, 0, 3);
            layout.Controls.Add(body, 0, 4);
            layout.Controls.Add(closeBar, 0, 5);
            Controls.Add(layout);

            Load += Form19_Load;
        }

        private void Form19_Load(object sender, EventArgs e)
        {
            if (!session.IsTeacher)
            {
                ShowCarte(session.StudentId);
                return;
            }

            LoadStudentChoices();
            labelPerson.Text = "教員: " + (string.IsNullOrEmpty(session.Name) ? session.LoginId : session.Name);
            ClearSections("学籍番号を選んでください。");
        }

        private void LoadStudentChoices()
        {
            loadingStudents = true;
            comboStudent.Items.Clear();
            try
            {
                if (!EnsureConnected())
                    return;

                var table = new DataTable();
                string sql = $@"
                    SELECT student_id, name
                    FROM personal_info
                    WHERE {PersonalInfoHelper.SqlStudentsOnly}
                    ORDER BY student_id";
                if (!Connector.TryTableReader(sql, table, out string error))
                {
                    labelPerson.Text = "学生一覧を読めません。 " + error;
                    return;
                }

                foreach (DataRow row in table.Rows)
                {
                    comboStudent.Items.Add(new StudentChoice
                    {
                        StudentId = Convert.ToString(row["student_id"]) ?? "",
                        Name = Convert.ToString(row["name"]) ?? ""
                    });
                }
            }
            finally
            {
                loadingStudents = false;
            }
        }

        private void ReloadSelected()
        {
            var choice = comboStudent.SelectedItem as StudentChoice;
            if (choice == null || string.IsNullOrWhiteSpace(choice.StudentId))
            {
                ClearSections("学籍番号を選んでください。");
                return;
            }

            ShowCarte(choice.StudentId);
        }

        private void ShowCarte(string studentId)
        {
            if (!EnsureConnected())
            {
                labelPerson.Text = "データベースに接続できません。";
                ClearSections("未接続");
                return;
            }

            StudentCarte carte = StudentCarteStore.Load(studentId);
            string name = string.IsNullOrEmpty(carte.Name) ? "（personal_info に氏名なし）" : carte.Name;
            labelPerson.Text = "学籍番号 " + carte.StudentId + "　" + name;

            BindSection(carte.Attendance, labelAttendance, gridAttendance);
            BindSection(carte.Duty, labelDuty, gridDuty);
            BindSection(carte.Progress, labelProgress, gridProgress);
            BindSection(carte.Literature, labelLiterature, gridLiterature);
        }

        private void ClearSections(string detail)
        {
            BindSection(Empty("出席", StudentCarteStore.AttendanceSources, detail), labelAttendance, gridAttendance);
            BindSection(Empty("日直", StudentCarteStore.DutySources, detail), labelDuty, gridDuty);
            BindSection(Empty("進捗", StudentCarteStore.ProgressSources, detail), labelProgress, gridProgress);
            BindSection(Empty("文献", StudentCarteStore.LiteratureSources, detail), labelLiterature, gridLiterature);
        }

        private static CarteSection Empty(string title, string sources, string detail)
        {
            return new CarteSection
            {
                Title = title,
                SourceTables = sources,
                Detail = detail,
                Rows = new DataTable()
            };
        }

        private static void BindSection(CarteSection section, Label status, DataGridView grid)
        {
            string source = "参照: " + (section.SourceTables ?? "");
            if (!string.IsNullOrEmpty(section.Error))
                status.Text = section.Error + "　" + source;
            else if (section.Count == 0)
                status.Text = "未登録" + DetailSuffix(section.Detail) + "　" + source;
            else
                status.Text = section.Count + "件" + DetailSuffix(section.Detail) + "　" + source;

            grid.DataSource = section.Count == 0 ? null : section.Rows;
            grid.Visible = section.Count > 0;
            if (section.Count > 0 && grid.Columns.Count > 0)
            {
                foreach (DataGridViewColumn column in grid.Columns)
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
        }

        private static string DetailSuffix(string detail)
        {
            if (string.IsNullOrEmpty(detail))
                return "";
            return "（" + detail + "）";
        }

        private static DataGridView MakeGrid()
        {
            return new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                Visible = false
            };
        }

        private static Label MakeStatus()
        {
            return new Label
            {
                Dock = DockStyle.Top,
                Height = 36,
                TextAlign = ContentAlignment.MiddleLeft,
                Text = "未登録"
            };
        }

        private static GroupBox MakeGroup(string title, Label status, DataGridView grid)
        {
            var group = new GroupBox
            {
                Text = title,
                Dock = DockStyle.Fill,
                Padding = new Padding(8, 8, 8, 4),
                Font = UiFonts.Get(10F, FontStyle.Bold)
            };
            status.Font = UiFonts.Get(9F);
            grid.Font = UiFonts.Get(9F);
            group.Controls.Add(grid);
            group.Controls.Add(status);
            return group;
        }

        private bool EnsureConnected()
        {
            if (Connector.IsConnected)
                return true;

            return Connector.Connect(
                settings.UserID,
                settings.PassWd,
                settings.DataBaseName,
                settings.ServerIP);
        }
    }
}
