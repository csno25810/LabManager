using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using CsvHelper;
using CsvHelper.Configuration;
using MySql.Data.MySqlClient;

namespace MySQL_ConnectionTest
{
    public partial class Form8 : Form
    {
        // MySQL接続設定（研究室：server=localhost;Database=felica;Uid=root;Pwd=; もここに入る想定）
        private Setting mySqlSet;

        // DataGridView に表示するデータテーブル
        private DataTable referenceTable = new DataTable();

        // ユーザーが選択した順番を記録する（クリック順）
        private List<int> selectedOrder = new List<int>();

        // CSV
        private readonly string googleFormCsvUrl =
      "https://docs.google.com/spreadsheets/d/e/2PACX-1vTNSU7NP_vyrfFULzCBCHuppfmM4YdFeDjlwf5HyA-0ZZOq0igozt3dQJXwwB9XuR-rlXwQ5HG7SZfc/pub?output=csv";

        public Form8(Setting settings)
        {
            InitializeComponent();
            this.mySqlSet = settings;

            // Genre候補
            if (comboBoxGenre.Items.Count == 0)
            {
                comboBoxGenre.Items.AddRange(new object[] { "画像", "経路", "WEB" });
            }

            LoadReferences();
        }

        // CSV 1レコード
        public class FormData
        {
            public string title { get; set; }        // タイトル
            public string author { get; set; }       // 著者名
            public string thesis_type { get; set; }  // 文献タイプ
            public string year { get; set; }         // 発行年
            public string pages { get; set; }        // ページ数
            public string site_name { get; set; }    // サイト名
            public string url { get; set; }          // URL
            public string open_date { get; set; }    // 参照日（DBでは open_date）
            public string genre { get; set; }        // ジャンル
            public string notes { get; set; }        // 説明
        }

        // CSV と FormData の対応
        public sealed class FormDataMap : ClassMap<FormData>
        {
            public FormDataMap()
            {
                // 先頭2列は無視（Mapしない）

                Map(m => m.title).Name("タイトル");
                Map(m => m.author).Name("著者名");
                Map(m => m.thesis_type).Name("文献タイプ");
                Map(m => m.year).Name("発行年");
                Map(m => m.pages).Name("ページ数");
                Map(m => m.site_name).Name("サイト名");
                Map(m => m.url).Name("URL");
                Map(m => m.open_date).Name("参照日");
                Map(m => m.genre).Name("ジャンル");
                Map(m => m.notes).Name("説明");
            }
        }

        // GoogleフォームCSV取り込み（更新ボタン）
        private void buttonImportFromForm_Click(object sender, EventArgs e)
        {
            try
            {
                List<FormData> records;

                using (var client = new WebClient())
                using (var stream = client.OpenRead(googleFormCsvUrl))
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HeaderValidated = null,
                    MissingFieldFound = null,
                    IgnoreBlankLines = true,
                    BadDataFound = null
                }))
                {
                    // ヘッダー名で正しくマップする（列ズレ防止）
                    csv.Context.RegisterClassMap<FormDataMap>();
                    records = csv.GetRecords<FormData>().ToList();
                }

