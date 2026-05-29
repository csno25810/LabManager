namespace LabManager
{
    partial class Form5
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
            this.monthCalendar1 = new System.Windows.Forms.MonthCalendar();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.dateTimePicker2 = new System.Windows.Forms.DateTimePicker();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.panel4 = new System.Windows.Forms.Panel();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxW = new System.Windows.Forms.TextBox();
            this.textBoxF = new System.Windows.Forms.TextBox();
            this.textBoxT = new System.Windows.Forms.TextBox();
            this.textBoxTh = new System.Windows.Forms.TextBox();
            this.textBoxM = new System.Windows.Forms.TextBox();
            this.button7 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.button6 = new System.Windows.Forms.Button();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.panel5 = new System.Windows.Forms.Panel();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.buttonRemove = new System.Windows.Forms.Button();
            this.listBoxStudentNumbers = new System.Windows.Forms.ListBox();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.textBoxStudentNumber = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.dateTimePicker3 = new System.Windows.Forms.DateTimePicker();
            this.dateTimePicker4 = new System.Windows.Forms.DateTimePicker();
            this.buttonRegister = new System.Windows.Forms.Button();
            this.panel7 = new System.Windows.Forms.Panel();
            this.checkBox6 = new System.Windows.Forms.CheckBox();
            this.checkBox5 = new System.Windows.Forms.CheckBox();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.label19 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.radioButton11 = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();

            // Googleフォーム読み込みUI
            this.labelMonth = new System.Windows.Forms.Label();
            this.comboBoxMonth = new System.Windows.Forms.ComboBox();
            this.buttonLoadFromGoogleForm = new System.Windows.Forms.Button();
            this.labelNote = new System.Windows.Forms.Label();
            this.dataGridViewNote = new System.Windows.Forms.DataGridView();

            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewNote)).BeginInit();

            this.panel5.SuspendLayout();
            this.panel7.SuspendLayout();
            this.SuspendLayout();
            // 
            // monthCalendar1
            // 
            this.monthCalendar1.Location = new System.Drawing.Point(4, 9);
            this.monthCalendar1.Name = "monthCalendar1";
            this.monthCalendar1.TabIndex = 0;
            this.monthCalendar1.DateSelected += new System.Windows.Forms.DateRangeEventHandler(this.monthCalendar1_DateSelected);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(4, 519);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 19);
            this.textBox1.TabIndex = 1;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(110, 519);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "登録";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(394, 779);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(101, 16);
            this.radioButton1.TabIndex = 4;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "曜日別振り分け";
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(4, 171);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox2.Size = new System.Drawing.Size(100, 326);
            this.textBox2.TabIndex = 5;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(191, 519);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(68, 23);
            this.button2.TabIndex = 7;
            this.button2.Text = "削除";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(110, 474);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(50, 23);
            this.button3.TabIndex = 8;
            this.button3.Text = "clear";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label15);
            this.panel1.Controls.Add(this.label14);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.monthCalendar1);
            this.panel1.Controls.Add(this.textBox2);
            this.panel1.Controls.Add(this.button3);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.textBox1);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(556, 579);
            this.panel1.TabIndex = 11;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(501, 779);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(89, 16);
            this.radioButton2.TabIndex = 12;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "自動振り分け";
            this.radioButton2.UseVisualStyleBackColor = true;
            this.radioButton2.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label16);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.dateTimePicker2);
            this.panel2.Controls.Add(this.dateTimePicker1);
            this.panel2.Controls.Add(this.panel4);
            this.panel2.Controls.Add(this.button7);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Location = new System.Drawing.Point(574, 12);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(698, 579);
            this.panel2.TabIndex = 11;
            // Googleフォーム取り込み UI

            // labelMonth
            this.labelMonth = new System.Windows.Forms.Label();
            this.labelMonth.AutoSize = true;
            this.labelMonth.Location = new System.Drawing.Point(20, 60);
            this.labelMonth.Name = "labelMonth";
            this.labelMonth.Size = new System.Drawing.Size(92, 12);
            this.labelMonth.TabIndex = 100;
            this.labelMonth.Text = "Googleフォーム月度";

            // comboBoxMonth
            this.comboBoxMonth = new System.Windows.Forms.ComboBox();
            this.comboBoxMonth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxMonth.FormattingEnabled = true;
            this.comboBoxMonth.Location = new System.Drawing.Point(150, 57);
            this.comboBoxMonth.Name = "comboBoxMonth";
            this.comboBoxMonth.Size = new System.Drawing.Size(110, 20);
            this.comboBoxMonth.TabIndex = 101;
            this.comboBoxMonth.Items.AddRange(new object[] {
  "1月","2月","3月","4月","5月","6月",
  "7月","8月","9月","10月","11月","12月"
});

            // buttonLoadFromGoogleForm
            this.buttonLoadFromGoogleForm = new System.Windows.Forms.Button();
            this.buttonLoadFromGoogleForm.Location = new System.Drawing.Point(270, 55);
            this.buttonLoadFromGoogleForm.Name = "buttonLoadFromGoogleForm";
            this.buttonLoadFromGoogleForm.Size = new System.Drawing.Size(100, 23);
            this.buttonLoadFromGoogleForm.TabIndex = 102;
            this.buttonLoadFromGoogleForm.Text = "フォーム読込";
            this.buttonLoadFromGoogleForm.UseVisualStyleBackColor = true;
            this.buttonLoadFromGoogleForm.Click += new System.EventHandler(this.buttonLoadFromGoogleForm_Click);

            // labelNote
            this.labelNote = new System.Windows.Forms.Label();
            this.labelNote.AutoSize = true;
            this.labelNote.Location = new System.Drawing.Point(20, 360);
            this.labelNote.Name = "labelNote";
            this.labelNote.Size = new System.Drawing.Size(53, 12);
            this.labelNote.TabIndex = 103;
            this.labelNote.Text = "備考一覧";

            // dataGridViewNote
            this.dataGridViewNote = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewNote)).BeginInit();
            this.dataGridViewNote.Location = new System.Drawing.Point(20, 380);
            this.dataGridViewNote.Name = "dataGridViewNote";
            this.dataGridViewNote.Size = new System.Drawing.Size(650, 120);
            this.dataGridViewNote.TabIndex = 104;
            this.dataGridViewNote.ReadOnly = true;
            this.dataGridViewNote.AllowUserToAddRows = false;
            this.dataGridViewNote.AllowUserToDeleteRows = false;
            this.dataGridViewNote.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewNote)).EndInit();
            // panel2 に追加（これが無いと表示されません）
            this.panel2.Controls.Add(this.labelMonth);
            this.panel2.Controls.Add(this.comboBoxMonth);
            this.panel2.Controls.Add(this.buttonLoadFromGoogleForm);
            this.panel2.Controls.Add(this.labelNote);
            this.panel2.Controls.Add(this.dataGridViewNote);



            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 182);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(233, 12);
            this.label6.TabIndex = 29;
            this.label6.Text = "曜日ごとに担当者の学生番号を入力してください";
            // 
            // dateTimePicker2
            // 
            this.dateTimePicker2.Location = new System.Drawing.Point(229, 121);
            this.dateTimePicker2.Name = "dateTimePicker2";
            this.dateTimePicker2.Size = new System.Drawing.Size(200, 19);
            this.dateTimePicker2.TabIndex = 28;
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Location = new System.Drawing.Point(17, 121);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(200, 19);
            this.dateTimePicker1.TabIndex = 27;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.label11);
            this.panel4.Controls.Add(this.label10);
            this.panel4.Controls.Add(this.label9);
            this.panel4.Controls.Add(this.label8);
            this.panel4.Controls.Add(this.label7);
            this.panel4.Controls.Add(this.textBoxW);
            this.panel4.Controls.Add(this.textBoxF);
            this.panel4.Controls.Add(this.textBoxT);
            this.panel4.Controls.Add(this.textBoxTh);
            this.panel4.Controls.Add(this.textBoxM);
            this.panel4.Location = new System.Drawing.Point(12, 197);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(394, 153);
            this.panel4.TabIndex = 25;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(252, 33);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(29, 12);
            this.label11.TabIndex = 26;
            this.label11.Text = "水曜";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(126, 87);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(29, 12);
            this.label10.TabIndex = 26;
            this.label10.Text = "金曜";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(126, 33);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(29, 12);
            this.label9.TabIndex = 26;
            this.label9.Text = "火曜";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 87);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(29, 12);
            this.label8.TabIndex = 26;
            this.label8.Text = "木曜";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 33);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(29, 12);
            this.label7.TabIndex = 26;
            this.label7.Text = "月曜";
            // 
            // textBoxW
            // 
            this.textBoxW.Location = new System.Drawing.Point(254, 48);
            this.textBoxW.Multiline = true;
            this.textBoxW.Name = "textBoxW";
            this.textBoxW.Size = new System.Drawing.Size(86, 19);
            this.textBoxW.TabIndex = 2;
            // 
            // textBoxF
            // 
            this.textBoxF.Location = new System.Drawing.Point(128, 102);
            this.textBoxF.Multiline = true;
            this.textBoxF.Name = "textBoxF";
            this.textBoxF.Size = new System.Drawing.Size(86, 19);
            this.textBoxF.TabIndex = 4;
            // 
            // textBoxT
            // 
            this.textBoxT.Location = new System.Drawing.Point(128, 48);
            this.textBoxT.Multiline = true;
            this.textBoxT.Name = "textBoxT";
            this.textBoxT.Size = new System.Drawing.Size(86, 19);
            this.textBoxT.TabIndex = 1;
            // 
            // textBoxTh
            // 
            this.textBoxTh.Location = new System.Drawing.Point(3, 102);
            this.textBoxTh.Multiline = true;
            this.textBoxTh.Name = "textBoxTh";
            this.textBoxTh.Size = new System.Drawing.Size(86, 19);
            this.textBoxTh.TabIndex = 3;
            // 
            // textBoxM
            // 
            this.textBoxM.Location = new System.Drawing.Point(3, 48);
            this.textBoxM.Multiline = true;
            this.textBoxM.Name = "textBoxM";
            this.textBoxM.Size = new System.Drawing.Size(86, 19);
            this.textBoxM.TabIndex = 0;
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(302, 537);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(75, 23);
            this.button7.TabIndex = 0;
            this.button7.Text = "登録";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(227, 96);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(45, 12);
            this.label4.TabIndex = 19;
            this.label4.Text = " 終了日";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 96);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 12);
            this.label5.TabIndex = 19;
            this.label5.Text = "開始日";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.SystemColors.ControlLight;
            this.label3.Font = new System.Drawing.Font("MS UI Gothic", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label3.Location = new System.Drawing.Point(10, 375);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(606, 126);
            this.label3.TabIndex = 19;
            this.label3.Text = "開始日及び終了日をDateTimePickerを選択し\r\n日直者登録を行いたい者の担当の曜日が書かれたTextBox内\r\nに学生番号(例0001,0002)を入力" +
        "してください。\r\n登録ボタンをクリックすることで日直者の登録が行えます。\r\n\r\nカンマ(\",\")もしくはハイフン(\"-\")で学生番号を区切ることで複数同時登録が" +
        "行えます。\r\n\r\n";
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(373, 750);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(92, 23);
            this.button6.TabIndex = 14;
            this.button6.Text = "日直予定表示";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);

            // 
            // dataGridView2
            // 
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.Dock = System.Windows.Forms.DockStyle.Right;
            this.dataGridView2.Location = new System.Drawing.Point(1650, 0);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.RowTemplate.Height = 21;
            this.dataGridView2.Size = new System.Drawing.Size(274, 891);
            this.dataGridView2.TabIndex = 13;
            this.dataGridView2.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView2_CellContentClick);
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.label17);
            this.panel5.Controls.Add(this.comboBox1);
            this.panel5.Controls.Add(this.buttonRemove);
            this.panel5.Controls.Add(this.listBoxStudentNumbers);
            this.panel5.Controls.Add(this.buttonAdd);
            this.panel5.Controls.Add(this.textBoxStudentNumber);
            this.panel5.Controls.Add(this.label13);
            this.panel5.Controls.Add(this.label1);
            this.panel5.Controls.Add(this.label12);
            this.panel5.Controls.Add(this.dateTimePicker3);
            this.panel5.Controls.Add(this.dateTimePicker4);
            this.panel5.Controls.Add(this.buttonRegister);
            this.panel5.Controls.Add(this.panel7);
            this.panel5.Controls.Add(this.label19);
            this.panel5.Controls.Add(this.label20);
            this.panel5.Controls.Add(this.label21);
            this.panel5.Controls.Add(this.label22);
            this.panel5.Location = new System.Drawing.Point(1289, 12);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(698, 579);
            this.panel5.TabIndex = 30;
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
      "1",
      "2",
      "3",
      "4",
      "5"});
            this.comboBox1.Location = new System.Drawing.Point(262, 221);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 20);
            this.comboBox1.TabIndex = 34;
            // 
            // buttonRemove
            // 
            this.buttonRemove.Location = new System.Drawing.Point(94, 258);
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.Size = new System.Drawing.Size(75, 23);
            this.buttonRemove.TabIndex = 33;
            this.buttonRemove.Text = "削除";
            this.buttonRemove.UseVisualStyleBackColor = true;
            this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
            // 
            // listBoxStudentNumbers
            // 
            this.listBoxStudentNumbers.FormattingEnabled = true;
            this.listBoxStudentNumbers.ItemHeight = 12;
            this.listBoxStudentNumbers.Location = new System.Drawing.Point(414, 155);
            this.listBoxStudentNumbers.Name = "listBoxStudentNumbers";
            this.listBoxStudentNumbers.Size = new System.Drawing.Size(99, 184);
            this.listBoxStudentNumbers.TabIndex = 32;
            // 
            // buttonAdd
            // 
            this.buttonAdd.Location = new System.Drawing.Point(94, 219);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(75, 23);
            this.buttonAdd.TabIndex = 31;
            this.buttonAdd.Text = "追加";
            this.buttonAdd.UseVisualStyleBackColor = true;
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // textBoxStudentNumber
            // 
            this.textBoxStudentNumber.Location = new System.Drawing.Point(16, 221);
            this.textBoxStudentNumber.Name = "textBoxStudentNumber";
            this.textBoxStudentNumber.Size = new System.Drawing.Size(72, 19);
            this.textBoxStudentNumber.TabIndex = 30;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(261, 206);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(123, 12);
            this.label13.TabIndex = 29;
            this.label13.Text = "一日あたり配置する人数";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 167);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 29;
            this.label1.Text = "学生番号\r\n";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(11, 155);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(342, 24);
            this.label12.TabIndex = 29;
            this.label12.Text = "自動振り分けリストに含む学生の学生番号を以下のtextBoxに入力して\r\n追加ボタンを押してlistBoxに追加していってください。\r\n";
            // 
            // dateTimePicker3
            // 
            this.dateTimePicker3.Location = new System.Drawing.Point(12, 109);
            this.dateTimePicker3.Name = "dateTimePicker3";
            this.dateTimePicker3.Size = new System.Drawing.Size(200, 19);
            this.dateTimePicker3.TabIndex = 28;
            // 
            // dateTimePicker4
            // 
            this.dateTimePicker4.Location = new System.Drawing.Point(221, 109);
            this.dateTimePicker4.Name = "dateTimePicker4";
            this.dateTimePicker4.Size = new System.Drawing.Size(200, 19);
            this.dateTimePicker4.TabIndex = 27;
            // 
            // buttonRegister
            // 
            this.buttonRegister.Location = new System.Drawing.Point(302, 537);
            this.buttonRegister.Name = "buttonRegister";
            this.buttonRegister.Size = new System.Drawing.Size(75, 23);
            this.buttonRegister.TabIndex = 24;
            this.buttonRegister.Text = "登録";
            this.buttonRegister.UseVisualStyleBackColor = true;
            this.buttonRegister.Click += new System.EventHandler(this.buttonRegister_Click);
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.checkBox6);
            this.panel7.Controls.Add(this.checkBox5);
            this.panel7.Controls.Add(this.checkBox4);
            this.panel7.Controls.Add(this.checkBox3);
            this.panel7.Controls.Add(this.checkBox2);
            this.panel7.Controls.Add(this.checkBox1);
            this.panel7.Location = new System.Drawing.Point(12, 328);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(368, 22);
            this.panel7.TabIndex = 20;
            // 
            // checkBox6
            // 
            this.checkBox6.AutoSize = true;
            this.checkBox6.Location = new System.Drawing.Point(213, 3);
            this.checkBox6.Name = "checkBox6";
            this.checkBox6.Size = new System.Drawing.Size(36, 16);
            this.checkBox6.TabIndex = 5;
            this.checkBox6.Text = "土";
            this.checkBox6.UseVisualStyleBackColor = true;
            // 
            // checkBox5
            // 
            this.checkBox5.AutoSize = true;
            this.checkBox5.Location = new System.Drawing.Point(171, 3);
            this.checkBox5.Name = "checkBox5";
            this.checkBox5.Size = new System.Drawing.Size(36, 16);
            this.checkBox5.TabIndex = 4;
            this.checkBox5.Text = "金";
            this.checkBox5.UseVisualStyleBackColor = true;
            // 
            // checkBox4
            // 
            this.checkBox4.AutoSize = true;
            this.checkBox4.Location = new System.Drawing.Point(129, 3);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(36, 16);
            this.checkBox4.TabIndex = 3;
            this.checkBox4.Text = "木";
            this.checkBox4.UseVisualStyleBackColor = true;
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Location = new System.Drawing.Point(87, 3);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(36, 16);
            this.checkBox3.TabIndex = 2;
            this.checkBox3.Text = "水";
            this.checkBox3.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(45, 3);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(36, 16);
            this.checkBox2.TabIndex = 1;
            this.checkBox2.Text = "火";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(3, 3);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(36, 16);
            this.checkBox1.TabIndex = 0;
            this.checkBox1.Text = "月";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(222, 84);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(45, 12);
            this.label19.TabIndex = 19;
            this.label19.Text = " 終了日";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(10, 84);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(41, 12);
            this.label20.TabIndex = 19;
            this.label20.Text = "開始日";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.BackColor = System.Drawing.SystemColors.ControlLight;
            this.label21.Font = new System.Drawing.Font("MS UI Gothic", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label21.Location = new System.Drawing.Point(10, 375);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(543, 90);
            this.label21.TabIndex = 19;
            this.label21.Text = "1.日直を入力する区間をDatePickerで開始日と終了日選択\r\n2.自動で振り分けを行う学生番号を入力し追加buttonを押してリストに格納\r\n3.各曜日に何" +
        "人配置するかを選択する。(1-5)\r\n4.自動振り分けを行う曜日を選択\r\n5.登録buttonを押し日直DBへ登録";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(10, 306);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(207, 12);
            this.label22.TabIndex = 19;
            this.label22.Text = "自動振り分けを行う曜日を選択してください";
            // 
            // radioButton11
            // 
            this.radioButton11.AutoSize = true;
            this.radioButton11.Location = new System.Drawing.Point(596, 779);
            this.radioButton11.Name = "radioButton11";
            this.radioButton11.Size = new System.Drawing.Size(71, 16);
            this.radioButton11.TabIndex = 31;
            this.radioButton11.TabStop = true;
            this.radioButton11.Text = "罰直登録";
            this.radioButton11.UseVisualStyleBackColor = true;
            this.radioButton11.CheckedChanged += new System.EventHandler(this.radioButton11_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.SystemColors.ControlLight;
            this.label2.Font = new System.Drawing.Font("MS UI Gothic", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label2.Location = new System.Drawing.Point(-139, 306);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 54);
            this.label2.TabIndex = 20;
            this.label2.Text = "罰直登録\r\n\r\n\r\n";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("MS UI Gothic", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label14.Location = new System.Drawing.Point(110, 219);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(332, 45);
            this.label14.TabIndex = 30;
            this.label14.Text = "罰直登録を行う日付をカレンダーから選択してください。\r\n下のテキストボックスに学生番号を入れて登録を押すと、\r\n罰直として登録が行われます";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("MS UI Gothic", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label15.Location = new System.Drawing.Point(215, 9);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(178, 27);
            this.label15.TabIndex = 31;
            this.label15.Text = "罰直登録Form";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("MS UI Gothic", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label16.Location = new System.Drawing.Point(12, 26);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(259, 27);
            this.label16.TabIndex = 32;
            this.label16.Text = "曜日別日直登録Form";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("MS UI Gothic", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label17.Location = new System.Drawing.Point(7, 26);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(320, 27);
            this.label17.TabIndex = 35;
            this.label17.Text = "自動日直割り振り登録Form";
            // 
            // Form5
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1924, 891);
            this.Controls.Add(this.radioButton11);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.radioButton2);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.radioButton1);
            this.Controls.Add(this.dataGridView2);
            this.Controls.Add(this.button6);
            this.Name = "Form5";
            this.Text = "Form5";
            this.Load += new System.EventHandler(this.Form5_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MonthCalendar monthCalendar1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.DataGridView dataGridView2;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBoxW;
        private System.Windows.Forms.TextBox textBoxF;
        private System.Windows.Forms.TextBox textBoxT;
        private System.Windows.Forms.TextBox textBoxTh;
        private System.Windows.Forms.TextBox textBoxM;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.DateTimePicker dateTimePicker2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.TextBox textBoxStudentNumber;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.DateTimePicker dateTimePicker3;
        private System.Windows.Forms.DateTimePicker dateTimePicker4;
        private System.Windows.Forms.Button buttonRegister;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.RadioButton radioButton11;
        private System.Windows.Forms.CheckBox checkBox6;
        private System.Windows.Forms.CheckBox checkBox5;
        private System.Windows.Forms.CheckBox checkBox4;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button buttonRemove;
        private System.Windows.Forms.ListBox listBoxStudentNumbers;
        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        // Googleフォーム読み込みUI
        private System.Windows.Forms.ComboBox comboBoxMonth;
        private System.Windows.Forms.Button buttonLoadFromGoogleForm;
        private System.Windows.Forms.DataGridView dataGridViewNote;
        private System.Windows.Forms.Label labelMonth;
        private System.Windows.Forms.Label labelNote;

    }
}