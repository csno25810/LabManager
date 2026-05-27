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
    public partial class Form2 : Form
    {
        Setting mySqlSet = null;

        public Form2(Setting SetData)
        {
            InitializeComponent();

            mySqlSet = SetData;

            textBox1.Text = mySqlSet.UserID;
            textBox2.Text = mySqlSet.PassWd;
            textBox3.Text = mySqlSet.ServerIP;
            textBox4.Text = mySqlSet.DataBaseName;
            textBox5.Text = mySqlSet.ReloadTime.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            mySqlSet.UserID         = textBox1.Text;
            mySqlSet.PassWd         = textBox2.Text;
            mySqlSet.ServerIP       = textBox3.Text;
            mySqlSet.DataBaseName   = textBox4.Text;
            mySqlSet.ReloadTime     = int.Parse( textBox5.Text );

            mySqlSet.WriteFile();

            this.Close();
        }



    }
}
