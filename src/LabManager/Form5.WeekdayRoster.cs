using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace LabManager
{
    public partial class Form5
    {
        private readonly ComboBox[,] weekdaySlotCombos = new ComboBox[5, 2];
        private Button btnSaveWeekdayRoster;
        private Label lblRosterSummary;
        private TableLayoutPanel weekdayRosterTable;
        private List<StudentListItem> studentChoices = new List<StudentListItem>();

        private static readonly DayOfWeek[] WeekdayOrder =
        {
            DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday,
            DayOfWeek.Thursday, DayOfWeek.Friday
        };

        private static readonly string[] WeekdayLabels = { "月曜", "火曜", "水曜", "木曜", "金曜" };

        private void BuildWeekdayRosterUi()
        {
            label3.Visible = false;
            label6.Text = "曜日ごとに担当者を選択（各曜日最大2名）";

            textBoxM.Visible = false;
            textBoxT.Visible = false;
            textBoxW.Visible = false;
            textBoxTh.Visible = false;
            textBoxF.Visible = false;
            label7.Visible = false;
            label8.Visible = false;
            label9.Visible = false;
            label10.Visible = false;
            label11.Visible = false;

            panel4.Controls.Clear();
            panel4.BorderStyle = BorderStyle.FixedSingle;

            weekdayRosterTable = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 5,
                Padding = new Padding(4)
            };
            weekdayRosterTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 44F));
            weekdayRosterTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            weekdayRosterTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            for (int i = 0; i < 5; i++)
                weekdayRosterTable.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));

            for (int i = 0; i < WeekdayOrder.Length; i++)
            {
                weekdayRosterTable.Controls.Add(new Label
                {
                    Text = WeekdayLabels[i],
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Font = UiFonts.Get(9F, FontStyle.Bold)
                }, 0, i);

                for (int slot = 0; slot < 2; slot++)
                {
                    var combo = new ComboBox
                    {
                        Dock = DockStyle.Fill,
                        DropDownStyle = ComboBoxStyle.DropDownList,
                        Font = UiFonts.Get(9F),
                        Margin = new Padding(2),
                        Tag = WeekdayOrder[i]
                    };
                    weekdaySlotCombos[i, slot] = combo;
                    weekdayRosterTable.Controls.Add(combo, slot + 1, i);
                }
            }

            panel4.Controls.Add(weekdayRosterTable);

            btnSaveWeekdayRoster = new Button
            {
                Text = "担当を保存",
                Size = new Size(100, 28),
                Font = UiFonts.Get(9F, FontStyle.Bold)
            };
            btnSaveWeekdayRoster.Click += btnSaveWeekdayRoster_Click;

            lblRosterSummary = new Label
            {
                BorderStyle = BorderStyle.FixedSingle,
                Font = UiFonts.Get(9F),
                Text = "登録担当:\r\n（未設定）"
            };

            panel2.Controls.Add(btnSaveWeekdayRoster);
            panel2.Controls.Add(lblRosterSummary);

            panel2.Resize += (sender, e) => LayoutPanel2Controls();
            LayoutPanel2Controls();
        }

        private void LayoutPanel2Controls()
        {
            if (panel2 == null || btnSaveWeekdayRoster == null)
                return;

            int margin = 12;
            int width = Math.Max(400, panel2.ClientSize.Width - margin * 2);
            int y = margin;

            label16.Font = UiFonts.Get(13F, FontStyle.Bold);
            label16.Location = new Point(margin, y);
            label16.Size = new Size(width, 24);
            y += 28;

            labelMonth.Location = new Point(margin, y + 2);
            comboBoxMonth.Location = new Point(margin + 100, y);
            comboBoxMonth.Size = new Size(90, 22);
            buttonLoadFromGoogleForm.Location = new Point(margin + 200, y - 1);
            buttonLoadFromGoogleForm.Size = new Size(90, 24);
            y += 30;

            label5.Location = new Point(margin, y);
            dateTimePicker1.Location = new Point(margin, y + 18);
            dateTimePicker1.Size = new Size(160, 22);
            label4.Location = new Point(margin + 200, y);
            dateTimePicker2.Location = new Point(margin + 200, y + 18);
            dateTimePicker2.Size = new Size(160, 22);
            y += 48;

            label6.Location = new Point(margin, y);
            label6.Size = new Size(width, 16);
            y += 20;

            int rosterHeight = 170;
            int summaryWidth = Math.Max(180, width / 3);
            int rosterWidth = width - summaryWidth - 8;
            panel4.Location = new Point(margin, y);
            panel4.Size = new Size(rosterWidth, rosterHeight);

            lblRosterSummary.Location = new Point(margin + rosterWidth + 8, y);
            lblRosterSummary.Size = new Size(summaryWidth, rosterHeight);
            y += rosterHeight + 10;

            btnSaveWeekdayRoster.Location = new Point(margin, y);
            button7.Location = new Point(margin + 112, y);
            button7.Size = new Size(90, 28);
            y += 36;

            labelNote.Location = new Point(margin, y);
            y += 18;

            int noteHeight = Math.Max(80, panel2.ClientSize.Height - y - margin);
            dataGridViewNote.Location = new Point(margin, y);
            dataGridViewNote.Size = new Size(width, noteHeight);
        }

        private void InitWeekdayRosterPanel()
        {
            studentChoices = DutyWeekdayRosterStore.LoadAllStudents(out string errorMessage);
            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                lblRosterSummary.Text = errorMessage;
                return;
            }

            for (int i = 0; i < WeekdayOrder.Length; i++)
            {
                for (int slot = 0; slot < 2; slot++)
                {
                    ComboBox combo = weekdaySlotCombos[i, slot];
                    combo.DataSource = null;
                    combo.DisplayMember = "Display";
                    combo.ValueMember = "StudentId";
                    combo.DataSource = studentChoices.ToList();
                }
            }

            LoadWeekdayRosterIntoUi();
        }

        private void LoadWeekdayRosterIntoUi()
        {
            var roster = DutyWeekdayRosterStore.LoadRoster(out string errorMessage);
            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                lblRosterSummary.Text = errorMessage;
                return;
            }

            for (int i = 0; i < WeekdayOrder.Length; i++)
            {
                DayOfWeek dow = WeekdayOrder[i];
                roster.TryGetValue(dow, out List<string> ids);
                if (ids == null)
                    ids = new List<string>();

                for (int slot = 0; slot < 2; slot++)
                {
                    string selectedId = slot < ids.Count ? ids[slot] : "";
                    SelectStudentInCombo(weekdaySlotCombos[i, slot], selectedId);
                }
            }

            UpdateRosterSummaryLabel(roster);
        }

        private void SelectStudentInCombo(ComboBox combo, string studentId)
        {
            if (combo.DataSource == null)
                return;

            for (int i = 0; i < combo.Items.Count; i++)
            {
                if (combo.Items[i] is StudentListItem item &&
                    string.Equals(item.StudentId, studentId ?? "", StringComparison.Ordinal))
                {
                    combo.SelectedIndex = i;
                    return;
                }
            }

            combo.SelectedIndex = 0;
        }

        private Dictionary<DayOfWeek, List<string>> GetWeekdayRosterFromUi()
        {
            var roster = new Dictionary<DayOfWeek, List<string>>();
            for (int i = 0; i < WeekdayOrder.Length; i++)
            {
                var ids = new List<string>();
                for (int slot = 0; slot < 2; slot++)
                {
                    if (weekdaySlotCombos[i, slot].SelectedItem is StudentListItem item &&
                        !string.IsNullOrWhiteSpace(item.StudentId))
                    {
                        if (!ids.Contains(item.StudentId))
                            ids.Add(item.StudentId);
                    }
                }
                roster[WeekdayOrder[i]] = ids;
            }

            return roster;
        }

        private void UpdateRosterSummaryLabel(Dictionary<DayOfWeek, List<string>> roster)
        {
            lblRosterSummary.Text = "登録担当:\r\n" +
                DutyWeekdayRosterStore.FormatRosterSummary(roster, studentChoices);
        }

        private void btnSaveWeekdayRoster_Click(object sender, EventArgs e)
        {
            var roster = GetWeekdayRosterFromUi();
            if (!DutyWeekdayRosterStore.SaveRoster(roster, out string errorMessage))
            {
                MessageBox.Show(errorMessage ?? "担当の保存に失敗しました。", "日直管理");
                return;
            }

            UpdateRosterSummaryLabel(roster);
            MessageBox.Show("曜日別担当を保存しました。", "日直管理");
        }
    }
}
