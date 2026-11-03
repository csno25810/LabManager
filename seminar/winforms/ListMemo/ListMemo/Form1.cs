using System;
using System.Windows.Forms;

namespace ListMemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string text = textBox1.Text.Trim();
            if (text.Length == 0)
            {
                return;
            }

            listBox1.Items.Add(text);
            textBox1.Clear();
            textBox1.Focus();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null)
            {
                return;
            }

            label1.Text = listBox1.SelectedItem.ToString();
        }
    }
}
