namespace LabManager
{
    partial class Form12
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
            this.tabPageView = new System.Windows.Forms.TabPage();
            this.labelChipStatus = new System.Windows.Forms.Label();
            this.buttonDeleteChip = new System.Windows.Forms.Button();
            this.dataGridViewChipInfo = new System.Windows.Forms.DataGridView();
            this.buttonLoadChip = new System.Windows.Forms.Button();
            this.buttonLoadLatestTouch = new System.Windows.Forms.Button();
            this.textBoxChipId = new System.Windows.Forms.TextBox();
            this.labelChipId = new System.Windows.Forms.Label();
            this.labelViewTitle = new System.Windows.Forms.Label();
            this.tabPageLink = new System.Windows.Forms.TabPage();
            this.buttonCopyChipId = new System.Windows.Forms.Button();
            this.textBoxSystemId = new System.Windows.Forms.TextBox();
            this.labelSystemId = new System.Windows.Forms.Label();
            this.buttonLink = new System.Windows.Forms.Button();
            this.dataGridViewStudents = new System.Windows.Forms.DataGridView();
            this.buttonSearchStudent = new System.Windows.Forms.Button();
            this.textBoxSearchValue = new System.Windows.Forms.TextBox();
            this.comboBoxSearchField = new System.Windows.Forms.ComboBox();
            this.labelSearch = new System.Windows.Forms.Label();
            this.textBoxLinkChipId = new System.Windows.Forms.TextBox();
            this.labelLinkChipId = new System.Windows.Forms.Label();
            this.labelLinkTitle = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPageView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewChipInfo)).BeginInit();
            this.tabPageLink.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewStudents)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageView);
            this.tabControl1.Controls.Add(this.tabPageLink);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(634, 421);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPageView
            // 
            this.tabPageView.Controls.Add(this.labelChipStatus);
            this.tabPageView.Controls.Add(this.buttonDeleteChip);
            this.tabPageView.Controls.Add(this.dataGridViewChipInfo);
            this.tabPageView.Controls.Add(this.buttonLoadChip);
            this.tabPageView.Controls.Add(this.buttonLoadLatestTouch);
            this.tabPageView.Controls.Add(this.textBoxChipId);
            this.tabPageView.Controls.Add(this.labelChipId);
            this.tabPageView.Controls.Add(this.labelViewTitle);
            this.tabPageView.Location = new System.Drawing.Point(4, 22);
            this.tabPageView.Name = "tabPageView";
            this.tabPageView.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageView.Size = new System.Drawing.Size(626, 395);
            this.tabPageView.TabIndex = 0;
            this.tabPageView.Text = "確認・削除";
            this.tabPageView.UseVisualStyleBackColor = true;
            // 
            // labelChipStatus
            // 
            this.labelChipStatus.AutoSize = true;
            this.labelChipStatus.Location = new System.Drawing.Point(20, 96);
            this.labelChipStatus.Name = "labelChipStatus";
            this.labelChipStatus.Size = new System.Drawing.Size(77, 12);
            this.labelChipStatus.TabIndex = 7;
            this.labelChipStatus.Text = "状態: 未確認";
            // 
            // buttonDeleteChip
            // 
            this.buttonDeleteChip.Location = new System.Drawing.Point(463, 350);
            this.buttonDeleteChip.Name = "buttonDeleteChip";
            this.buttonDeleteChip.Size = new System.Drawing.Size(120, 23);
            this.buttonDeleteChip.TabIndex = 6;
            this.buttonDeleteChip.Text = "関連付けを消去";
            this.buttonDeleteChip.UseVisualStyleBackColor = true;
            this.buttonDeleteChip.Click += new System.EventHandler(this.buttonDeleteChip_Click);
            // 
            // dataGridViewChipInfo
            // 
            this.dataGridViewChipInfo.AllowUserToAddRows = false;
            this.dataGridViewChipInfo.AllowUserToDeleteRows = false;
            this.dataGridViewChipInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewChipInfo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewChipInfo.Location = new System.Drawing.Point(22, 120);
            this.dataGridViewChipInfo.MultiSelect = false;
            this.dataGridViewChipInfo.Name = "dataGridViewChipInfo";
            this.dataGridViewChipInfo.ReadOnly = true;
            this.dataGridViewChipInfo.RowHeadersVisible = false;
            this.dataGridViewChipInfo.Size = new System.Drawing.Size(561, 220);
            this.dataGridViewChipInfo.TabIndex = 5;
            // 
            // buttonLoadChip
            // 
            this.buttonLoadChip.Location = new System.Drawing.Point(463, 58);
            this.buttonLoadChip.Name = "buttonLoadChip";
            this.buttonLoadChip.Size = new System.Drawing.Size(75, 23);
            this.buttonLoadChip.TabIndex = 4;
            this.buttonLoadChip.Text = "表示";
            this.buttonLoadChip.UseVisualStyleBackColor = true;
            this.buttonLoadChip.Click += new System.EventHandler(this.buttonLoadChip_Click);
            // 
            // buttonLoadLatestTouch
            // 
            this.buttonLoadLatestTouch.Location = new System.Drawing.Point(322, 58);
            this.buttonLoadLatestTouch.Name = "buttonLoadLatestTouch";
            this.buttonLoadLatestTouch.Size = new System.Drawing.Size(120, 23);
            this.buttonLoadLatestTouch.TabIndex = 3;
            this.buttonLoadLatestTouch.Text = "最新タッチから取得";
            this.buttonLoadLatestTouch.UseVisualStyleBackColor = true;
            this.buttonLoadLatestTouch.Click += new System.EventHandler(this.buttonLoadLatestTouch_Click);
            // 
            // textBoxChipId
            // 
            this.textBoxChipId.Location = new System.Drawing.Point(120, 60);
            this.textBoxChipId.Name = "textBoxChipId";
            this.textBoxChipId.Size = new System.Drawing.Size(180, 19);
            this.textBoxChipId.TabIndex = 2;
            // 
            // labelChipId
            // 
            this.labelChipId.AutoSize = true;
            this.labelChipId.Location = new System.Drawing.Point(20, 63);
            this.labelChipId.Name = "labelChipId";
            this.labelChipId.Size = new System.Drawing.Size(47, 12);
            this.labelChipId.TabIndex = 1;
            this.labelChipId.Text = "カードID";
            // 
            // labelViewTitle
            // 
            this.labelViewTitle.AutoSize = true;
            this.labelViewTitle.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Bold);
            this.labelViewTitle.Location = new System.Drawing.Point(19, 20);
            this.labelViewTitle.Name = "labelViewTitle";
            this.labelViewTitle.Size = new System.Drawing.Size(127, 16);
            this.labelViewTitle.TabIndex = 0;
            this.labelViewTitle.Text = "カード情報の確認";
            // 
            // tabPageLink
            // 
            this.tabPageLink.Controls.Add(this.buttonCopyChipId);
            this.tabPageLink.Controls.Add(this.textBoxSystemId);
            this.tabPageLink.Controls.Add(this.labelSystemId);
            this.tabPageLink.Controls.Add(this.buttonLink);
            this.tabPageLink.Controls.Add(this.dataGridViewStudents);
            this.tabPageLink.Controls.Add(this.buttonSearchStudent);
            this.tabPageLink.Controls.Add(this.textBoxSearchValue);
            this.tabPageLink.Controls.Add(this.comboBoxSearchField);
            this.tabPageLink.Controls.Add(this.labelSearch);
            this.tabPageLink.Controls.Add(this.textBoxLinkChipId);
            this.tabPageLink.Controls.Add(this.labelLinkChipId);
            this.tabPageLink.Controls.Add(this.labelLinkTitle);
            this.tabPageLink.Location = new System.Drawing.Point(4, 22);
            this.tabPageLink.Name = "tabPageLink";
            this.tabPageLink.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageLink.Size = new System.Drawing.Size(626, 395);
            this.tabPageLink.TabIndex = 1;
            this.tabPageLink.Text = "学生への関連付け";
            this.tabPageLink.UseVisualStyleBackColor = true;
            // 
            // buttonCopyChipId
            // 
            this.buttonCopyChipId.Location = new System.Drawing.Point(322, 57);
            this.buttonCopyChipId.Name = "buttonCopyChipId";
            this.buttonCopyChipId.Size = new System.Drawing.Size(120, 23);
            this.buttonCopyChipId.TabIndex = 11;
            this.buttonCopyChipId.Text = "確認タブからコピー";
            this.buttonCopyChipId.UseVisualStyleBackColor = true;
            this.buttonCopyChipId.Click += new System.EventHandler(this.buttonCopyChipId_Click);
            // 
            // textBoxSystemId
            // 
            this.textBoxSystemId.Location = new System.Drawing.Point(120, 350);
            this.textBoxSystemId.Name = "textBoxSystemId";
            this.textBoxSystemId.Size = new System.Drawing.Size(150, 19);
            this.textBoxSystemId.TabIndex = 10;
            // 
            // labelSystemId
            // 
            this.labelSystemId.AutoSize = true;
            this.labelSystemId.Location = new System.Drawing.Point(20, 353);
            this.labelSystemId.Name = "labelSystemId";
            this.labelSystemId.Size = new System.Drawing.Size(68, 12);
            this.labelSystemId.TabIndex = 9;
            this.labelSystemId.Text = "システムID";
            // 
            // buttonLink
            // 
            this.buttonLink.Location = new System.Drawing.Point(463, 348);
            this.buttonLink.Name = "buttonLink";
            this.buttonLink.Size = new System.Drawing.Size(120, 23);
            this.buttonLink.TabIndex = 8;
            this.buttonLink.Text = "関連付け";
            this.buttonLink.UseVisualStyleBackColor = true;
            this.buttonLink.Click += new System.EventHandler(this.buttonLink_Click);
            // 
            // dataGridViewStudents
            // 
            this.dataGridViewStudents.AllowUserToAddRows = false;
            this.dataGridViewStudents.AllowUserToDeleteRows = false;
            this.dataGridViewStudents.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewStudents.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewStudents.Location = new System.Drawing.Point(22, 130);
            this.dataGridViewStudents.MultiSelect = false;
            this.dataGridViewStudents.Name = "dataGridViewStudents";
            this.dataGridViewStudents.ReadOnly = true;
            this.dataGridViewStudents.RowHeadersVisible = false;
            this.dataGridViewStudents.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewStudents.Size = new System.Drawing.Size(561, 200);
            this.dataGridViewStudents.TabIndex = 7;
            this.dataGridViewStudents.SelectionChanged += new System.EventHandler(this.dataGridViewStudents_SelectionChanged);
            // 
            // buttonSearchStudent
            // 
            this.buttonSearchStudent.Location = new System.Drawing.Point(463, 96);
            this.buttonSearchStudent.Name = "buttonSearchStudent";
            this.buttonSearchStudent.Size = new System.Drawing.Size(75, 23);
            this.buttonSearchStudent.TabIndex = 6;
            this.buttonSearchStudent.Text = "検索";
            this.buttonSearchStudent.UseVisualStyleBackColor = true;
            this.buttonSearchStudent.Click += new System.EventHandler(this.buttonSearchStudent_Click);
            // 
            // textBoxSearchValue
            // 
            this.textBoxSearchValue.Location = new System.Drawing.Point(248, 98);
            this.textBoxSearchValue.Name = "textBoxSearchValue";
            this.textBoxSearchValue.Size = new System.Drawing.Size(200, 19);
            this.textBoxSearchValue.TabIndex = 5;
            // 
            // comboBoxSearchField
            // 
            this.comboBoxSearchField.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSearchField.FormattingEnabled = true;
            this.comboBoxSearchField.Items.AddRange(new object[] {
            "学籍番号",
            "氏名",
            "メールアドレス"});
            this.comboBoxSearchField.Location = new System.Drawing.Point(120, 98);
            this.comboBoxSearchField.Name = "comboBoxSearchField";
            this.comboBoxSearchField.Size = new System.Drawing.Size(110, 20);
            this.comboBoxSearchField.TabIndex = 4;
            // 
            // labelSearch
            // 
            this.labelSearch.AutoSize = true;
            this.labelSearch.Location = new System.Drawing.Point(20, 101);
            this.labelSearch.Name = "labelSearch";
            this.labelSearch.Size = new System.Drawing.Size(53, 12);
            this.labelSearch.TabIndex = 3;
            this.labelSearch.Text = "学生検索";
            // 
            // textBoxLinkChipId
            // 
            this.textBoxLinkChipId.Location = new System.Drawing.Point(120, 59);
            this.textBoxLinkChipId.Name = "textBoxLinkChipId";
            this.textBoxLinkChipId.Size = new System.Drawing.Size(180, 19);
            this.textBoxLinkChipId.TabIndex = 2;
            // 
            // labelLinkChipId
            // 
            this.labelLinkChipId.AutoSize = true;
            this.labelLinkChipId.Location = new System.Drawing.Point(20, 62);
            this.labelLinkChipId.Name = "labelLinkChipId";
            this.labelLinkChipId.Size = new System.Drawing.Size(47, 12);
            this.labelLinkChipId.TabIndex = 1;
            this.labelLinkChipId.Text = "カードID";
            // 
            // labelLinkTitle
            // 
            this.labelLinkTitle.AutoSize = true;
            this.labelLinkTitle.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Bold);
            this.labelLinkTitle.Location = new System.Drawing.Point(19, 20);
            this.labelLinkTitle.Name = "labelLinkTitle";
            this.labelLinkTitle.Size = new System.Drawing.Size(195, 16);
            this.labelLinkTitle.TabIndex = 0;
            this.labelLinkTitle.Text = "カードを学生に関連付け";
            // 
            // Form12
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(634, 421);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form12";
            this.Text = "カード管理";
            this.tabControl1.ResumeLayout(false);
            this.tabPageView.ResumeLayout(false);
            this.tabPageView.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewChipInfo)).EndInit();
            this.tabPageLink.ResumeLayout(false);
            this.tabPageLink.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewStudents)).EndInit();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageView;
        private System.Windows.Forms.TabPage tabPageLink;
        private System.Windows.Forms.Label labelViewTitle;
        private System.Windows.Forms.Label labelChipId;
        private System.Windows.Forms.TextBox textBoxChipId;
        private System.Windows.Forms.Button buttonLoadLatestTouch;
        private System.Windows.Forms.Button buttonLoadChip;
        private System.Windows.Forms.DataGridView dataGridViewChipInfo;
        private System.Windows.Forms.Button buttonDeleteChip;
        private System.Windows.Forms.Label labelChipStatus;
        private System.Windows.Forms.Label labelLinkTitle;
        private System.Windows.Forms.Label labelLinkChipId;
        private System.Windows.Forms.TextBox textBoxLinkChipId;
        private System.Windows.Forms.Label labelSearch;
        private System.Windows.Forms.ComboBox comboBoxSearchField;
        private System.Windows.Forms.TextBox textBoxSearchValue;
        private System.Windows.Forms.Button buttonSearchStudent;
        private System.Windows.Forms.DataGridView dataGridViewStudents;
        private System.Windows.Forms.Button buttonLink;
        private System.Windows.Forms.Label labelSystemId;
        private System.Windows.Forms.TextBox textBoxSystemId;
        private System.Windows.Forms.Button buttonCopyChipId;
    }
}
