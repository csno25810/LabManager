using System;
using System.Drawing;
using System.Windows.Forms;

namespace LabManager
{
    /// <summary>
    /// LabManager 起動時のテレビ画面。左に Form14、右に Form1 を同じウィンドウで表示する。
    /// </summary>
    public class Form16 : Form
    {
        private readonly Setting settings;
        private Form14 leftPanel;
        private Form1 rightPanel;

        public Form16() : this(new Setting())
        {
        }

        public Form16(Setting settings)
        {
            this.settings = settings ?? new Setting();
            Text = "LabManager";
            BackColor = Color.White;
            StartPosition = FormStartPosition.Manual;
            FormBorderStyle = FormBorderStyle.None;
            int width = Screen.PrimaryScreen.Bounds.Width;
            int height = Screen.PrimaryScreen.Bounds.Height - 50;
            Location = new Point(0, 0);
            Size = new Size(width, height);

            var leftHost = new Panel
            {
                Dock = DockStyle.Left,
                Width = width / 2,
                BackColor = Color.White
            };
            var rightHost = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };

            Controls.Add(rightHost);
            Controls.Add(leftHost);

            Load += (sender, e) =>
            {
                rightPanel = new Form1(true);
                rightPanel.TopLevel = false;
                rightPanel.FormBorderStyle = FormBorderStyle.None;
                rightHost.Controls.Add(rightPanel);
                rightPanel.Show();

                leftPanel = new Form14(this.settings);
                leftPanel.TopLevel = false;
                leftPanel.FormBorderStyle = FormBorderStyle.None;
                leftHost.Controls.Add(leftPanel);
                leftPanel.ConfigureAsEmbedded();
                leftPanel.Show();
            };

            FormClosed += (sender, e) =>
            {
                if (leftPanel != null && !leftPanel.IsDisposed)
                    leftPanel.Close();
                if (rightPanel != null && !rightPanel.IsDisposed)
                    rightPanel.Close();
            };
        }
    }
}
