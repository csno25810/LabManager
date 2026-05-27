using System;
using System.IO;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Data;

namespace MySQL_ConnectionTest
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Setting setting = new Setting();
            setting.ReadSetting(); // ini 読み込み（あれば）

            bool connected = Connector.Connect(
                setting.UserID,
                setting.PassWd,
                setting.DataBaseName,
                setting.ServerIP
            );

            if (!connected)
            {
                MessageBox.Show("データベースに接続できません。");
                return;
            }

            Application.Run(new Form1());
        }
    }
    public class Setting
    {
        public string UserID = "userid";
        public string PassWd = "passwd";
        public string ServerIP = "127.0.0.1";
        public string DataBaseName = "felica";
        public int ReloadTime = 30;

        private string DirPos = "C:\\MyReader\\";
        private string FileName = "SQLReader.ini";

        // Form8 が要求しているため追加（他は変更なし）
        public string ConnectionString
        {
            get
            {
                return $"Server={ServerIP};Database={DataBaseName};Uid={UserID};Pwd={PassWd};Charset=utf8;";
            }
        }

        public bool CheckFile()
        {
            return !File.Exists(DirPos + FileName);
        }

        public void WriteFile()
        {
            using (StreamWriter makeFile = new StreamWriter(DirPos + FileName))
            {
                makeFile.WriteLine("UserID =" + UserID);
                makeFile.WriteLine("PassWd =" + PassWd);
                makeFile.WriteLine("ServerIP =" + ServerIP);
                makeFile.WriteLine("ReloadTime =" + ReloadTime.ToString());
                makeFile.WriteLine("DataBaseName =" + DataBaseName);
            }
        }

        public bool CheckDirectory()
        {
            return !Directory.Exists(DirPos);
        }

        public bool CreateFile()
        {
            if (CheckDirectory())
            {
                Directory.CreateDirectory(DirPos);
                WriteFile();
                return true;
            }

            return false;
        }

        public bool ReadSetting()
        {
            if (!CheckDirectory() && !CheckFile())
            {
                using (StreamReader readFile = new StreamReader(DirPos + FileName))
                {
                    string line;
                    while ((line = readFile.ReadLine()) != null)
                    {
                        var key = line.Split('=')[0].Trim();
                        var value = line.Split('=')[1].Trim();

                        switch (key)
                        {
                            case "UserID": UserID = value; break;
                            case "PassWd": PassWd = value; break;
                            case "ServerIP": ServerIP = value; break;
                            case "ReloadTime": ReloadTime = int.Parse(value); break;
                            case "DataBaseName": DataBaseName = value; break;
                        }
                    }
                }
                return true;
            }

            return false;
        }
    }

    class Connector
    {
        private static MySqlConnection conn;

        public static bool Connect(string user, string password, string dbname, string ip)
        {
            string connstr = $"Server={ip};Database={dbname};Uid={user};Pwd={password};Charset=utf8;";
            conn = new MySqlConnection(connstr);

            try
            {
                conn.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("接続に失敗しました\n" + ex.Message);
                return false;
            }
        }

        public static bool TableReader(string sql, DataTable table)
        {
            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter(sql, conn);
                da.Fill(table);
                return true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("データ取得エラー\n" + ex.Message);
                return false;
            }
        }

        public static void ExecuteCommand(string sql)
        {
            try
            {
                new MySqlCommand(sql, conn).ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("SQL 実行エラー\n" + ex.Message);
            }
        }
    }
}