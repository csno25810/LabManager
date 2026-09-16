using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Data;

namespace LabManager
{
    static class Program
    {
        /// <summary>
        /// 研究室テレビPC用。Screen.bat から /tv で起動すると
        /// メイン画面（右半分）と大学カレンダー（左半分）を同時表示する。
        /// </summary>
        public static bool TvMode { get; private set; }

        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            TvMode = args.Any(arg =>
                string.Equals(arg, "/tv", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(arg, "-tv", StringComparison.OrdinalIgnoreCase));

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // DB 接続は Form1 の GetConnection() に一任する。
            // 接続に失敗してもアプリは起動し、未接続モードとして UI を触れるようにする。
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
        /// <summary>
        /// 出席状況編集のパスワード。空ならパスワード不要。
        /// SQLReader.ini の DebugPassword で設定する。
        /// </summary>
        public string DebugPassword = "";

        private string DirPos = "C:\\MyReader\\";
        private string FileName = "SQLReader.ini";

        // Form8 が要求しているため追加（他は変更なし）
        public string ConnectionString
        {
            get
            {
                return $"Server={ServerIP};Database={DataBaseName};Uid={UserID};Pwd={PassWd};CharSet=utf8mb4;";
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
                if (!string.IsNullOrEmpty(DebugPassword))
                    makeFile.WriteLine("DebugPassword =" + DebugPassword);
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
                            case "DebugPassword": DebugPassword = value; break;
                        }
                    }
                }
                return true;
            }

