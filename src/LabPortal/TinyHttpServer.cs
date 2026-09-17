using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Web;

namespace LabPortal
{
    public sealed class HttpRequest
    {
        public string Method { get; set; }
        public string Path { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public Dictionary<string, string> Cookies { get; set; }
        public Dictionary<string, string> Form { get; set; }
        public string Body { get; set; }

        public string Cookie(string name)
        {
            string value;
            return Cookies != null && Cookies.TryGetValue(name, out value) ? value : null;
        }

        public string FormValue(string name)
        {
            string value;
            return Form != null && Form.TryGetValue(name, out value) ? value : "";
        }
    }

    public sealed class HttpResponse
    {
        public int Status { get; set; }
        public string ContentType { get; set; }
        public string Body { get; set; }
        public string Location { get; set; }
        public string SetCookie { get; set; }
        public string ExtraHeaders { get; set; }

        public static HttpResponse Html(string html, int status = 200)
        {
            return new HttpResponse
            {
                Status = status,
                ContentType = "text/html; charset=utf-8",
                Body = html ?? ""
            };
        }

        public static HttpResponse Redirect(string location)
        {
            return new HttpResponse
            {
                Status = 302,
                ContentType = "text/html; charset=utf-8",
                Body = "",
                Location = location
            };
        }
    }

    public static class TinyHttpServer
    {
        public static void Listen(int port, Func<HttpRequest, HttpResponse> handler)
        {
            var listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            Console.WriteLine("LabPortal を起動しました。ブラウザで開いてください。");
            Console.WriteLine("  このPC: http://localhost:" + port + "/");
            try
            {
                foreach (var address in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
                {
                    if (address.AddressFamily == AddressFamily.InterNetwork)
                        Console.WriteLine("  他デバイス: http://" + address + ":" + port + "/");
                }
            }
            catch
            {
                // ホスト名解決に失敗しても localhost では動く
            }
            Console.WriteLine("終了するには Ctrl+C");

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                ThreadPool.QueueUserWorkItem(_ => Serve(client, handler));
            }
        }

        private static void Serve(TcpClient client, Func<HttpRequest, HttpResponse> handler)
        {
            try
            {
                using (client)
                using (NetworkStream stream = client.GetStream())
                {
                    stream.ReadTimeout = 15000;
                    stream.WriteTimeout = 15000;
                    HttpRequest request = ReadRequest(stream);
                    if (request == null)
                        return;

                    HttpResponse response;
                    try
                    {
                        response = handler(request) ?? HttpResponse.Html("empty", 500);
                    }
                    catch (Exception ex)
                    {
                        response = HttpResponse.Html(Page("エラー", "<p>処理に失敗しました。</p><pre>" +
                            HttpUtility.HtmlEncode(ex.Message) + "</pre>"), 500);
                    }

                    WriteResponse(stream, response);
                }
            }
            catch
            {
                // 切断は無視
            }
        }

        private static string Page(string title, string inner)
        {
            return "<!doctype html><html lang=\"ja\"><head><meta charset=\"utf-8\"><title>" +
                   title + "</title></head><body>" + inner + "</body></html>";
        }

