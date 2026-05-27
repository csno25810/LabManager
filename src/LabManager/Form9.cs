using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MySQL_ConnectionTest
{
    public partial class Form9 : Form
    {
        private Setting mySqlSet;  // MySQL 接続情報
        private DataTable dutyTable = new DataTable();  // DataGridView に表示するデータ用

        public Form9(Setting settings)
        {
            InitializeComponent();
            this.mySqlSet = settings;
            LoadStudents();     // コンボボックスに学生一覧をロード
            LoadDutySchedule(); // duty_schedule テーブルからデータを取得
        }

        /// <summary>
        /// コンボボックスに学生名をロード
        /// personal_info テーブルの name を表示、student_id を内部的に保持する
        /// </summary>
        private void LoadStudents()
        {
            string query = "SELECT student_id, name FROM personal_info ORDER BY name";
            DataTable students = new DataTable();
            Connector.TableReader(query, students);

            comboBoxStudent.DataSource = students;
            comboBoxStudent.DisplayMember = "name";       // ユーザーに見せる名前
            comboBoxStudent.ValueMember = "student_id";   // 内部で保持するID
        }

        /// <summary>
        /// duty_schedule の内容を DataGridView に表示
        /// </summary>
        private void LoadDutySchedule()
        {
            string query = "SELECT duty_date, student_id, duty_status, penalty_count, duty_type FROM duty_schedule ORDER BY duty_date";
            dutyTable.Clear();
            Connector.TableReader(query, dutyTable);
            dataGridViewDuty.DataSource = dutyTable;

            // ヘッダーテキストを日本語に
            dataGridViewDuty.Columns["duty_date"].HeaderText = "日付";
            dataGridViewDuty.Columns["student_id"].HeaderText = "学籍番号";
            dataGridViewDuty.Columns["duty_status"].HeaderText = "ステータス";
            dataGridViewDuty.Columns["penalty_count"].HeaderText = "罰直回数";
            dataGridViewDuty.Columns["duty_type"].HeaderText = "種類";
        }

        /// <summary>
        /// 「追加」ボタンが押されたとき
        /// 選択した学生と日付を duty_schedule に追加
        /// </summary>
        private void buttonAddDuty_Click(object sender, EventArgs e)
        {
            string date = dateTimePickerDuty.Value.ToString("yyyy-MM-dd");
            string studentId = comboBoxStudent.SelectedValue.ToString();

            string insertQuery = $@"
                INSERT INTO duty_schedule (duty_date, student_id, duty_status, penalty_count, duty_type)
                VALUES ('{date}', '{studentId}', 0, 0, 0)";

            Connector.ExecuteCommand(insertQuery);
            MessageBox.Show("日直を追加しました");

            LoadDutySchedule();  // 更新後に再読み込み
        }

        /// <summary>
        /// 「削除」ボタンが押されたとき
        /// 選択した duty_schedule のレコードを削除
        /// </summary>
        private void buttonDeleteDuty_Click(object sender, EventArgs e)
        {
            if (dataGridViewDuty.SelectedRows.Count == 0)
            {
                MessageBox.Show("削除する日直を選択してください");
                return;
            }

            string date = dataGridViewDuty.SelectedRows[0].Cells["duty_date"].Value.ToString();
            string studentId = dataGridViewDuty.SelectedRows[0].Cells["student_id"].Value.ToString();

            string deleteQuery = $@"
                DELETE FROM duty_schedule
                WHERE duty_date = '{date}' AND student_id = '{studentId}'";

            Connector.ExecuteCommand(deleteQuery);
            MessageBox.Show("日直を削除しました");

            LoadDutySchedule();  // 更新後に再読み込み
        }

        /// <summary>
        /// 「閉じる」ボタンが押されたとき
        /// </summary>
        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}