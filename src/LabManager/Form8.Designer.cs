using System;
using System.Windows.Forms;

namespace MySQL_ConnectionTest
{
    partial class Form8
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TextBox textBoxTitle;
        private System.Windows.Forms.TextBox textBoxAuthor;
        private System.Windows.Forms.TextBox textBoxYear;
        private System.Windows.Forms.TextBox textBoxThesisType;
        private System.Windows.Forms.TextBox textBoxSiteName;
        private System.Windows.Forms.TextBox textBoxUrl;
        private System.Windows.Forms.TextBox textBoxNotes;
        private System.Windows.Forms.TextBox textBoxPages;
        private System.Windows.Forms.ComboBox comboBoxGenre;
        private System.Windows.Forms.RadioButton radioButtonThesis;
        private System.Windows.Forms.RadioButton radioButtonSite;
        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.Button buttonEdit;
        private System.Windows.Forms.Button buttonCopyCitation;
        private System.Windows.Forms.Button buttonSortByGenre;
        private System.Windows.Forms.Button buttonSortById;
        private System.Windows.Forms.Button buttonImportFromCSV;

        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.Label labelAuthor;
        private System.Windows.Forms.Label labelYear;
        private System.Windows.Forms.Label labelThesisType;
        private System.Windows.Forms.Label labelSiteName;
        private System.Windows.Forms.Label labelUrl;
        private System.Windows.Forms.Label labelNotes;
        private System.Windows.Forms.Label labelPages;
        private System.Windows.Forms.Label labelGenre;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.textBoxTitle = new System.Windows.Forms.TextBox();
            this.textBoxAuthor = new System.Windows.Forms.TextBox();
            this.textBoxYear = new System.Windows.Forms.TextBox();
            this.textBoxThesisType = new System.Windows.Forms.TextBox();
            this.textBoxSiteName = new System.Windows.Forms.TextBox();
            this.textBoxUrl = new System.Windows.Forms.TextBox();
            this.textBoxNotes = new System.Windows.Forms.TextBox();
            this.textBoxPages = new System.Windows.Forms.TextBox();
            this.comboBoxGenre = new System.Windows.Forms.ComboBox();
            this.radioButtonThesis = new System.Windows.Forms.RadioButton();
            this.radioButtonSite = new System.Windows.Forms.RadioButton();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonEdit = new System.Windows.Forms.Button();
            this.buttonCopyCitation = new System.Windows.Forms.Button();
            this.buttonSortByGenre = new System.Windows.Forms.Button();
            this.buttonSortById = new System.Windows.Forms.Button();
            this.labelTitle = new System.Windows.Forms.Label();
            this.labelAuthor = new System.Windows.Forms.Label();
            this.labelYear = new System.Windows.Forms.Label();
            this.labelThesisType = new System.Windows.Forms.Label();
            this.labelSiteName = new System.Windows.Forms.Label();
            this.labelUrl = new System.Windows.Forms.Label();
            this.labelNotes = new System.Windows.Forms.Label();
            this.labelPages = new System.Windows.Forms.Label();
            this.labelGenre = new System.Windows.Forms.Label();
            this.buttonImportFromCSV = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();