            return false;
        }
    }

    /// <summary>
    /// Google カレンダー連携設定（旧 GoogleCalenderReader.ini 互換）
    /// </summary>
    public class CalendarSetting
    {
        public string CalendarName = "";
        public string DefaultQuery = "";
        public string IcsUrl = "";
        public int ReloadTime = 100;

        private readonly string _dirPos = "C:\\MyReader\\";
        private readonly string _fileName = "GoogleCalenderReader.ini";

        public string ConfigPath => _dirPos + _fileName;

        public bool ReadSetting()
        {
            if (!Directory.Exists(_dirPos) || !File.Exists(ConfigPath))
                return false;

            var values = new System.Collections.Generic.Dictionary<string, string>();
            using (var readFile = new StreamReader(ConfigPath))
            {
                string line;
                while ((line = readFile.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line) || !line.Contains("="))
                        continue;

                    var parts = line.Split(new[] { '=' }, 2);
                    values[parts[0].Trim()] = parts[1].Trim();
                }
            }

            if (values.TryGetValue("CalenderName", out var calendarName))
                CalendarName = calendarName;
            if (values.TryGetValue("DefaultQuery", out var defaultQuery))
                DefaultQuery = defaultQuery;
            if (values.TryGetValue("IcsUrl", out var icsUrl))
                IcsUrl = icsUrl;
            if (values.TryGetValue("ReloadTime", out var reloadTime) && int.TryParse(reloadTime, out var parsed))
                ReloadTime = parsed;

            return !string.IsNullOrWhiteSpace(GetIcsUrl());
        }

        /// <summary>
        /// ICS 取得 URL を返す。IcsUrl があれば優先、なければ CalenderName + DefaultQuery から組み立てる。
        /// </summary>
        public string GetIcsUrl()
        {
            if (!string.IsNullOrWhiteSpace(IcsUrl))
                return IcsUrl.Trim();

            if (string.IsNullOrWhiteSpace(CalendarName) || string.IsNullOrWhiteSpace(DefaultQuery))
                return null;

            var query = DefaultQuery.Trim();
            if (!query.StartsWith("/"))
                query = "/" + query;
            if (!query.EndsWith(".ics", StringComparison.OrdinalIgnoreCase))
                query = query.TrimEnd('/') + ".ics";

            var encodedName = CalendarName.Replace("@", "%40");
            return "https://calendar.google.com/calendar/ical/" + encodedName + query;
        }
    }

    class Connector
    {
        private static MySqlConnection conn;

        // 現在 DB 接続が確立されているかどうか。
        // 未接続のときは TableReader / ExecuteCommand がエラーを出さず即 return する。
        public static bool IsConnected { get; private set; } = false;

        public static bool Connect(string user, string password, string dbname, string ip)
        {
            IsConnected = false;

            try { conn?.Dispose(); } catch { /* 古い接続の破棄失敗は無視 */ }

            string connstr = $"Server={ip};Database={dbname};Uid={user};Pwd={password};CharSet=utf8mb4;";
            conn = new MySqlConnection(connstr);

            try
            {
                conn.Open();

                // MySQL 8.0 の既定 sql_mode には ONLY_FULL_GROUP_BY が含まれており、
                // 旧コードの GROUP BY を伴うクエリが拒否される。
                // 旧 MySQL の挙動に合わせるため、本接続のセッションでのみ無効化する。
                try
                {
                    using (var modeCmd = new MySqlCommand(
                        "SET SESSION sql_mode = " +
                        "(SELECT REPLACE(REPLACE(@@sql_mode, 'ONLY_FULL_GROUP_BY,', ''), 'ONLY_FULL_GROUP_BY', ''))",
                        conn))
                    {
                        modeCmd.ExecuteNonQuery();
                    }
                }
                catch (MySqlException) { /* 設定失敗は致命的ではないので無視 */ }

                IsConnected = true;
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
            if (!IsConnected) return false;
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
            if (!IsConnected) return;
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

    /// <summary>
    /// duty_schedule の手動編集ログ（不正防止用）。
    /// </summary>
    /// <summary>
    /// 日本語表示可能な UI フォント。メイリオ未インストール環境でも文字化けしないようフォールバックする。
    /// </summary>
    public static class UiFonts
    {
        private static readonly string[] PreferredFamilies =
        {
            "Yu Gothic UI", "Meiryo UI", "メイリオ", "MS UI Gothic", "MS Gothic", "Segoe UI"
        };

        public static Font Get(float size, FontStyle style = FontStyle.Regular)
        {
            foreach (var familyName in PreferredFamilies)
            {
                if (!FontFamily.Families.Any(f => f.Name.Equals(familyName, StringComparison.OrdinalIgnoreCase)))
                    continue;

                return new Font(familyName, size, style, GraphicsUnit.Point, 128);
            }

            return new Font(SystemFonts.DefaultFont.FontFamily, size, style);
        }
    }

    static class DutyAuditLog
    {
        public static void EnsureTable()
        {
            const string sql = @"
                CREATE TABLE IF NOT EXISTS duty_edit_log (
                    id INT NOT NULL AUTO_INCREMENT,
                    edited_at DATETIME NOT NULL,
                    action VARCHAR(10) NOT NULL,
                    duty_date DATE NOT NULL,
                    student_id VARCHAR(20) NOT NULL,
                    old_duty_status INT NULL,
                    new_duty_status INT NULL,
                    old_duty_type VARCHAR(10) NULL,
                    new_duty_type VARCHAR(10) NULL,
                    PRIMARY KEY (id),
                    KEY idx_duty_edit_time (edited_at),
                    KEY idx_duty_edit_date (duty_date)
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8";
            Connector.ExecuteCommand(sql);
        }

        public static void Record(
            string action,
            string dutyDate,
            string studentId,
            int? oldStatus,
            int? newStatus,
            string oldType,
            string newType)
        {
            string oldStatusSql = oldStatus.HasValue ? oldStatus.Value.ToString() : "NULL";
            string newStatusSql = newStatus.HasValue ? newStatus.Value.ToString() : "NULL";
            string oldTypeSql = oldType != null ? $"'{Escape(oldType)}'" : "NULL";
            string newTypeSql = newType != null ? $"'{Escape(newType)}'" : "NULL";

            string sql = $@"
                INSERT INTO duty_edit_log
                    (edited_at, action, duty_date, student_id,
                     old_duty_status, new_duty_status, old_duty_type, new_duty_type)
                VALUES
                    (NOW(), '{Escape(action)}', '{Escape(dutyDate)}', '{Escape(studentId)}',
                     {oldStatusSql}, {newStatusSql}, {oldTypeSql}, {newTypeSql})";
            Connector.ExecuteCommand(sql);
        }

        private static string Escape(string value)
        {
            return (value ?? "").Replace("'", "''");
        }
    }
}