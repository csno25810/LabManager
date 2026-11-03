using System;
using System.Drawing;
using System.Windows.Forms;

namespace LabManager
{
    /// <summary>
    /// 個人カルテ用ログイン。学籍番号と lab_user の既存パスワードだけを使う。
    /// </summary>
    public class Form18 : Form
    {
        private readonly Setting settings;
        private readonly TextBox textStudentId;
        private readonly TextBox textPassword;
        private readonly Label labelMessage;

        public LabSession Session { get; private set; }

        public Form18(Setting settings)
        {
            this.settings = settings ?? new Setting();
            Text = "個人カルテ ログイン";
            Font = UiFonts.Get(10F);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(460, 230);

            var title = new Label
            {
                AutoSize = false,
                Location = new Point(16, 12),
                Size = new Size(428, 24),
                Font = UiFonts.Get(12F, FontStyle.Bold),
                Text = "学籍番号でログイン"
            };
            var help = new Label
            {
                AutoSize = false,
                Location = new Point(16, 40),
                Size = new Size(428, 24),
                Text = "学籍番号とパスワードを入れてください。"
            };
            var labelId = new Label
            {
                AutoSize = true,
                Location = new Point(16, 76),
                Text = "学籍番号"
            };
            textStudentId = new TextBox
            {
                Location = new Point(110, 72),
                Size = new Size(220, 24),
                ImeMode = ImeMode.Disable,
                MaxLength = 20
            };
            var labelPassword = new Label
            {
                AutoSize = true,
                Location = new Point(16, 110),
                Text = "パスワード"
            };
            textPassword = new TextBox
            {
                Location = new Point(110, 106),
                Size = new Size(220, 24),
                UseSystemPasswordChar = true,
                ImeMode = ImeMode.Disable
            };
            labelMessage = new Label
            {
                AutoSize = false,
                Location = new Point(16, 140),
                Size = new Size(428, 40),
                ForeColor = Color.DarkRed,
                Text = ""
            };
            var buttonLogin = new Button
            {
                Text = "ログイン",
                Location = new Point(250, 186),
                Size = new Size(90, 30)
            };
            var buttonCancel = new Button
            {
                Text = "閉じる",
                Location = new Point(348, 186),
                Size = new Size(90, 30),
                DialogResult = DialogResult.Cancel
            };
            textStudentId.TextChanged += textStudentId_TextChanged;

            buttonLogin.Click += buttonLogin_Click;
            AcceptButton = buttonLogin;
            CancelButton = buttonCancel;
            Load += (sender, e) =>
            {
                textStudentId.ImeMode = ImeMode.Disable;
                textPassword.ImeMode = ImeMode.Disable;
            };

            Controls.Add(title);
            Controls.Add(help);
            Controls.Add(labelId);
            Controls.Add(textStudentId);
            Controls.Add(labelPassword);
            Controls.Add(textPassword);
            Controls.Add(labelMessage);
            Controls.Add(buttonLogin);
            Controls.Add(buttonCancel);
        }

        private void textStudentId_TextChanged(object sender, EventArgs e)
        {
            string halfWidth = ToHalfWidthAscii(textStudentId.Text);
            if (halfWidth == textStudentId.Text)
                return;

            int caret = textStudentId.SelectionStart - (textStudentId.Text.Length - halfWidth.Length);
            textStudentId.Text = halfWidth;
            textStudentId.SelectionStart = Math.Max(0, caret);
        }

        /// <summary>全角英数字を半角にし、半角以外は除く。パスワード欄と同じく IME は使わない。</summary>
        private static string ToHalfWidthAscii(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            var chars = new char[value.Length];
            int n = 0;
            foreach (char c in value)
            {
                char converted = c;
                if (c >= '！' && c <= '～')
                    converted = (char)(c - 0xFEE0);
                else if (c == '　')
                    converted = ' ';

                if (converted <= 0x7F)
                    chars[n++] = converted;
            }

            return new string(chars, 0, n);
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            labelMessage.Text = "";
            if (!EnsureConnected())
            {
                labelMessage.Text = "データベースに接続できません。C:\\MyReader\\SQLReader.ini を確認してください。";
                return;
            }

            LabSession session;
            string error;
            if (!LabUserStore.TryLogin(textStudentId.Text, textPassword.Text, out session, out error))
            {
                labelMessage.Text = error ?? "学籍番号またはパスワードが違います。";
                textPassword.Clear();
                return;
            }

            Session = session;
            DialogResult = DialogResult.OK;
            Close();
        }

        private bool EnsureConnected()
        {
            if (Connector.IsConnected)
                return true;

            return Connector.Connect(
                settings.UserID,
                settings.PassWd,
                settings.DataBaseName,
                settings.ServerIP);
        }
    }
}