            // dataGridView1
            this.dataGridView1.Location = new System.Drawing.Point(20, 20);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.MultiSelect = true;
            this.dataGridView1.Size = new System.Drawing.Size(1040, 200);
            this.dataGridView1.TabIndex = 13;
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            this.dataGridView1.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.dataGridView1_RowPrePaint);

            // textBoxTitle
            this.textBoxTitle.Location = new System.Drawing.Point(120, 230);
            this.textBoxTitle.Name = "textBoxTitle";
            this.textBoxTitle.Size = new System.Drawing.Size(300, 19);
            this.textBoxTitle.TabIndex = 15;

            // textBoxAuthor
            this.textBoxAuthor.Location = new System.Drawing.Point(120, 260);
            this.textBoxAuthor.Name = "textBoxAuthor";
            this.textBoxAuthor.Size = new System.Drawing.Size(300, 19);
            this.textBoxAuthor.TabIndex = 17;

            // textBoxYear
            this.textBoxYear.Location = new System.Drawing.Point(120, 290);
            this.textBoxYear.Name = "textBoxYear";
            this.textBoxYear.Size = new System.Drawing.Size(100, 19);
            this.textBoxYear.TabIndex = 19;

            // textBoxThesisType
            this.textBoxThesisType.Location = new System.Drawing.Point(120, 320);
            this.textBoxThesisType.Name = "textBoxThesisType";
            this.textBoxThesisType.Size = new System.Drawing.Size(300, 19);
            this.textBoxThesisType.TabIndex = 21;

            // textBoxSiteName
            this.textBoxSiteName.Location = new System.Drawing.Point(120, 350);
            this.textBoxSiteName.Name = "textBoxSiteName";
            this.textBoxSiteName.Size = new System.Drawing.Size(300, 19);
            this.textBoxSiteName.TabIndex = 23;

            // textBoxUrl
            this.textBoxUrl.Location = new System.Drawing.Point(120, 380);
            this.textBoxUrl.Name = "textBoxUrl";
            this.textBoxUrl.Size = new System.Drawing.Size(500, 19);
            this.textBoxUrl.TabIndex = 25;

            // textBoxNotes
            this.textBoxNotes.Location = new System.Drawing.Point(120, 410);
            this.textBoxNotes.Name = "textBoxNotes";
            this.textBoxNotes.Size = new System.Drawing.Size(500, 19);
            this.textBoxNotes.TabIndex = 27;

            // textBoxPages
            this.textBoxPages.Location = new System.Drawing.Point(120, 440);
            this.textBoxPages.Name = "textBoxPages";
            this.textBoxPages.Size = new System.Drawing.Size(200, 19);
            this.textBoxPages.TabIndex = 29;

            // comboBoxGenre
            this.comboBoxGenre.Location = new System.Drawing.Point(120, 470);
            this.comboBoxGenre.Name = "comboBoxGenre";
            this.comboBoxGenre.Size = new System.Drawing.Size(200, 20);
            this.comboBoxGenre.TabIndex = 31;

            // radioButtonThesis
            this.radioButtonThesis.Checked = true;
            this.radioButtonThesis.Location = new System.Drawing.Point(350, 470);
            this.radioButtonThesis.Name = "radioButtonThesis";
            this.radioButtonThesis.Size = new System.Drawing.Size(104, 24);
            this.radioButtonThesis.TabIndex = 32;
            this.radioButtonThesis.TabStop = true;
            this.radioButtonThesis.Text = "論文形式";

            // radioButtonSite
            this.radioButtonSite.Location = new System.Drawing.Point(460, 470);
            this.radioButtonSite.Name = "radioButtonSite";
            this.radioButtonSite.Size = new System.Drawing.Size(104, 24);
            this.radioButtonSite.TabIndex = 33;
            this.radioButtonSite.Text = "サイト形式";

            // buttonAdd
            this.buttonAdd.Location = new System.Drawing.Point(20, 510);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(75, 23);
            this.buttonAdd.TabIndex = 34;
            this.buttonAdd.Text = "追加";
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);

            // buttonDelete
            this.buttonDelete.Location = new System.Drawing.Point(120, 510);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(75, 23);
            this.buttonDelete.TabIndex = 35;
            this.buttonDelete.Text = "削除";
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);

            // buttonEdit
            this.buttonEdit.Location = new System.Drawing.Point(220, 510);
            this.buttonEdit.Name = "buttonEdit";
            this.buttonEdit.Size = new System.Drawing.Size(75, 23);
            this.buttonEdit.TabIndex = 36;
            this.buttonEdit.Text = "編集";
            this.buttonEdit.Click += new System.EventHandler(this.buttonEdit_Click);

            // buttonCopyCitation
            this.buttonCopyCitation.Location = new System.Drawing.Point(320, 510);
            this.buttonCopyCitation.Name = "buttonCopyCitation";
            this.buttonCopyCitation.Size = new System.Drawing.Size(90, 23);
            this.buttonCopyCitation.TabIndex = 37;
            this.buttonCopyCitation.Text = "引用をコピー";
            this.buttonCopyCitation.Click += new System.EventHandler(this.buttonCopyCitation_Click);

            // buttonSortByGenre
            this.buttonSortByGenre.Location = new System.Drawing.Point(850, 520);
            this.buttonSortByGenre.Name = "buttonSortByGenre";
            this.buttonSortByGenre.Size = new System.Drawing.Size(150, 30);
            this.buttonSortByGenre.TabIndex = 38;
            this.buttonSortByGenre.Text = "ジャンルで並び替え";
            this.buttonSortByGenre.Click += new System.EventHandler(this.buttonSortByGenre_Click);

            // buttonSortById
            this.buttonSortById.Location = new System.Drawing.Point(690, 520);
            this.buttonSortById.Name = "buttonSortById";
            this.buttonSortById.Size = new System.Drawing.Size(150, 30);
            this.buttonSortById.TabIndex = 39;
            this.buttonSortById.Text = "登録順で並び替え";
            this.buttonSortById.Click += new System.EventHandler(this.buttonSortById_Click);

            // labels
            this.labelTitle.Location = new System.Drawing.Point(20, 230);
            this.labelTitle.Size = new System.Drawing.Size(100, 23);
            this.labelTitle.Text = "タイトル";

            this.labelAuthor.Location = new System.Drawing.Point(20, 260);
            this.labelAuthor.Size = new System.Drawing.Size(100, 23);
            this.labelAuthor.Text = "著者";

            this.labelYear.Location = new System.Drawing.Point(20, 290);
            this.labelYear.Size = new System.Drawing.Size(100, 23);
            this.labelYear.Text = "発行年";

            this.labelThesisType.Location = new System.Drawing.Point(20, 320);
            this.labelThesisType.Size = new System.Drawing.Size(100, 23);
            this.labelThesisType.Text = "文献タイプ";

            this.labelSiteName.Location = new System.Drawing.Point(20, 350);
            this.labelSiteName.Size = new System.Drawing.Size(100, 23);
            this.labelSiteName.Text = "サイト名";

            this.labelUrl.Location = new System.Drawing.Point(20, 380);
            this.labelUrl.Size = new System.Drawing.Size(100, 23);
            this.labelUrl.Text = "URL";

            this.labelNotes.Location = new System.Drawing.Point(20, 410);
            this.labelNotes.Size = new System.Drawing.Size(100, 23);
            this.labelNotes.Text = "説明";

            this.labelPages.Location = new System.Drawing.Point(20, 440);
            this.labelPages.Size = new System.Drawing.Size(100, 23);
            this.labelPages.Text = "ページ数";

            this.labelGenre.Location = new System.Drawing.Point(20, 470);
            this.labelGenre.Size = new System.Drawing.Size(100, 23);
            this.labelGenre.Text = "ジャンル";

            // buttonImportFromCSV
            this.buttonImportFromCSV.Location = new System.Drawing.Point(850, 470);
            this.buttonImportFromCSV.Name = "buttonImportFromCSV";
            this.buttonImportFromCSV.Size = new System.Drawing.Size(120, 30);
            this.buttonImportFromCSV.TabIndex = 12;
            this.buttonImportFromCSV.Text = "更新";
            this.buttonImportFromCSV.UseVisualStyleBackColor = true;
            this.buttonImportFromCSV.Click += new System.EventHandler(this.buttonImportFromForm_Click);

            // Form8
            this.ClientSize = new System.Drawing.Size(1070, 580);
            this.Controls.Add(this.buttonImportFromCSV);
            this.Controls.Add(this.dataGridView1);

            this.Controls.Add(this.labelTitle);
            this.Controls.Add(this.textBoxTitle);
            this.Controls.Add(this.labelAuthor);
            this.Controls.Add(this.textBoxAuthor);
            this.Controls.Add(this.labelYear);
            this.Controls.Add(this.textBoxYear);
            this.Controls.Add(this.labelThesisType);
            this.Controls.Add(this.textBoxThesisType);
            this.Controls.Add(this.labelSiteName);
            this.Controls.Add(this.textBoxSiteName);
            this.Controls.Add(this.labelUrl);
            this.Controls.Add(this.textBoxUrl);
            this.Controls.Add(this.labelNotes);
            this.Controls.Add(this.textBoxNotes);
            this.Controls.Add(this.labelPages);
            this.Controls.Add(this.textBoxPages);
            this.Controls.Add(this.labelGenre);
            this.Controls.Add(this.comboBoxGenre);

            this.Controls.Add(this.radioButtonThesis);
            this.Controls.Add(this.radioButtonSite);

            this.Controls.Add(this.buttonAdd);
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.buttonEdit);
            this.Controls.Add(this.buttonCopyCitation);
            this.Controls.Add(this.buttonSortByGenre);
            this.Controls.Add(this.buttonSortById);

            this.Name = "Form8";
            this.Text = "文献管理";

            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}

