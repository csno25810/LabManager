namespace LabManager
{
    partial class Form11
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageAdd = new System.Windows.Forms.TabPage();
            this.buttonRegister = new System.Windows.Forms.Button();
            this.textBoxMail = new System.Windows.Forms.TextBox();
            this.textBoxGivenName = new System.Windows.Forms.TextBox();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.textBoxStudentId = new System.Windows.Forms.TextBox();
            this.labelMail = new System.Windows.Forms.Label();
            this.labelGivenName = new System.Windows.Forms.Label();
            this.labelName = new System.Windows.Forms.Label();
            this.labelStudentId = new System.Windows.Forms.Label();
            this.labelAddTitle = new System.Windows.Forms.Label();
            this.labelNameHint = new System.Windows.Forms.Label();
            this.tabPageDelete = new System.Windows.Forms.TabPage();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.dataGridViewStudents = new System.Windows.Forms.DataGridView();
            this.buttonSearch = new System.Windows.Forms.Button();
            this.textBoxSearchValue = new System.Windows.Forms.TextBox();
            this.comboBoxSearchField = new System.Windows.Forms.ComboBox();
            this.labelSearch = new System.Windows.Forms.Label();
            this.labelDeleteTitle = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPageAdd.SuspendLayout();
            this.tabPageDelete.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewStudents)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageAdd);
            this.tabControl1.Controls.Add(this.tabPageDelete);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(584, 361);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPageAdd
            // 
            this.tabPageAdd.Controls.Add(this.buttonRegister);
            this.tabPageAdd.Controls.Add(this.labelNameHint);
            this.tabPageAdd.Controls.Add(this.textBoxMail);
            this.tabPageAdd.Controls.Add(this.textBoxGivenName);
            this.tabPageAdd.Controls.Add(this.textBoxName);
            this.tabPageAdd.Controls.Add(this.textBoxStudentId);
            this.tabPageAdd.Controls.Add(this.labelMail);
            this.tabPageAdd.Controls.Add(this.labelGivenName);
            this.tabPageAdd.Controls.Add(this.labelName);
            this.tabPageAdd.Controls.Add(this.labelStudentId);
            this.tabPageAdd.Controls.Add(this.labelAddTitle);
            this.tabPageAdd.Location = new System.Drawing.Point(4, 22);
            this.tabPageAdd.Name = "tabPageAdd";
            this.tabPageAdd.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageAdd.Size = new System.Drawing.Size(576, 335);
            this.tabPageAdd.TabIndex = 0;
            this.tabPageAdd.Text = "追加";
            this.tabPageAdd.UseVisualStyleBackColor = true;
            // 
            // buttonRegister
            // 
            this.buttonRegister.Location = new System.Drawing.Point(463, 220);
            this.buttonRegister.Name = "buttonRegister";
            this.buttonRegister.Size = new System.Drawing.Size(75, 23);
            this.buttonRegister.TabIndex = 9;
            this.buttonRegister.Text = "登録";
            this.buttonRegister.UseVisualStyleBackColor = true;
            this.buttonRegister.Click += new System.EventHandler(this.buttonRegister_Click);
            // 
            // textBoxMail
            // 
            this.textBoxMail.Location = new System.Drawing.Point(120, 170);
            this.textBoxMail.Name = "textBoxMail";
            this.textBoxMail.Size = new System.Drawing.Size(418, 19);
            this.textBoxMail.TabIndex = 8;
            // 
            // textBoxGivenName
            // 
            this.textBoxGivenName.Location = new System.Drawing.Point(120, 134);
            this.textBoxGivenName.Name = "textBoxGivenName";
            this.textBoxGivenName.Size = new System.Drawing.Size(250, 19);
            this.textBoxGivenName.TabIndex = 6;
            // 
            // textBoxName
            // 
            this.textBoxName.Location = new System.Drawing.Point(120, 98);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(250, 19);
            this.textBoxName.TabIndex = 5;
            // 
            // textBoxStudentId
            // 
            this.textBoxStudentId.Location = new System.Drawing.Point(120, 62);
            this.textBoxStudentId.Name = "textBoxStudentId";
            this.textBoxStudentId.Size = new System.Drawing.Size(120, 19);
            this.textBoxStudentId.TabIndex = 4;
            // 
            // labelMail
            // 
            this.labelMail.AutoSize = true;
            this.labelMail.Location = new System.Drawing.Point(20, 173);
            this.labelMail.Name = "labelMail";
            this.labelMail.Size = new System.Drawing.Size(79, 12);
            this.labelMail.TabIndex = 3;
            this.labelMail.Text = "メールアドレス";
            // 
            // labelGivenName
            // 
            this.labelGivenName.AutoSize = true;
            this.labelGivenName.Location = new System.Drawing.Point(20, 137);
            this.labelGivenName.Name = "labelGivenName";
            this.labelGivenName.Size = new System.Drawing.Size(29, 12);
            this.labelGivenName.TabIndex = 11;
            this.labelGivenName.Text = "名前";
            // 
            // labelName
            // 
            this.labelName.AutoSize = true;
            this.labelName.Location = new System.Drawing.Point(20, 101);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(29, 12);
            this.labelName.TabIndex = 2;
            this.labelName.Text = "苗字";
            // 
            // labelNameHint
            // 
            this.labelNameHint.AutoSize = true;
            this.labelNameHint.ForeColor = System.Drawing.SystemColors.GrayText;
            this.labelNameHint.Location = new System.Drawing.Point(118, 196);
            this.labelNameHint.Name = "labelNameHint";
            this.labelNameHint.Size = new System.Drawing.Size(300, 12);
            this.labelNameHint.TabIndex = 12;
            this.labelNameHint.Text = "保存は「苗字 名前」。カレンダーの日直は空白より前を苗字として表示します。";
            // 
            // labelStudentId
            // 
            this.labelStudentId.AutoSize = true;
            this.labelStudentId.Location = new System.Drawing.Point(20, 65);
            this.labelStudentId.Name = "labelStudentId";
            this.labelStudentId.Size = new System.Drawing.Size(53, 12);
            this.labelStudentId.TabIndex = 1;
            this.labelStudentId.Text = "学籍番号";
            // 
            // labelAddTitle
            // 
            this.labelAddTitle.AutoSize = true;
            this.labelAddTitle.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Bold);
            this.labelAddTitle.Location = new System.Drawing.Point(19, 20);
            this.labelAddTitle.Name = "labelAddTitle";
            this.labelAddTitle.Size = new System.Drawing.Size(127, 16);
            this.labelAddTitle.TabIndex = 0;
            this.labelAddTitle.Text = "学生情報の追加";
            // 
            // tabPageDelete
            // 
            this.tabPageDelete.Controls.Add(this.buttonDelete);
            this.tabPageDelete.Controls.Add(this.dataGridViewStudents);
            this.tabPageDelete.Controls.Add(this.buttonSearch);
            this.tabPageDelete.Controls.Add(this.textBoxSearchValue);
            this.tabPageDelete.Controls.Add(this.comboBoxSearchField);
            this.tabPageDelete.Controls.Add(this.labelSearch);
            this.tabPageDelete.Controls.Add(this.labelDeleteTitle);
            this.tabPageDelete.Location = new System.Drawing.Point(4, 22);
            this.tabPageDelete.Name = "tabPageDelete";
            this.tabPageDelete.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageDelete.Size = new System.Drawing.Size(576, 335);
            this.tabPageDelete.TabIndex = 1;
            this.tabPageDelete.Text = "削除";
            this.tabPageDelete.UseVisualStyleBackColor = true;
            // 
            // buttonDelete
            // 
            this.buttonDelete.Location = new System.Drawing.Point(463, 296);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(75, 23);
            this.buttonDelete.TabIndex = 6;
            this.buttonDelete.Text = "削除";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // dataGridViewStudents
            // 
            this.dataGridViewStudents.AllowUserToAddRows = false;
            this.dataGridViewStudents.AllowUserToDeleteRows = false;
            this.dataGridViewStudents.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewStudents.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewStudents.Location = new System.Drawing.Point(22, 92);
            this.dataGridViewStudents.MultiSelect = false;
            this.dataGridViewStudents.Name = "dataGridViewStudents";
            this.dataGridViewStudents.ReadOnly = true;
            this.dataGridViewStudents.RowHeadersVisible = false;
            this.dataGridViewStudents.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewStudents.Size = new System.Drawing.Size(516, 190);
            this.dataGridViewStudents.TabIndex = 5;
            // 
            // buttonSearch
            // 
            this.buttonSearch.Location = new System.Drawing.Point(463, 55);
            this.buttonSearch.Name = "buttonSearch";
            this.buttonSearch.Size = new System.Drawing.Size(75, 23);
            this.buttonSearch.TabIndex = 4;
            this.buttonSearch.Text = "検索";
            this.buttonSearch.UseVisualStyleBackColor = true;
            this.buttonSearch.Click += new System.EventHandler(this.buttonSearch_Click);
            // 
            // textBoxSearchValue
            // 
            this.textBoxSearchValue.Location = new System.Drawing.Point(248, 57);
            this.textBoxSearchValue.Name = "textBoxSearchValue";
            this.textBoxSearchValue.Size = new System.Drawing.Size(200, 19);
            this.textBoxSearchValue.TabIndex = 3;
            // 
            // comboBoxSearchField
            // 
            this.comboBoxSearchField.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSearchField.FormattingEnabled = true;
            this.comboBoxSearchField.Items.AddRange(new object[] {
            "学籍番号",
            "氏名",
            "メールアドレス"});
            this.comboBoxSearchField.Location = new System.Drawing.Point(120, 57);
            this.comboBoxSearchField.Name = "comboBoxSearchField";
            this.comboBoxSearchField.Size = new System.Drawing.Size(110, 20);
            this.comboBoxSearchField.TabIndex = 2;
            // 
            // labelSearch
            // 
            this.labelSearch.AutoSize = true;
            this.labelSearch.Location = new System.Drawing.Point(20, 60);
            this.labelSearch.Name = "labelSearch";
            this.labelSearch.Size = new System.Drawing.Size(53, 12);
            this.labelSearch.TabIndex = 1;
            this.labelSearch.Text = "検索条件";
            // 
            // labelDeleteTitle
            // 
            this.labelDeleteTitle.AutoSize = true;
            this.labelDeleteTitle.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Bold);
            this.labelDeleteTitle.Location = new System.Drawing.Point(19, 20);
            this.labelDeleteTitle.Name = "labelDeleteTitle";
            this.labelDeleteTitle.Size = new System.Drawing.Size(127, 16);
            this.labelDeleteTitle.TabIndex = 0;
            this.labelDeleteTitle.Text = "学生情報の削除";
            // 
            // Form11
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 361);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form11";
            this.Text = "学生情報管理";
            this.tabControl1.ResumeLayout(false);
            this.tabPageAdd.ResumeLayout(false);
            this.tabPageAdd.PerformLayout();
            this.tabPageDelete.ResumeLayout(false);
            this.tabPageDelete.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewStudents)).EndInit();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageAdd;
        private System.Windows.Forms.TabPage tabPageDelete;
        private System.Windows.Forms.Label labelAddTitle;
        private System.Windows.Forms.Label labelStudentId;
        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.Label labelGivenName;
        private System.Windows.Forms.Label labelNameHint;
        private System.Windows.Forms.Label labelMail;
        private System.Windows.Forms.TextBox textBoxStudentId;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.TextBox textBoxGivenName;
        private System.Windows.Forms.TextBox textBoxMail;
        private System.Windows.Forms.Button buttonRegister;
        private System.Windows.Forms.Label labelDeleteTitle;
        private System.Windows.Forms.Label labelSearch;
        private System.Windows.Forms.ComboBox comboBoxSearchField;
        private System.Windows.Forms.TextBox textBoxSearchValue;
        private System.Windows.Forms.Button buttonSearch;
        private System.Windows.Forms.DataGridView dataGridViewStudents;
        private System.Windows.Forms.Button buttonDelete;
    }
}
