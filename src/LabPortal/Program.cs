using System;

namespace LabPortal
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            int port = 8080;
            if (args.Length > 0)
            {
                int parsed;
                if (int.TryParse(args[0], out parsed) && parsed > 0 && parsed < 65536)
                    port = parsed;
            }

            string user;
            string password;
            string server;
            string database;
            string error;
            if (!IniSettings.TryRead(out user, out password, out server, out database, out error))
            {
                Console.WriteLine(error);
                Console.WriteLine("Enter で終了します。");
                Console.ReadLine();
                return 1;
            }

            string googleClientId;
            string googleClientSecret;
            IniSettings.TryReadGoogle(out googleClientId, out googleClientSecret);

            string connectionString =
                "Server=" + server + ";Database=" + database + ";Uid=" + user + ";Pwd=" + password + ";CharSet=utf8mb4;";
            var portal = new PortalService(connectionString, server, googleClientId, googleClientSecret);

            try
            {
                portal.PrepareDatabase();
            }
            catch (Exception ex)
            {
                Console.WriteLine("データベースに接続できませんでした。");
                Console.WriteLine(ex.Message);
                Console.WriteLine("C:\\MyReader\\SQLReader.ini の ServerIP / UserID / PassWd を確認してください。");
                Console.WriteLine("Enter で終了します。");
                Console.ReadLine();
                return 1;
            }

            if (string.IsNullOrEmpty(googleClientId) || string.IsNullOrEmpty(googleClientSecret))
            {
                Console.WriteLine("Gmailログイン: 未設定。C:\\MyReader\\LabPortal.ini に GoogleClientId / GoogleClientSecret を書いてください。");
            }
            else
            {
                Console.WriteLine("Gmailログイン: 有効");
            }

            try
            {
                TinyHttpServer.Listen(port, portal.Handle);
            }
            catch (Exception ex)
            {
                Console.WriteLine("HTTP サーバを起動できませんでした（ポート " + port + "）。");
                Console.WriteLine(ex.Message);
                Console.WriteLine("Enter で終了します。");
                Console.ReadLine();
                return 1;
            }

            return 0;
        }
    }
}
