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
    public partial class Form4 : Form
    {
        private Setting mySqlSet;
        public Form4(Setting settings)
        {
            InitializeComponent();
            this.mySqlSet = settings;

        }

        private void MenuForm_Load(object sender, EventArgs e)
        {

        }

            private void button2_Click(object sender, EventArgs e)
        {
            //日直管理システム
            Form5 form5 = new Form5(mySqlSet);
            form5.ShowDialog();
        }


        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            using (Form3 configWindow = new Form3(mySqlSet))
            {
                configWindow.ShowDialog();
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            //日直変更Form
            Form7 form7 = new Form7(mySqlSet);
            form7.ShowDialog();
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            Form6 form6 = new Form6(mySqlSet);
            form6.ShowDialog();
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            //文献管理システム
            Form8 form8 = new Form8(mySqlSet);
            form8.ShowDialog();
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            //日直登録システム
            Form9 form9 = new Form9(mySqlSet);
            form9.ShowDialog();
        }


        private void button7_Click_1(object sender, EventArgs e)
        {
            //カレンダーシステム
            Form10 form10 = new Form10(mySqlSet);  
            form10.ShowDialog();
        }



        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
