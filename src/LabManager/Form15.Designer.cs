namespace LabManager
{
    partial class Form15
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel topPanel;
        private System.Windows.Forms.Panel bottomPanel;
        private System.Windows.Forms.TableLayoutPanel mainTable;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblDate;
        private System.Windows.Forms.DateTimePicker datePicker;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblKpi;
        private System.Windows.Forms.GroupBox groupAttendance;
        private System.Windows.Forms.DataGridView gridAttendance;
        private System.Windows.Forms.GroupBox groupCalendar;
        private System.Windows.Forms.DataGridView gridCalendar;
        private System.Windows.Forms.GroupBox groupDuty;
        private System.Windows.Forms.TableLayoutPanel tableDuty;
        private System.Windows.Forms.DataGridView gridDuty;
        private System.Windows.Forms.Label lblRoster;
        private System.Windows.Forms.GroupBox groupDiary;
        private System.Windows.Forms.TableLayoutPanel tableDiary;
        private System.Windows.Forms.DataGridView gridDiary;
        private System.Windows.Forms.TextBox txtDiary;
        private System.Windows.Forms.Button btnOpenDuty;
        private System.Windows.Forms.Button btnOpenCal;
        private System.Windows.Forms.Button btnOpenDiary;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.topPanel = new System.Windows.Forms.Panel();
            this.bottomPanel = new System.Windows.Forms.Panel();
            this.mainTable = new System.Windows.Forms.TableLayoutPanel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblDate = new System.Windows.Forms.Label();
            this.datePicker = new System.Windows.Forms.DateTimePicker();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblKpi = new System.Windows.Forms.Label();
            this.groupAttendance = new System.Windows.Forms.GroupBox();
            this.gridAttendance = new System.Windows.Forms.DataGridView();
            this.groupCalendar = new System.Windows.Forms.GroupBox();
            this.gridCalendar = new System.Windows.Forms.DataGridView();
            this.groupDuty = new System.Windows.Forms.GroupBox();
            this.tableDuty = new System.Windows.Forms.TableLayoutPanel();
            this.gridDuty = new System.Windows.Forms.DataGridView();
            this.lblRoster = new System.Windows.Forms.Label();
            this.groupDiary = new System.Windows.Forms.GroupBox();
            this.tableDiary = new System.Windows.Forms.TableLayoutPanel();
            this.gridDiary = new System.Windows.Forms.DataGridView();
            this.txtDiary = new System.Windows.Forms.TextBox();
            this.btnOpenDuty = new System.Windows.Forms.Button();
            this.btnOpenCal = new System.Windows.Forms.Button();
            this.btnOpenDiary = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.gridAttendance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridCalendar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridDuty)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridDiary)).BeginInit();
            this.topPanel.SuspendLayout();
            this.bottomPanel.SuspendLayout();
            this.mainTable.SuspendLayout();
            this.groupAttendance.SuspendLayout();
            this.groupCalendar.SuspendLayout();
            this.groupDuty.SuspendLayout();
            this.tableDuty.SuspendLayout();
            this.groupDiary.SuspendLayout();
            this.tableDiary.SuspendLayout();
            this.SuspendLayout();
            //
            // topPanel
            //
            this.topPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.topPanel.Height = 92;
            this.topPanel.Padding = new System.Windows.Forms.Padding(8, 6, 8, 4);
            this.topPanel.Controls.Add(this.lblKpi);
            this.topPanel.Controls.Add(this.btnClose);
            this.topPanel.Controls.Add(this.lblStatus);
            this.topPanel.Controls.Add(this.btnRefresh);
            this.topPanel.Controls.Add(this.datePicker);
            this.topPanel.Controls.Add(this.lblDate);
            this.topPanel.Controls.Add(this.lblTitle);
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Yu Gothic UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(8, 8);
            this.lblTitle.Text = "まとめページ";
            //
            // lblDate
            //
            this.lblDate.AutoSize = true;
            this.lblDate.Location = new System.Drawing.Point(168, 14);
            this.lblDate.Text = "対象日:";
            //
            // datePicker
            //
            this.datePicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.datePicker.Location = new System.Drawing.Point(220, 10);
            this.datePicker.Size = new System.Drawing.Size(130, 22);
            this.datePicker.ValueChanged += new System.EventHandler(this.datePicker_ValueChanged);
            //
            // btnRefresh
            //
            this.btnRefresh.Location = new System.Drawing.Point(360, 8);
            this.btnRefresh.Size = new System.Drawing.Size(80, 26);
            this.btnRefresh.Text = "更新";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            //
            // lblStatus
            //
            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStatus.Location = new System.Drawing.Point(450, 12);
            this.lblStatus.Size = new System.Drawing.Size(520, 20);
            this.lblStatus.Text = "接続: --";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // btnClose
            //
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(1004, 8);
            this.btnClose.Size = new System.Drawing.Size(80, 26);
            this.btnClose.Text = "閉じる";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            //
            // lblKpi
            //
            this.lblKpi.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.lblKpi.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblKpi.Location = new System.Drawing.Point(8, 42);
            this.lblKpi.Size = new System.Drawing.Size(1076, 42);
            this.lblKpi.Text = "授業日: --　予定: --　在室 -- 人　来室 -- 人　日直 --　日誌 -- 件";
            this.lblKpi.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblKpi.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
            //
            // mainTable
            //
            this.mainTable.ColumnCount = 2;
            this.mainTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.mainTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.mainTable.RowCount = 2;
            this.mainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 55F));
            this.mainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 45F));
            this.mainTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTable.Padding = new System.Windows.Forms.Padding(6);
            this.mainTable.Controls.Add(this.groupAttendance, 0, 0);
            this.mainTable.Controls.Add(this.groupCalendar, 1, 0);
            this.mainTable.Controls.Add(this.groupDuty, 0, 1);
            this.mainTable.Controls.Add(this.groupDiary, 1, 1);
            //
            // groupAttendance
            //
            this.groupAttendance.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupAttendance.Padding = new System.Windows.Forms.Padding(6);
            this.groupAttendance.Text = "在席（学籍番号順）";
            this.groupAttendance.Controls.Add(this.gridAttendance);
            //
            // gridAttendance
            //
            this.gridAttendance.Dock = System.Windows.Forms.DockStyle.Fill;
            ConfigureGrid(this.gridAttendance);
            this.gridAttendance.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.gridAttendance_CellFormatting);
            //
            // groupCalendar
            //
            this.groupCalendar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupCalendar.Padding = new System.Windows.Forms.Padding(6);
            this.groupCalendar.Text = "カレンダー（対象日から2週間）";
            this.groupCalendar.Controls.Add(this.gridCalendar);
            //
            // gridCalendar
            //
            this.gridCalendar.Dock = System.Windows.Forms.DockStyle.Fill;
            ConfigureGrid(this.gridCalendar);
            //
            // groupDuty
            //
            this.groupDuty.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupDuty.Padding = new System.Windows.Forms.Padding(6);
            this.groupDuty.Text = "日直";
            this.groupDuty.Controls.Add(this.tableDuty);
            //
            // tableDuty
            //
            this.tableDuty.ColumnCount = 1;
            this.tableDuty.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableDuty.RowCount = 2;
            this.tableDuty.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 62F));
            this.tableDuty.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 38F));
            this.tableDuty.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableDuty.Controls.Add(this.gridDuty, 0, 0);
            this.tableDuty.Controls.Add(this.lblRoster, 0, 1);
            //
            // gridDuty
            //
            this.gridDuty.Dock = System.Windows.Forms.DockStyle.Fill;
            ConfigureGrid(this.gridDuty);
            this.gridDuty.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.gridDuty_CellFormatting);
            //
            // lblRoster
            //
            this.lblRoster.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblRoster.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblRoster.Padding = new System.Windows.Forms.Padding(6);
            this.lblRoster.Text = "曜日担当: --";
            //
            // groupDiary
            //
            this.groupDiary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupDiary.Padding = new System.Windows.Forms.Padding(6);
            this.groupDiary.Text = "日誌（直近30日）";
            this.groupDiary.Controls.Add(this.tableDiary);
            //
            // tableDiary
            //
            this.tableDiary.ColumnCount = 1;
            this.tableDiary.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableDiary.RowCount = 2;
            this.tableDiary.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 58F));
            this.tableDiary.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 42F));
            this.tableDiary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableDiary.Controls.Add(this.gridDiary, 0, 0);
            this.tableDiary.Controls.Add(this.txtDiary, 0, 1);
            //
            // gridDiary
            //
            this.gridDiary.Dock = System.Windows.Forms.DockStyle.Fill;
            ConfigureGrid(this.gridDiary);
            this.gridDiary.SelectionChanged += new System.EventHandler(this.gridDiary_SelectionChanged);
            //
            // txtDiary
            //
            this.txtDiary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtDiary.Multiline = true;
            this.txtDiary.ReadOnly = true;
            this.txtDiary.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDiary.WordWrap = true;
            //
            // bottomPanel
            //
            this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottomPanel.Height = 48;
            this.bottomPanel.Padding = new System.Windows.Forms.Padding(8, 8, 8, 8);
            this.bottomPanel.Controls.Add(this.btnOpenDiary);
            this.bottomPanel.Controls.Add(this.btnOpenCal);
            this.bottomPanel.Controls.Add(this.btnOpenDuty);
            //
            // btnOpenDuty
            //
            this.btnOpenDuty.Location = new System.Drawing.Point(8, 10);
            this.btnOpenDuty.Size = new System.Drawing.Size(140, 28);
            this.btnOpenDuty.Text = "日直管理";
            this.btnOpenDuty.Click += new System.EventHandler(this.btnOpenDuty_Click);
            //
            // btnOpenCal
            //
            this.btnOpenCal.Location = new System.Drawing.Point(154, 10);
            this.btnOpenCal.Size = new System.Drawing.Size(160, 28);
            this.btnOpenCal.Text = "CalendarEditor";
            this.btnOpenCal.Click += new System.EventHandler(this.btnOpenCal_Click);
            //
            // btnOpenDiary
            //
            this.btnOpenDiary.Location = new System.Drawing.Point(320, 10);
            this.btnOpenDiary.Size = new System.Drawing.Size(140, 28);
            this.btnOpenDiary.Text = "日誌";
            this.btnOpenDiary.Click += new System.EventHandler(this.btnOpenDiary_Click);
            //
            // Form15
            //
            this.ClientSize = new System.Drawing.Size(1100, 720);
            this.MinimumSize = new System.Drawing.Size(900, 600);
            this.Controls.Add(this.mainTable);
            this.Controls.Add(this.bottomPanel);
            this.Controls.Add(this.topPanel);
            this.Name = "Form15";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "15:まとめページ";
            this.Load += new System.EventHandler(this.Form15_Load);
            this.topPanel.ResumeLayout(false);
            this.topPanel.PerformLayout();
            this.bottomPanel.ResumeLayout(false);
            this.mainTable.ResumeLayout(false);
            this.groupAttendance.ResumeLayout(false);
            this.groupCalendar.ResumeLayout(false);
            this.groupDuty.ResumeLayout(false);
            this.tableDuty.ResumeLayout(false);
            this.groupDiary.ResumeLayout(false);
            this.tableDiary.ResumeLayout(false);
            this.tableDiary.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridAttendance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridCalendar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridDuty)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridDiary)).EndInit();
            this.ResumeLayout(false);
        }

        private static void ConfigureGrid(System.Windows.Forms.DataGridView grid)
        {
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToResizeRows = false;
            grid.ReadOnly = true;
            grid.MultiSelect = false;
            grid.RowHeadersVisible = false;
            grid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            grid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.None;
            grid.BackgroundColor = System.Drawing.Color.White;
            grid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        }
    }
}
