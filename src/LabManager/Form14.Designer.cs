namespace LabManager
{
    partial class Form14
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Label lblMonth;
        private System.Windows.Forms.TableLayoutPanel tableWeekdays;
        private System.Windows.Forms.TableLayoutPanel tableCalendar;
        private System.Windows.Forms.Label lblTodayHeader;
        private System.Windows.Forms.ListView listViewToday;
        private System.Windows.Forms.Timer timerRefresh;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lblMonth = new System.Windows.Forms.Label();
            this.tableWeekdays = new System.Windows.Forms.TableLayoutPanel();
            this.tableCalendar = new System.Windows.Forms.TableLayoutPanel();
            this.lblTodayHeader = new System.Windows.Forms.Label();
            this.listViewToday = new System.Windows.Forms.ListView();
            this.timerRefresh = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();

            this.lblMonth.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            this.tableWeekdays.ColumnCount = 7;
            this.tableWeekdays.RowCount = 1;
            for (int i = 0; i < 7; i++)
                this.tableWeekdays.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.28F));
            this.tableWeekdays.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));

            this.tableCalendar.ColumnCount = 7;
            this.tableCalendar.RowCount = 6;
            for (int i = 0; i < 7; i++)
                this.tableCalendar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.28F));
            for (int i = 0; i < 6; i++)
                this.tableCalendar.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66F));
            this.tableCalendar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            this.lblTodayHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.listViewToday.View = System.Windows.Forms.View.Details;
            this.listViewToday.FullRowSelect = true;
            this.listViewToday.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewToday.Columns.Add("時刻", 80);
            this.listViewToday.Columns.Add("予定", 300);

            this.timerRefresh.Tick += new System.EventHandler(this.timerRefresh_Tick);

            this.BackColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Form14";
            this.Text = "テレビ左画面";
            this.Load += new System.EventHandler(this.Form14_Load);
            this.ResumeLayout(false);
        }
    }
}