                using (var connection = new MySqlConnection(mySqlSet.ConnectionString))
                {
                    connection.Open();

                    foreach (var r in records)
                    {
                        // 空行っぽいものを除外
                        string title = (r.title ?? "").Trim();
                        string author = (r.author ?? "").Trim();
                        string year = (r.year ?? "").Trim();
                        string thesisType = (r.thesis_type ?? "").Trim();
                        string pages = (r.pages ?? "").Trim();
                        string siteName = (r.site_name ?? "").Trim();
                        string url = (r.url ?? "").Trim();
                        string openDate = (r.open_date ?? "").Trim(); // 参照日
                        string genre = (r.genre ?? "").Trim();
                        string notes = (r.notes ?? "").Trim();

                        // タイトルが空だがURLがあるWEBケース：タイトルにURLを仮セット
                        if (string.IsNullOrWhiteSpace(title) && !string.IsNullOrWhiteSpace(url))
                            title = url;

                        // それでも何もない行は無視
                        if (string.IsNullOrWhiteSpace(title) && string.IsNullOrWhiteSpace(url))
                            continue;

                        // WEB判定：URLが入っていたらWEB扱い
                        bool isWeb = !string.IsNullOrWhiteSpace(url);

                        if (!isWeb)
                        {
                            // 論文：title+author+year が完全一致なら重複→登録しない
                            using (var check = new MySqlCommand(@"
                                SELECT COUNT(*) 
                                FROM references_list 
                                WHERE title=@title AND author=@author AND year=@year
                            ", connection))
                            {
                                check.Parameters.AddWithValue("@title", title);
                                check.Parameters.AddWithValue("@author", author);
                                check.Parameters.AddWithValue("@year", year);

                                int count = Convert.ToInt32(check.ExecuteScalar());
                                if (count > 0) continue;
                            }

                            // INSERT（論文）
                            using (var ins = new MySqlCommand(@"
                                INSERT INTO references_list
                                (title, author, thesis_type, year, notes, url, site_name, reference_type, pages, genre, open_date)
                                VALUES
                                (@title, @author, @thesis_type, @year, @notes, @url, @site_name, @reference_type, @pages, @genre, @open_date)
                            ", connection))
                            {
                                ins.Parameters.AddWithValue("@title", title);
                                ins.Parameters.AddWithValue("@author", author);
                                ins.Parameters.AddWithValue("@thesis_type", thesisType);
                                ins.Parameters.AddWithValue("@year", year);
                                ins.Parameters.AddWithValue("@notes", notes);
                                ins.Parameters.AddWithValue("@url", url);
                                ins.Parameters.AddWithValue("@site_name", siteName);
                                ins.Parameters.AddWithValue("@reference_type", ""); // 使っていないなら空でOK
                                ins.Parameters.AddWithValue("@pages", pages);
                                ins.Parameters.AddWithValue("@genre", genre);
                                ins.Parameters.AddWithValue("@open_date", openDate); // 論文でも入ってたら保存はする

                                ins.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            // WEB：URL（優先）または title が一致する既存があるか？
                            // 参照日(open_date)が違えば、既存の open_date を更新（上書き）

                            int existingId = -1;
                            string existingOpenDate = "";

                            // まずURLで検索（URLが最優先）
                            using (var find = new MySqlCommand(
                                "SELECT id, open_date FROM references_list WHERE url = @url AND url <> '' LIMIT 1",
                                connection))
                            {
                                find.Parameters.AddWithValue("@url", url);
                                using (var rd = find.ExecuteReader())
                                {
                                    if (rd.Read())
                                    {
                                        existingId = Convert.ToInt32(rd["id"]);
                                        existingOpenDate = (rd["open_date"] ?? "").ToString();
                                    }
                                }
                            }

                            // URLで見つからない場合、titleで検索
                            if (existingId < 0 && !string.IsNullOrWhiteSpace(title))
                            {
                                using (var find2 = new MySqlCommand(
                                    "SELECT id, open_date FROM references_list WHERE title=@title AND title<>'' LIMIT 1", 
                                    connection))
                                {
                                    find2.Parameters.AddWithValue("@title", title);
                                    using (var rd = find2.ExecuteReader())
                                    {
                                        if (rd.Read())
                                        {
                                            existingId = Convert.ToInt32(rd["id"]);
                                            existingOpenDate = (rd["open_date"] ?? "").ToString();
                                        }
                                    }
                                }
                            }

                            if (existingId >= 0)
                            {
                                // 参照日が違うなら更新
                                if (!string.IsNullOrWhiteSpace(openDate) && existingOpenDate != openDate)
                                {
                                    using (var upd = new MySqlCommand(@"
                                        UPDATE references_list SET
                                            open_date=@open_date,
                                            site_name=@site_name,
                                            notes=@notes,
                                            genre=@genre,
                                            year=@year,
                                            thesis_type=@thesis_type,
                                            pages=@pages,
                                            title=@title
                                        WHERE id=@id
                                    ", connection))
                                    {
                                        upd.Parameters.AddWithValue("@open_date", openDate);
                                        upd.Parameters.AddWithValue("@site_name", siteName);
                                        upd.Parameters.AddWithValue("@notes", notes);
                                        upd.Parameters.AddWithValue("@genre", genre);
                                        upd.Parameters.AddWithValue("@year", year);
                                        upd.Parameters.AddWithValue("@thesis_type", thesisType);
                                        upd.Parameters.AddWithValue("@pages", pages);
                                        upd.Parameters.AddWithValue("@title", title);
                                        upd.Parameters.AddWithValue("@id", existingId);

                                        upd.ExecuteNonQuery();
                                    }
                                }
                                // 参照日が同じなら何もしない（重複扱い）
                            }
                            else
                            {
                                // 既存がない → INSERT（WEB）
                                using (var ins = new MySqlCommand(@"
                                INSERT INTO references_list
                                (title, author, thesis_type, year, notes, url, site_name, reference_type, pages, genre, open_date)
                                VALUES
                                (@title, @author, @thesis_type, @year, @notes, @url, @site_name, @reference_type, @pages, @genre, @open_date)
                                ", connection))
                                {
                                    ins.Parameters.AddWithValue("@title", title);
                                    ins.Parameters.AddWithValue("@author", author); // WEBでも空でOK
                                    ins.Parameters.AddWithValue("@thesis_type", thesisType);
                                    ins.Parameters.AddWithValue("@year", year);
                                    ins.Parameters.AddWithValue("@notes", notes);
                                    ins.Parameters.AddWithValue("@url", url);
                                    ins.Parameters.AddWithValue("@site_name", siteName);
                                    ins.Parameters.AddWithValue("@reference_type", "");
                                    ins.Parameters.AddWithValue("@pages", pages);
                                    ins.Parameters.AddWithValue("@genre", genre);
                                    ins.Parameters.AddWithValue("@open_date", openDate);

                                    ins.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }

                LoadReferences();
                MessageBox.Show("Googleフォームのデータを取り込みました。");
            }
            catch (Exception ex)
            {
                MessageBox.Show("エラー：" + ex.Message);
            }
        }

        // 文献一覧読み込み
        private void LoadReferences()
        {
            selectedOrder.Clear();

            string query = @"
        SELECT
            id, title, author, year, thesis_type,
            site_name, url, open_date, notes, pages, genre
        FROM references_list
    ";

            referenceTable.Clear();

            using (var conn = new MySqlConnection(mySqlSet.ConnectionString))
            {
                conn.Open();
                using (var da = new MySqlDataAdapter(query, conn))
                {
                    da.Fill(referenceTable);
                }
            }

            dataGridView1.DataSource = referenceTable;
            ApplyRowColors();
        }


        // Row色（ジャンル）
        private void ApplyRowColors()
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue;

                string genre = row.Cells["genre"].Value?.ToString() ?? "";

                if (genre.Contains("画像"))
                    row.DefaultCellStyle.BackColor = Color.LightCoral;
                else if (genre.Contains("経路"))
                    row.DefaultCellStyle.BackColor = Color.LightYellow;
                else if (genre.Contains("WEB"))
                    row.DefaultCellStyle.BackColor = Color.LightGreen;
                else
                    row.DefaultCellStyle.BackColor = Color.White;
            }
        }

        // 互換用
        private void dataGridView1_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (e.RowIndex < 0) return;
        }

        // 追加/削除/編集
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            string title = textBoxTitle.Text.Trim();
            string author = textBoxAuthor.Text.Trim();
            string year = textBoxYear.Text.Trim();
            string thesisType = textBoxThesisType.Text.Trim();
            string siteName = textBoxSiteName.Text.Trim();
            string url = textBoxUrl.Text.Trim();
            string notes = textBoxNotes.Text.Trim();
            string pages = textBoxPages.Text.Trim();
            string genre = comboBoxGenre.Text.Trim();

            if (string.IsNullOrWhiteSpace(title))
            {
                MessageBox.Show("タイトルは必須です");
                return;
            }

            using (var conn = new MySqlConnection(mySqlSet.ConnectionString))
            {
                conn.Open();
                using (var cmd = new MySqlCommand(@"
                    INSERT INTO references_list
                    (title, author, thesis_type, year, notes, url, site_name, reference_type, pages, genre, open_date)
                    VALUES
                    (@title, @author, @thesis_type, @year, @notes, @url, @site_name, @reference_type, @pages, @genre, @open_date)
                ", conn))
                {
                    cmd.Parameters.AddWithValue("@title", title);
                    cmd.Parameters.AddWithValue("@author", author);
                    cmd.Parameters.AddWithValue("@thesis_type", thesisType);
                    cmd.Parameters.AddWithValue("@year", year);
                    cmd.Parameters.AddWithValue("@notes", notes);
                    cmd.Parameters.AddWithValue("@url", url);
                    cmd.Parameters.AddWithValue("@site_name", siteName);
                    cmd.Parameters.AddWithValue("@reference_type", "");
                    cmd.Parameters.AddWithValue("@pages", pages);
                    cmd.Parameters.AddWithValue("@genre", genre);
                    cmd.Parameters.AddWithValue("@open_date", ""); // 手入力UIに参照日が無いので空

                    cmd.ExecuteNonQuery();
                }
            }

            LoadReferences();
            ClearFields();
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0) return;

            int id = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["id"].Value);

            using (var conn = new MySqlConnection(mySqlSet.ConnectionString))
            {
                conn.Open();
                using (var cmd = new MySqlCommand("DELETE FROM references_list WHERE id=@id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }

            LoadReferences();
            ClearFields();
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0) return;

            int id = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["id"].Value);

            string title = textBoxTitle.Text.Trim();
            string author = textBoxAuthor.Text.Trim();
            string year = textBoxYear.Text.Trim();
            string thesisType = textBoxThesisType.Text.Trim();
            string siteName = textBoxSiteName.Text.Trim();
            string url = textBoxUrl.Text.Trim();
            string notes = textBoxNotes.Text.Trim();
            string pages = textBoxPages.Text.Trim();
            string genre = comboBoxGenre.Text.Trim();

            using (var conn = new MySqlConnection(mySqlSet.ConnectionString))
            {
                conn.Open();
                using (var cmd = new MySqlCommand(@"
                    UPDATE references_list SET
                        title=@title,
                        author=@author,
                        year=@year,
                        thesis_type=@thesis_type,
                        site_name=@site_name,
                        url=@url,
                        notes=@notes,
                        pages=@pages,
                        genre=@genre
                    WHERE id=@id
                ", conn))
                {
                    cmd.Parameters.AddWithValue("@title", title);
                    cmd.Parameters.AddWithValue("@author", author);
                    cmd.Parameters.AddWithValue("@year", year);
                    cmd.Parameters.AddWithValue("@thesis_type", thesisType);
                    cmd.Parameters.AddWithValue("@site_name", siteName);
                    cmd.Parameters.AddWithValue("@url", url);
                    cmd.Parameters.AddWithValue("@notes", notes);
                    cmd.Parameters.AddWithValue("@pages", pages);
                    cmd.Parameters.AddWithValue("@genre", genre);
                    cmd.Parameters.AddWithValue("@id", id);

                    cmd.ExecuteNonQuery();
                }
            }

            LoadReferences();
            ClearFields();
        }

        // クリック順を記録（複数選択の順番制御）
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = dataGridView1.Rows[e.RowIndex];
            int id = Convert.ToInt32(row.Cells["id"].Value);

            // Ctrl / Shift を押していない → 単一選択扱い
            if ((ModifierKeys & (Keys.Control | Keys.Shift)) == Keys.None)
            {
                selectedOrder.Clear();
            }

            // 同じIDがすでにあれば一度消して末尾に（順番更新）
            selectedOrder.Remove(id);
            selectedOrder.Add(id);

            // 入力欄へ反映
            textBoxTitle.Text = row.Cells["title"].Value?.ToString();
            textBoxAuthor.Text = row.Cells["author"].Value?.ToString();
            textBoxYear.Text = row.Cells["year"].Value?.ToString();
            textBoxThesisType.Text = row.Cells["thesis_type"].Value?.ToString();
            textBoxSiteName.Text = row.Cells["site_name"].Value?.ToString();
            textBoxUrl.Text = row.Cells["url"].Value?.ToString();
            textBoxNotes.Text = row.Cells["notes"].Value?.ToString();
            textBoxPages.Text = row.Cells["pages"].Value?.ToString();
            comboBoxGenre.Text = row.Cells["genre"].Value?.ToString();

            // サイト or 論文 判定
            string siteName = row.Cells["site_name"].Value?.ToString() ?? "";
            string url = row.Cells["url"].Value?.ToString() ?? "";

            if (!string.IsNullOrWhiteSpace(siteName) || !string.IsNullOrWhiteSpace(url))
                radioButtonSite.Checked = true;
            else
                radioButtonThesis.Checked = true;
        }



        // 引用コピー（複数・クリック順）
        private void buttonCopyCitation_Click(object sender, EventArgs e)
        {
            if (selectedOrder.Count == 0)
            {
                MessageBox.Show("文献を選択してください。");
                return;
            }

            StringBuilder sb = new StringBuilder();
            int index = 1;

            foreach (int id in selectedOrder)
            {
                DataRow[] rows = referenceTable.Select($"id = {id}");
                if (rows.Length == 0) continue;

                DataRow r = rows[0];

                string title = r["title"].ToString();
                string author = r["author"].ToString();
                string year = r["year"].ToString();
                string thesisType = r["thesis_type"].ToString();
                string siteName = r["site_name"].ToString();
                string url = r["url"].ToString();

                string citation;

                // WEB
                if (!string.IsNullOrWhiteSpace(siteName) || !string.IsNullOrWhiteSpace(url))
                {
                    citation = $"{siteName} ：「{title}」,{url} ({year})";
                }
                // 論文
                else
                {
                    citation = $"{author}：「{title}」，{thesisType}（{year}）";
                }

                sb.AppendLine($"[{index}] {citation}");
                index++;
            }

            Clipboard.SetText(sb.ToString());

            // コピー内容を表示
            MessageBox.Show(
        "以下の内容をコピーしました：\n\n" + sb.ToString(),
        "引用コピー完了",
        MessageBoxButtons.OK,
        MessageBoxIcon.Information
      );
        }


        // ソート
        private void buttonSortByGenre_Click(object sender, EventArgs e)
        {
            DataView view = referenceTable.DefaultView;
            view.Sort = "genre ASC";
            dataGridView1.DataSource = view.ToTable();
            ApplyRowColors();
        }

        private void buttonSortById_Click(object sender, EventArgs e)
        {
            DataView view = referenceTable.DefaultView;
            view.Sort = "id ASC";
            dataGridView1.DataSource = view.ToTable();
            ApplyRowColors();
        }

        private void ClearFields()
        {
            textBoxTitle.Clear();
            textBoxAuthor.Clear();
            textBoxYear.Clear();
            textBoxThesisType.Clear();
            textBoxSiteName.Clear();
            textBoxUrl.Clear();
            textBoxNotes.Clear();
            textBoxPages.Clear();
            comboBoxGenre.SelectedIndex = -1;
            radioButtonThesis.Checked = true;
        }
    }
}