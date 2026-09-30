using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;

namespace LabPortal
{
    public sealed class GoogleSignIn
    {
        private sealed class Pending
        {
            public string RedirectUri { get; set; }
            public DateTime ExpiresUtc { get; set; }
        }

        private readonly string clientId;
        private readonly string clientSecret;
        private readonly ConcurrentDictionary<string, Pending> pending =
            new ConcurrentDictionary<string, Pending>();

        public GoogleSignIn(string clientId, string clientSecret)
        {
            this.clientId = (clientId ?? "").Trim();
            this.clientSecret = (clientSecret ?? "").Trim();
        }

        public bool IsConfigured
        {
            get { return clientId.Length > 0 && clientSecret.Length > 0; }
        }

        public string SetupHint
        {
            get
            {
                if (IsConfigured)
                    return "";
                if (!File.Exists(@"C:\MyReader\LabPortal.ini"))
                    return "C:\\MyReader\\LabPortal.ini がありません。deploy\\LabPortal.ini.sample をコピーしてください。";
                return "C:\\MyReader\\LabPortal.ini はありますが、GoogleClientId と GoogleClientSecret が空です。Google Cloud のクライアント ID とシークレットを書いて、LabPortal を再起動してください。";
            }
        }

        public string AuthorizationUrl(string state, string redirectUri)
        {
            return "https://accounts.google.com/o/oauth2/v2/auth" +
                   "?client_id=" + Uri.EscapeDataString(clientId) +
                   "&redirect_uri=" + Uri.EscapeDataString(redirectUri) +
                   "&response_type=code" +
                   "&scope=" + Uri.EscapeDataString("openid email profile") +
                   "&state=" + Uri.EscapeDataString(state) +
                   "&prompt=select_account";
        }

        public void Remember(string state, string redirectUri)
        {
            Sweep();
            pending[state] = new Pending
            {
                RedirectUri = redirectUri,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(10)
            };
        }

        public bool TryTake(string state, out string redirectUri)
        {
            redirectUri = null;
            Sweep();
            Pending item;
            if (string.IsNullOrEmpty(state) || !pending.TryRemove(state, out item))
                return false;
            if (item.ExpiresUtc < DateTime.UtcNow)
                return false;
            redirectUri = item.RedirectUri;
            return true;
        }

        public bool TryGetEmail(string code, string redirectUri, out string email, out string error)
        {
            email = null;
            error = null;
            if (!IsConfigured)
            {
                error = "Gmailログインが未設定です。";
                return false;
            }

            string tokenJson;
            if (!TryPostForm("https://oauth2.googleapis.com/token",
                    "code=" + Uri.EscapeDataString(code ?? "") +
                    "&client_id=" + Uri.EscapeDataString(clientId) +
                    "&client_secret=" + Uri.EscapeDataString(clientSecret) +
                    "&redirect_uri=" + Uri.EscapeDataString(redirectUri ?? "") +
                    "&grant_type=authorization_code",
                    out tokenJson, out error))
            {
                return false;
            }

            string accessToken = JsonString(tokenJson, "access_token");
            if (string.IsNullOrEmpty(accessToken))
            {
                error = "Google からアクセストークンを取得できませんでした。";
                return false;
            }

            string userJson;
            if (!TryGet("https://www.googleapis.com/oauth2/v2/userinfo", accessToken, out userJson, out error))
                return false;

            email = (JsonString(userJson, "email") ?? "").Trim();
            if (email.Length == 0)
            {
                error = "Google からメールアドレスを取得できませんでした。";
                return false;
            }

            return true;
        }

        private void Sweep()
        {
            DateTime now = DateTime.UtcNow;
            foreach (var pair in pending)
            {
                if (pair.Value.ExpiresUtc < now)
                {
                    Pending removed;
                    pending.TryRemove(pair.Key, out removed);
                }
            }
        }

        private static bool TryPostForm(string url, string body, out string responseText, out string error)
        {
            responseText = null;
            error = null;
            try
            {
                using (var client = NewClient())
                using (var content = new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded"))
                using (HttpResponseMessage response = client.PostAsync(url, content).Result)
                {
                    responseText = response.Content.ReadAsStringAsync().Result;
                    if (!response.IsSuccessStatusCode)
                    {
                        error = "Google トークン交換に失敗しました。";
                        return false;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                error = "Google に接続できませんでした。" + ex.GetBaseException().Message;
                return false;
            }
        }

        private static bool TryGet(string url, string accessToken, out string responseText, out string error)
        {
            responseText = null;
            error = null;
            try
            {
                using (var client = NewClient())
                {
                    client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                    using (HttpResponseMessage response = client.GetAsync(url).Result)
                    {
                        responseText = response.Content.ReadAsStringAsync().Result;
                        if (!response.IsSuccessStatusCode)
                        {
                            error = "Google のユーザー情報を取得できませんでした。";
                            return false;
                        }
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                error = "Google に接続できませんでした。" + ex.GetBaseException().Message;
                return false;
            }
        }

        private static HttpClient NewClient()
        {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            return new HttpClient { Timeout = TimeSpan.FromSeconds(15) };
        }

        private static string JsonString(string json, string key)
        {
            if (string.IsNullOrEmpty(json) || string.IsNullOrEmpty(key))
                return "";

            string needle = "\"" + key + "\"";
            int start = json.IndexOf(needle, StringComparison.Ordinal);
            if (start < 0)
                return "";

            int colon = json.IndexOf(':', start + needle.Length);
            if (colon < 0)
                return "";

            int i = colon + 1;
            while (i < json.Length && char.IsWhiteSpace(json[i]))
                i++;
            if (i >= json.Length || json[i] != '"')
                return "";

            i++;
            var sb = new StringBuilder();
            while (i < json.Length)
            {
                char c = json[i++];
                if (c == '\\' && i < json.Length)
                {
                    sb.Append(json[i++]);
                    continue;
                }
                if (c == '"')
                    break;
                sb.Append(c);
            }

            return HttpUtility.HtmlDecode(sb.ToString());
        }
    }
}
