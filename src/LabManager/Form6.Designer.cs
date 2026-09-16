namespace LabManager
{
    partial class Form6
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageSubmit = new System.Windows.Forms.TabPage();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tabPageView = new System.Windows.Forms.TabPage();
            this.textBoxDiaryContent = new System.Windows.Forms.TextBox();
            this.labelContent = new System.Windows.Forms.Label();
            this.dataGridViewDiaries = new System.Windows.Forms.DataGridView();
            this.buttonLoadDiaries = new System.Windows.Forms.Button();
            this.textBoxFilterStudent = new System.Windows.Forms.TextBox();
            this.labelFilterStudent = new System.Windows.Forms.Label();
            this.dateTimePickerTo = new System.Windows.Forms.DateTimePicker();
            this.dateTimePickerFrom = new System.Windows.Forms.DateTimePicker();
            this.labelDateTo = new System.Windows.Forms.Label();
            this.labelDateFrom = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPageSubmit.SuspendLayout();
            this.tabPageView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDiaries)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageSubmit);
            this.tabControl1.Controls.Add(this.tabPageView);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(634, 451);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPageSubmit
            // 
            this.tabPageSubmit.Controls.Add(this.textBox1);
            this.tabPageSubmit.Controls.Add(this.textBox2);
            this.tabPageSubmit.Controls.Add(this.button1);
            this.tabPageSubmit.Controls.Add(this.button2);
            this.tabPageSubmit.Controls.Add(this.label1);
            this.tabPageSubmit.Controls.Add(this.label2);
            this.tabPageSubmit.Controls.Add(this.label3);
            this.tabPageSubmit.Controls.Add(this.label4);
            this.tabPageSubmit.Controls.Add(this.textBox3);
            this.tabPageSubmit.Controls.Add(this.label5);
            this.tabPageSubmit.Location = new System.Drawing.Point(4, 22);
            this.tabPageSubmit.Name = "tabPageSubmit";
            this.tabPageSubmit.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSubmit.Size = new System.Drawing.Size(626, 425);
            this.tabPageSubmit.TabIndex = 0;
            this.tabPageSubmit.Text = "提出";
            this.tabPageSubmit.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(17, 119);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(111, 19);
            this.textBox1.TabIndex = 0;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(153, 119);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(75, 19);
            this.textBox2.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(483, 144);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "提出";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(386, 144);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "clear";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("MS UI Gothic", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label1.Location = new System.Drawing.Point(12, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(362, 27);
            this.label1.TabIndex = 4;
            this.label1.Text = "研究進捗管理ー日誌提出Form";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("MS UI Gothic", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label2.Location = new System.Drawing.Point(212, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(376, 36);
            this.label2.TabIndex = 5;
            this.label2.Text = "日直者は本日行った研究活動について記録を行う。\r\n研究に関する不明点などを記入してもよい。";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 104);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "日誌タイトル";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 158);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "本文";
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(17, 173);
            this.textBox3.Multiline = true;
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(541, 183);
            this.textBox3.TabIndex = 2;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(151, 104);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 9;
            this.label5.Text = "学籍番号";
            // 
            // tabPageView
            // 
            this.tabPageView.Controls.Add(this.textBoxDiaryContent);
            this.tabPageView.Controls.Add(this.labelContent);
            this.tabPageView.Controls.Add(this.dataGridViewDiaries);
            this.tabPageView.Controls.Add(this.buttonLoadDiaries);
            this.tabPageView.Controls.Add(this.textBoxFilterStudent);
            this.tabPageView.Controls.Add(this.labelFilterStudent);
            this.tabPageView.Controls.Add(this.dateTimePickerTo);
            this.tabPageView.Controls.Add(this.dateTimePickerFrom);
            this.tabPageView.Controls.Add(this.labelDateTo);
            this.tabPageView.Controls.Add(this.labelDateFrom);
            this.tabPageView.Location = new System.Drawing.Point(4, 22);
            this.tabPageView.Name = "tabPageView";
            this.tabPageView.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageView.Size = new System.Drawing.Size(626, 425);
            this.tabPageView.TabIndex = 1;
            this.tabPageView.Text = "閲覧";
            this.tabPageView.UseVisualStyleBackColor = true;
            // 
            // textBoxDiaryContent
            // 
            this.textBoxDiaryContent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDiaryContent.Location = new System.Drawing.Point(17, 298);
            this.textBoxDiaryContent.Multiline = true;
            this.textBoxDiaryContent.Name = "textBoxDiaryContent";
            this.textBoxDiaryContent.ReadOnly = true;
            this.textBoxDiaryContent.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxDiaryContent.Size = new System.Drawing.Size(593, 112);
            this.textBoxDiaryContent.TabIndex = 9;
            // 
            // labelContent
            // 
            this.labelContent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelContent.AutoSize = true;
            this.labelContent.Location = new System.Drawing.Point(15, 283);
            this.labelContent.Name = "labelContent";
            this.labelContent.Size = new System.Drawing.Size(29, 12);
            this.labelContent.TabIndex = 8;
            this.labelContent.Text = "本文";
            // 
            // dataGridViewDiaries
            // 
            this.dataGridViewDiaries.AllowUserToAddRows = false;
            this.dataGridViewDiaries.AllowUserToDeleteRows = false;
            this.dataGridViewDiaries.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewDiaries.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewDiaries.Location = new System.Drawing.Point(17, 72);
            this.dataGridViewDiaries.MultiSelect = false;
            this.dataGridViewDiaries.Name = "dataGridViewDiaries";
            this.dataGridViewDiaries.ReadOnly = true;
            this.dataGridViewDiaries.RowHeadersVisible = false;
            this.dataGridViewDiaries.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewDiaries.Size = new System.Drawing.Size(593, 200);
            this.dataGridViewDiaries.TabIndex = 7;
            this.dataGridViewDiaries.SelectionChanged += new System.EventHandler(this.dataGridViewDiaries_SelectionChanged);
            // 
            // buttonLoadDiaries
            // 
            this.buttonLoadDiaries.Location = new System.Drawing.Point(535, 16);
            this.buttonLoadDiaries.Name = "buttonLoadDiaries";
            this.buttonLoadDiaries.Size = new System.Drawing.Size(75, 23);
            this.buttonLoadDiaries.TabIndex = 6;
            this.buttonLoadDiaries.Text = "表示";
            this.buttonLoadDiaries.UseVisualStyleBackColor = true;
            this.buttonLoadDiaries.Click += new System.EventHandler(this.buttonLoadDiaries_Click);
            // 
            // textBoxFilterStudent
            // 
            this.textBoxFilterStudent.Location = new System.Drawing.Point(417, 18);
            this.textBoxFilterStudent.Name = "textBoxFilterStudent";
            this.textBoxFilterStudent.Size = new System.Drawing.Size(75, 19);
            this.textBoxFilterStudent.TabIndex = 5;
            // 
            // labelFilterStudent
            // 
            this.labelFilterStudent.AutoSize = true;
            this.labelFilterStudent.Location = new System.Drawing.Point(358, 21);
            this.labelFilterStudent.Name = "labelFilterStudent";
            this.labelFilterStudent.Size = new System.Drawing.Size(53, 12);
            this.labelFilterStudent.TabIndex = 4;
            this.labelFilterStudent.Text = "学籍番号";
            // 
            // dateTimePickerTo
            // 
            this.dateTimePickerTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePickerTo.Location = new System.Drawing.Point(223, 18);
            this.dateTimePickerTo.Name = "dateTimePickerTo";
            this.dateTimePickerTo.Size = new System.Drawing.Size(110, 19);
            this.dateTimePickerTo.TabIndex = 3;
            // 
            // dateTimePickerFrom
            // 
            this.dateTimePickerFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePickerFrom.Location = new System.Drawing.Point(67, 18);
            this.dateTimePickerFrom.Name = "dateTimePickerFrom";
            this.dateTimePickerFrom.Size = new System.Drawing.Size(110, 19);
            this.dateTimePickerFrom.TabIndex = 2;
            // 
            // labelDateTo
            // 
            this.labelDateTo.AutoSize = true;
            this.labelDateTo.Location = new System.Drawing.Point(192, 21);
            this.labelDateTo.Name = "labelDateTo";
            this.labelDateTo.Size = new System.Drawing.Size(17, 12);
            this.labelDateTo.TabIndex = 1;
            this.labelDateTo.Text = "〜";
            // 
            // labelDateFrom
            // 
            this.labelDateFrom.AutoSize = true;
            this.labelDateFrom.Location = new System.Drawing.Point(15, 21);
            this.labelDateFrom.Name = "labelDateFrom";
            this.labelDateFrom.Size = new System.Drawing.Size(41, 12);
            this.labelDateFrom.TabIndex = 0;
            this.labelDateFrom.Text = "期間：";
            // 
            // Form6
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(634, 451);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form6";
            this.Text = "日誌Form";
            this.tabControl1.ResumeLayout(false);
            this.tabPageSubmit.ResumeLayout(false);
            this.tabPageSubmit.PerformLayout();
            this.tabPageView.ResumeLayout(false);
            this.tabPageView.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDiaries)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageSubmit;
        private System.Windows.Forms.TabPage tabPageView;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label labelDateFrom;
        private System.Windows.Forms.Label labelDateTo;
        private System.Windows.Forms.DateTimePicker dateTimePickerFrom;
        private System.Windows.Forms.DateTimePicker dateTimePickerTo;
        private System.Windows.Forms.Label labelFilterStudent;
        private System.Windows.Forms.TextBox textBoxFilterStudent;
        private System.Windows.Forms.Button buttonLoadDiaries;
        private System.Windows.Forms.DataGridView dataGridViewDiaries;
        private System.Windows.Forms.Label labelContent;
        private System.Windows.Forms.TextBox textBoxDiaryContent;
    }
}
