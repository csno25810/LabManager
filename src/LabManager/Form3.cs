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
    public partial class Form3 : Form
    {
        int winHeight = (int)(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height * 0.8);
        int winWidth =  (int)(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width * 0.8);

        int ParticlePos = 240;
        int margin = 12;
        int windowHeader = 50;

        DataTable CustomTable = new DataTable();

        Setting ConnectionData = null;

        public Form3(Setting mySqlSet)
        {
            ConnectionData = mySqlSet;

            InitializeComponent();

            // Form配置
            this.Size = new System.Drawing.Size(winWidth, winHeight);
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(winWidth / 8, winHeight/8);

            // dataGridView配置
            dataGridView1.Location = new Point(ParticlePos,margin);
            dataGridView1.Size = new Size(winWidth - margin - margin - ParticlePos,winHeight - windowHeader);

        }

        public static string ConvertEncoding(string src, System.Text.Encoding destEnc)
        {
            byte[] src_temp = System.Text.Encoding.ASCII.GetBytes(src);
            byte[] dest_temp = System.Text.Encoding.Convert(System.Text.Encoding.ASCII, destEnc, src_temp);
            string ret = destEnc.GetString(dest_temp);
            return ret;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked) 
            {
                CustomTable = new DataTable();

                string SelectDay = monthCalendar1.SelectionStart.ToString("yyyy-MM-dd");
                Connector.Connect(ConnectionData.UserID, ConnectionData.PassWd, ConnectionData.DataBaseName, ConnectionData.ServerIP);
                Connector.TableReader("SELECT personal_info.student_id,name,MIN(date_format(time_stamp,'%H:%i')), MAX(date_format(time_stamp,'%H:%i')),COUNT(name) FROM((touch_log INNER JOIN chip_list ON touch_log.chip_id = chip_list.chip_id)INNER JOIN personal_info ON chip_list.student_id = personal_info.student_id) WHERE touch_log.time_stamp LIKE \"" + SelectDay + "%\"GROUP BY name", CustomTable);

                CustomTable.Columns[0].ColumnName = "学籍番号";
                CustomTable.Columns[1].ColumnName = "氏名";
                CustomTable.Columns[2].ColumnName = "初回タッチ時刻";
                CustomTable.Columns[3].ColumnName = "最終タッチ時刻";
                CustomTable.Columns[4].ColumnName = "タッチ回数";

                dataGridView1.DataSource = CustomTable;
            }

            if (radioButton2.Checked) 
            {
                CustomTable = new DataTable();

                string QueryStrings = "SELECT personal_info.student_id,name,time_stamp FROM((touch_log INNER JOIN chip_list ON touch_log.chip_id = chip_list.chip_id)INNER JOIN personal_info ON chip_list.student_id = personal_info.student_id) WHERE personal_info.student_id = \"" + textBox1.Text + "\"";
                Connector.Connect(ConnectionData.UserID, ConnectionData.PassWd, ConnectionData.DataBaseName, ConnectionData.ServerIP);
                Connector.TableReader(QueryStrings, CustomTable);

                CustomTable.Columns[0].ColumnName = "学籍番号";
                CustomTable.Columns[1].ColumnName = "氏名";
                CustomTable.Columns[2].ColumnName = "タッチ時刻";

                dataGridView1.DataSource = CustomTable;
            }

            if (radioButton3.Checked) 
            {
                DialogResult result = MessageBox.Show("この操作を行う場合SQL文には細心の注意を払ってください。\nまた、読み取り文以外の実行は行わないでください。\n上記を踏まえ実行する場合はYesをしない場合はNoをクリックしてください。","警告", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);

                if (result == DialogResult.Yes) 
                {
                    Connector.Connect(ConnectionData.UserID, ConnectionData.PassWd, ConnectionData.DataBaseName, ConnectionData.ServerIP);
                    Connector.TableReader(textBox2.Text, CustomTable);

                    dataGridView1.DataSource = CustomTable;
                }

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        
    }
}
