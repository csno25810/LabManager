namespace LabManager
{
    partial class Form10
    {
        /// <summary>
        /// 必要なデザイナ変数。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        // UI要素の宣言
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;  // カレンダー用のグリッド
        private System.Windows.Forms.Label lblMonth;                      // 現在の月を表示するラベル
        private System.Windows.Forms.Button btnPrevMonth;                // 前月へ移動するボタン
        private System.Windows.Forms.Button btnNextMonth;                // 次月へ移動するボタン

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// デザイン初期化（UI構築）
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            // --- TableLayoutPanel の設定（カレンダー用） ---
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1.ColumnCount = 7; // 曜日（7列）
            this.tableLayoutPanel1.RowCount = 6;    // 最大6行（週）

            // 列スタイル（曜日：均等 14.28% × 7列）
            for (int i = 0; i < 7; i++)
            {
                this.tableLayoutPanel1.ColumnStyles.Add(
                    new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.28F));
            }

            // 行スタイル（6週対応 16.66% × 6行）
            for (int i = 0; i < 6; i++)
            {
                this.tableLayoutPanel1.RowStyles.Add(
                    new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66F));
            }

            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 50); // 画面上の位置
            this.tableLayoutPanel1.Size = new System.Drawing.Size(760, 500);   // 幅×高さ
            this.tableLayoutPanel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            // --- 月表示ラベル ---
            this.lblMonth = new System.Windows.Forms.Label();
            this.lblMonth.Font = new System.Drawing.Font("MS UI Gothic", 16F, System.Drawing.FontStyle.Bold);
            this.lblMonth.Location = new System.Drawing.Point(300, 10);
            this.lblMonth.Size = new System.Drawing.Size(200, 30);
            this.lblMonth.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblMonth.Text = "YYYY年MM月"; // 初期値は空でも可

            // --- 前月ボタン ---
            this.btnPrevMonth = new System.Windows.Forms.Button();
            this.btnPrevMonth.Location = new System.Drawing.Point(200, 10);
            this.btnPrevMonth.Size = new System.Drawing.Size(75, 30);
            this.btnPrevMonth.Text = "＜前月";
            this.btnPrevMonth.Click += new System.EventHandler(this.buttonPrevMonth_Click); // イベントに接続

            // --- 次月ボタン ---
            this.btnNextMonth = new System.Windows.Forms.Button();
            this.btnNextMonth.Location = new System.Drawing.Point(520, 10);
            this.btnNextMonth.Size = new System.Drawing.Size(75, 30);
            this.btnNextMonth.Text = "次月＞";
            this.btnNextMonth.Click += new System.EventHandler(this.buttonNextMonth_Click); // イベントに接続

            // --- Form 全体設定 ---
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.tableLayoutPanel1);   // カレンダーグリッド
            this.Controls.Add(this.lblMonth);            // 月ラベル
            this.Controls.Add(this.btnPrevMonth);        // 前月ボタン
            this.Controls.Add(this.btnNextMonth);        // 次月ボタン
            this.Name = "Form10";
            this.Text = "大学カレンダー";
            this.Load += new System.EventHandler(this.Form10_Load); // フォームロード時イベント
            this.ResumeLayout(false);
        }
    }
}
