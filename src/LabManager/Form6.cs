using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MySQL_ConnectionTest
{
    public partial class Form6 : Form
    {
        private Setting mySqlSet;

        public Form6(Setting settings)
        {
            InitializeComponent();
            this.mySqlSet = settings;

        }


        private void button1_Click(object sender, EventArgs e)
        {
            //提出button
            string title = textBox1.Text.Trim();       // タイトル
            string studentId = textBox2.Text.Trim();  // 学生番号
            string content = textBox3.Text.Trim();    // 本文
            if (string.IsNullOrWhiteSpace(title))
            {
                MessageBox.Show("タイトルを入力してください");
            }
            else if (string.IsNullOrWhiteSpace(studentId))
            {
                MessageBox.Show("学籍番号を入力してください");
            }
            else if (string.IsNullOrWhiteSpace(content))
            {
                MessageBox.Show("本文を入力してください");
            }
            else
            {
                InsertDiaryEntry(title, studentId, content);
                textBox1.Clear();
                textBox2.Clear();
                textBox3.Clear();
            }
        }
        private void InsertDiaryEntry(string title, string studentId, string content)
        {
            DateTime Today = DateTime.Today;
            // SQLコマンドを準備
            string commandText = $@"INSERT INTO diary_log (student_id,dialy_date,title,content) VALUES ('{studentId}', '{Today}','{title}','{content}')";
            // SQLコマンドを実行
            Connector.ExecuteCommand(commandText);
            MessageBox.Show("提出が完了しました。");

        }

        private void button2_Click(object sender, EventArgs e)
        {
            //clearButton
            textBox3.Clear();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string title = textBox1.Text.Trim();       // タイトル

        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            string studentId = textBox2.Text.Trim();  // 学生番号

        }
        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            string content = textBox3.Text.Trim();    // 本文
        }
        private void label5_Click(object sender, EventArgs e)
        {

        }
    }
}
