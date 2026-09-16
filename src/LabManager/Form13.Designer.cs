namespace LabManager
{
    partial class Form13
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.DataGridView dataGridViewDuty;
        private System.Windows.Forms.DataGridView dataGridViewLog;
        private System.Windows.Forms.Label labelFilterDate;
        private System.Windows.Forms.DateTimePicker dateTimePickerFilter;
        private System.Windows.Forms.Button buttonReload;
        private System.Windows.Forms.Label labelStudent;
        private System.Windows.Forms.ComboBox comboBoxStudent;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.ComboBox comboBoxStatus;
        private System.Windows.Forms.Label labelType;
        private System.Windows.Forms.ComboBox comboBoxType;
        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.Button buttonUpdate;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Label labelDutyList;
        private System.Windows.Forms.Label labelLog;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.dataGridViewDuty = new System.Windows.Forms.DataGridView();
            this.dataGridViewLog = new System.Windows.Forms.DataGridView();
            this.labelFilterDate = new System.Windows.Forms.Label();
            this.dateTimePickerFilter = new System.Windows.Forms.DateTimePicker();
            this.buttonReload = new System.Windows.Forms.Button();
            this.labelStudent = new System.Windows.Forms.Label();
            this.comboBoxStudent = new System.Windows.Forms.ComboBox();
            this.labelStatus = new System.Windows.Forms.Label();
            this.comboBoxStatus = new System.Windows.Forms.ComboBox();
            this.labelType = new System.Windows.Forms.Label();
            this.comboBoxType = new System.Windows.Forms.ComboBox();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.buttonUpdate = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonClose = new System.Windows.Forms.Button();
            this.labelDutyList = new System.Windows.Forms.Label();
            this.labelLog = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDuty)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewLog)).BeginInit();
            this.SuspendLayout();
            //
            // labelDutyList
            //
            this.labelDutyList.Location = new System.Drawing.Point(12, 12);
            this.labelDutyList.Size = new System.Drawing.Size(300, 20);
            this.labelDutyList.Text = "出席状況データ";
            //
            // dataGridViewDuty
            //
            this.dataGridViewDuty.Location = new System.Drawing.Point(12, 36);
            this.dataGridViewDuty.Size = new System.Drawing.Size(760, 180);
            this.dataGridViewDuty.ReadOnly = true;
            this.dataGridViewDuty.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewDuty.MultiSelect = false;
            this.dataGridViewDuty.RowHeadersVisible = false;
            this.dataGridViewDuty.SelectionChanged += new System.EventHandler(this.dataGridViewDuty_SelectionChanged);
            //
            // labelFilterDate
            //
            this.labelFilterDate.Location = new System.Drawing.Point(12, 228);
            this.labelFilterDate.Size = new System.Drawing.Size(60, 20);
            this.labelFilterDate.Text = "日付:";
            //
            // dateTimePickerFilter
            //
            this.dateTimePickerFilter.Location = new System.Drawing.Point(72, 224);
            this.dateTimePickerFilter.Size = new System.Drawing.Size(160, 22);
            //
            // buttonReload
            //
            this.buttonReload.Location = new System.Drawing.Point(248, 222);
            this.buttonReload.Size = new System.Drawing.Size(80, 26);
            this.buttonReload.Text = "表示";
            this.buttonReload.Click += new System.EventHandler(this.buttonReload_Click);
            //
            // labelStudent
            //
            this.labelStudent.Location = new System.Drawing.Point(12, 262);
            this.labelStudent.Size = new System.Drawing.Size(80, 20);
            this.labelStudent.Text = "学籍番号:";
            //
            // comboBoxStudent
            //
            this.comboBoxStudent.Location = new System.Drawing.Point(92, 258);
            this.comboBoxStudent.Size = new System.Drawing.Size(200, 22);
            this.comboBoxStudent.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            //
            // labelStatus
            //
            this.labelStatus.Location = new System.Drawing.Point(310, 262);
            this.labelStatus.Size = new System.Drawing.Size(80, 20);
            this.labelStatus.Text = "出席状況:";
            //
            // comboBoxStatus
            //
            this.comboBoxStatus.Location = new System.Drawing.Point(390, 258);
            this.comboBoxStatus.Size = new System.Drawing.Size(120, 22);
            this.comboBoxStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            //
            // labelType
            //
            this.labelType.Location = new System.Drawing.Point(520, 262);
            this.labelType.Size = new System.Drawing.Size(40, 20);
            this.labelType.Text = "種類:";
            //
            // comboBoxType
            //
            this.comboBoxType.Location = new System.Drawing.Point(560, 258);
            this.comboBoxType.Size = new System.Drawing.Size(100, 22);
            this.comboBoxType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            //
            // buttonAdd
            //
            this.buttonAdd.Location = new System.Drawing.Point(12, 296);
            this.buttonAdd.Size = new System.Drawing.Size(80, 28);
            this.buttonAdd.Text = "追加";
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            //
            // buttonUpdate
            //
            this.buttonUpdate.Location = new System.Drawing.Point(100, 296);
            this.buttonUpdate.Size = new System.Drawing.Size(80, 28);
            this.buttonUpdate.Text = "更新";
            this.buttonUpdate.Click += new System.EventHandler(this.buttonUpdate_Click);
            //
            // buttonDelete
            //
            this.buttonDelete.Location = new System.Drawing.Point(188, 296);
            this.buttonDelete.Size = new System.Drawing.Size(80, 28);
            this.buttonDelete.Text = "削除";
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            //
            // buttonClose
            //
            this.buttonClose.Location = new System.Drawing.Point(692, 296);
            this.buttonClose.Size = new System.Drawing.Size(80, 28);
            this.buttonClose.Text = "閉じる";
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            //
            // labelLog
            //
            this.labelLog.Location = new System.Drawing.Point(12, 336);
            this.labelLog.Size = new System.Drawing.Size(300, 20);
            this.labelLog.Text = "編集ログ（不正防止）";
            //
            // dataGridViewLog
            //
            this.dataGridViewLog.Location = new System.Drawing.Point(12, 360);
            this.dataGridViewLog.Size = new System.Drawing.Size(760, 160);
            this.dataGridViewLog.ReadOnly = true;
            this.dataGridViewLog.RowHeadersVisible = false;
            //
            // Form13
            //
            this.ClientSize = new System.Drawing.Size(784, 532);
            this.Controls.Add(this.labelDutyList);
            this.Controls.Add(this.dataGridViewDuty);
            this.Controls.Add(this.labelFilterDate);
            this.Controls.Add(this.dateTimePickerFilter);
            this.Controls.Add(this.buttonReload);
            this.Controls.Add(this.labelStudent);
            this.Controls.Add(this.comboBoxStudent);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.comboBoxStatus);
            this.Controls.Add(this.labelType);
            this.Controls.Add(this.comboBoxType);
            this.Controls.Add(this.buttonAdd);
            this.Controls.Add(this.buttonUpdate);
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.labelLog);
            this.Controls.Add(this.dataGridViewLog);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "Form13";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "出席状況編集";
            this.Load += new System.EventHandler(this.Form13_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDuty)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewLog)).EndInit();
            this.ResumeLayout(false);
        }
    }
}