        private static HttpRequest ReadRequest(NetworkStream stream)
        {
            var headerBuffer = new MemoryStream();
            int match = 0;
            byte[] one = new byte[1];
            while (headerBuffer.Length < 16384)
            {
                int n = stream.Read(one, 0, 1);
                if (n <= 0)
                    return null;
                headerBuffer.WriteByte(one[0]);
                if (one[0] == (match == 0 || match == 2 ? (byte)13 : (byte)10))
                    match++;
                else
                    match = one[0] == 13 ? 1 : 0;
                if (match == 4)
                    break;
            }

            string headerText = Encoding.UTF8.GetString(headerBuffer.ToArray());
            string[] lines = headerText.Split(new[] { "\r\n" }, StringSplitOptions.None);
            if (lines.Length == 0)
                return null;

            string[] parts = lines[0].Split(' ');
            if (parts.Length < 2)
                return null;

            var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            for (int i = 1; i < lines.Length; i++)
            {
                int colon = lines[i].IndexOf(':');
                if (colon <= 0)
                    continue;
                headers[lines[i].Substring(0, colon).Trim()] = lines[i].Substring(colon + 1).Trim();
            }

            string body = "";
            string lengthText;
            if (headers.TryGetValue("Content-Length", out lengthText))
            {
                int length;
                if (int.TryParse(lengthText, out length) && length > 0)
                {
                    if (length > 65536)
                        length = 65536;
                    byte[] bodyBytes = new byte[length];
                    int read = 0;
                    while (read < length)
                    {
                        int n = stream.Read(bodyBytes, read, length - read);
                        if (n <= 0)
                            break;
                        read += n;
                    }
                    body = Encoding.UTF8.GetString(bodyBytes, 0, read);
                }
            }

            string rawPath = parts[1];
            int q = rawPath.IndexOf('?');
            string path = q >= 0 ? rawPath.Substring(0, q) : rawPath;
            path = HttpUtility.UrlDecode(path) ?? "/";
            if (path.Length > 1 && path.EndsWith("/"))
                path = path.TrimEnd('/');

            var request = new HttpRequest
            {
                Method = parts[0].ToUpperInvariant(),
                Path = path,
                Headers = headers,
                Body = body,
                Cookies = ParseCookies(headers),
                Form = ParseForm(headers, body)
            };
            return request;
        }

        private static Dictionary<string, string> ParseCookies(Dictionary<string, string> headers)
        {
            var cookies = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            string raw;
            if (!headers.TryGetValue("Cookie", out raw) || string.IsNullOrEmpty(raw))
                return cookies;

            foreach (string part in raw.Split(';'))
            {
                int eq = part.IndexOf('=');
                if (eq <= 0)
                    continue;
                string name = part.Substring(0, eq).Trim();
                string value = part.Substring(eq + 1).Trim();
                cookies[name] = HttpUtility.UrlDecode(value) ?? value;
            }
            return cookies;
        }

        private static Dictionary<string, string> ParseForm(Dictionary<string, string> headers, string body)
        {
            var form = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            string contentType;
            if (!headers.TryGetValue("Content-Type", out contentType))
                return form;
            if (contentType.IndexOf("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase) < 0)
                return form;

            foreach (string pair in (body ?? "").Split('&'))
            {
                if (string.IsNullOrEmpty(pair))
                    continue;
                int eq = pair.IndexOf('=');
                string name = eq >= 0 ? pair.Substring(0, eq) : pair;
                string value = eq >= 0 ? pair.Substring(eq + 1) : "";
                form[HttpUtility.UrlDecode(name) ?? name] = HttpUtility.UrlDecode(value) ?? value;
            }
            return form;
        }

        private static void WriteResponse(NetworkStream stream, HttpResponse response)
        {
            byte[] bodyBytes = Encoding.UTF8.GetBytes(response.Body ?? "");
            string statusText = StatusText(response.Status);
            var sb = new StringBuilder();
            sb.Append("HTTP/1.1 ").Append(response.Status).Append(' ').Append(statusText).Append("\r\n");
            sb.Append("Content-Type: ").Append(response.ContentType ?? "text/html; charset=utf-8").Append("\r\n");
            sb.Append("Content-Length: ").Append(bodyBytes.Length).Append("\r\n");
            sb.Append("Connection: close\r\n");
            sb.Append("Cache-Control: no-store\r\n");
            if (!string.IsNullOrEmpty(response.Location))
                sb.Append("Location: ").Append(response.Location).Append("\r\n");
            if (!string.IsNullOrEmpty(response.SetCookie))
                sb.Append("Set-Cookie: ").Append(response.SetCookie).Append("\r\n");
            if (!string.IsNullOrEmpty(response.ExtraHeaders))
                sb.Append(response.ExtraHeaders);
            sb.Append("\r\n");

            byte[] headerBytes = Encoding.ASCII.GetBytes(sb.ToString());
            stream.Write(headerBytes, 0, headerBytes.Length);
            if (bodyBytes.Length > 0)
                stream.Write(bodyBytes, 0, bodyBytes.Length);
            stream.Flush();
        }

        private static string StatusText(int status)
        {
            switch (status)
            {
                case 200: return "OK";
                case 302: return "Found";
                case 400: return "Bad Request";
                case 401: return "Unauthorized";
                case 404: return "Not Found";
                default: return "Error";
            }
        }
    }
}
