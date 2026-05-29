namespace LabManager
{
    partial class Form9
    {
        private System.ComponentModel.IContainer components = null;

        // 日直スケジュールを表示するためのDataGridView
        private System.Windows.Forms.DataGridView dataGridViewDuty;

        // 日付入力用ラベルとDateTimePicker
        private System.Windows.Forms.Label labelDate;
        private System.Windows.Forms.DateTimePicker dateTimePickerDuty;

        // 学生選択用ラベルとComboBox
        private System.Windows.Forms.Label labelStudent;
        private System.Windows.Forms.ComboBox comboBoxStudent;

        // 追加・削除・閉じるボタン
        private System.Windows.Forms.Button buttonAddDuty;
        private System.Windows.Forms.Button buttonDeleteDuty;
        private System.Windows.Forms.Button buttonClose;

        /// <summary>
        /// リソースを解放する
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        /// <summary>
        /// Form9 の UI コンポーネント初期化
        /// </summary>
        private void InitializeComponent()
        {
            this.dataGridViewDuty = new System.Windows.Forms.DataGridView();
            this.labelDate = new System.Windows.Forms.Label();
            this.dateTimePickerDuty = new System.Windows.Forms.DateTimePicker();
            this.labelStudent = new System.Windows.Forms.Label();
            this.comboBoxStudent = new System.Windows.Forms.ComboBox();
            this.buttonAddDuty = new System.Windows.Forms.Button();
            this.buttonDeleteDuty = new System.Windows.Forms.Button();
            this.buttonClose = new System.Windows.Forms.Button();

            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDuty)).BeginInit();
            this.SuspendLayout();

            // 
            // dataGridViewDuty
            // 
            // 日直スケジュールを表示する表
            this.dataGridViewDuty.Location = new System.Drawing.Point(20, 20);
            this.dataGridViewDuty.Size = new System.Drawing.Size(500, 200);
            this.dataGridViewDuty.Name = "dataGridViewDuty";
            this.dataGridViewDuty.TabIndex = 0;

            // 
            // labelDate
            // 
            // 日付選択用のラベル
            this.labelDate.Location = new System.Drawing.Point(20, 240);
            this.labelDate.Size = new System.Drawing.Size(80, 20);
            this.labelDate.Text = "日付:";

            // 
            // dateTimePickerDuty
            // 
            // 日付をカレンダーから選択する
            this.dateTimePickerDuty.Location = new System.Drawing.Point(120, 240);
            this.dateTimePickerDuty.Size = new System.Drawing.Size(200, 20);
            this.dateTimePickerDuty.Name = "dateTimePickerDuty";
            this.dateTimePickerDuty.TabIndex = 1;

            // 
            // labelStudent
            // 
            // 学生選択用のラベル
            this.labelStudent.Location = new System.Drawing.Point(20, 280);
            this.labelStudent.Size = new System.Drawing.Size(80, 20);
            this.labelStudent.Text = "学生:";

            // 
            // comboBoxStudent
            // 
            // 学生をリストから選択するコンボボックス
            this.comboBoxStudent.Location = new System.Drawing.Point(120, 280);
            this.comboBoxStudent.Size = new System.Drawing.Size(200, 20);
            this.comboBoxStudent.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxStudent.Name = "comboBoxStudent";
            this.comboBoxStudent.TabIndex = 2;

            // 
            // buttonAddDuty
            // 
            // 日直を追加するボタン
            this.buttonAddDuty.Location = new System.Drawing.Point(350, 240);
            this.buttonAddDuty.Size = new System.Drawing.Size(80, 30);
            this.buttonAddDuty.Text = "追加";
            this.buttonAddDuty.Click += new System.EventHandler(this.buttonAddDuty_Click);

            // 
            // buttonDeleteDuty
            // 
            // 選択した日直を削除するボタン
            this.buttonDeleteDuty.Location = new System.Drawing.Point(350, 280);
            this.buttonDeleteDuty.Size = new System.Drawing.Size(80, 30);
            this.buttonDeleteDuty.Text = "削除";
            this.buttonDeleteDuty.Click += new System.EventHandler(this.buttonDeleteDuty_Click);

            // 
            // buttonClose
            // 
            // フォームを閉じるボタン
            this.buttonClose.Location = new System.Drawing.Point(440, 280);
            this.buttonClose.Size = new System.Drawing.Size(80, 30);
            this.buttonClose.Text = "閉じる";
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);

            // 
            // Form9
            // 
            // フォーム全体の設定
            this.ClientSize = new System.Drawing.Size(600, 350);
            this.Controls.Add(this.dataGridViewDuty);
            this.Controls.Add(this.labelDate);
            this.Controls.Add(this.dateTimePickerDuty);
            this.Controls.Add(this.labelStudent);
            this.Controls.Add(this.comboBoxStudent);
            this.Controls.Add(this.buttonAddDuty);
            this.Controls.Add(this.buttonDeleteDuty);
            this.Controls.Add(this.buttonClose);
            this.Name = "Form9";
            this.Text = "日直管理";

            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDuty)).EndInit();
            this.ResumeLayout(false);
        }
    }
}
