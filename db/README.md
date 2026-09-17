# db/

LabManager の開発用 MySQL スキーマ定義とサンプルデータ。

## 内容

| ファイル | 内容 |
|----------|------|
| `schema.sql` | データベース `felica` と全テーブル、およびアプリ用ユーザー `labapp` を作成 |
| `seed.sql` | 学生・先生マスタと chip_list のみ投入（**カレンダー・日直データは削除しない**） |
| `seed_demo_reset.sql` | デモ用に touch_log / duty / カレンダーを**まとめて消す**（通常のアップデートでは使わない） |
| `migrate_002_add_diary_log.sql` | 既存DBに `diary_log` テーブルを追加（Form6 用） |
| `migrate_003_add_references_list.sql` | 既存DBに `references_list` テーブルを追加（Form8 用） |
| `migrate_004_add_personal_info_mail.sql` | 既存DBの `personal_info` に `mail` 列を追加 |
| `migrate_005_add_chip_list_system_id.sql` | 既存DBの `chip_list` に `system_id` 列を追加 |
| `migrate_006_add_touch_log_terminal_id.sql` | 既存DBの `touch_log` に `terminal_id` 列を追加 |
| `migrate_007_add_duty_edit_log.sql` | 出席状況編集ログ `duty_edit_log` テーブルを追加 |
| `migrate_008_add_lab_calendar_day.sql` | 授業日・予定メモ `lab_calendar_day` テーブルを追加 |
| `migrate_009_add_duty_weekday_roster.sql` | 曜日別日直担当 `duty_weekday_roster` テーブルを追加 |
| `fix_personal_info_names.sql` | 氏名が `????` 化した personal_info を修復 |

## 適用方法（ローカルMySQLに対して）

PowerShell で以下を順に実行する。`-p` オプションでパスワードが対話入力で求められる。

```powershell
mysql -u root -p --default-character-set=utf8mb4 < db\schema.sql
mysql -u root -p --default-character-set=utf8mb4 < db\seed.sql
```

氏名が `????` になる場合（Windows で seed 投入時に文字化けしたとき）:

```powershell
mysql -u root -p --default-character-set=utf8mb4 felica < db\fix_personal_info_names.sql
```

既存DBを更新する場合（テーブル追加のみ）:

```powershell
mysql -u root -p --default-character-set=utf8mb4 felica < db\migrate_002_add_diary_log.sql
mysql -u root -p --default-character-set=utf8mb4 felica < db\migrate_003_add_references_list.sql
mysql -u root -p --default-character-set=utf8mb4 felica < db\migrate_004_add_personal_info_mail.sql
mysql -u root -p --default-character-set=utf8mb4 felica < db\migrate_005_add_chip_list_system_id.sql
mysql -u root -p --default-character-set=utf8mb4 felica < db\migrate_006_add_touch_log_terminal_id.sql
mysql -u root -p --default-character-set=utf8mb4 felica < db\migrate_007_add_duty_edit_log.sql
mysql -u root -p --default-character-set=utf8mb4 felica < db\migrate_008_add_lab_calendar_day.sql
mysql -u root -p --default-character-set=utf8mb4 felica < db\migrate_009_add_duty_weekday_roster.sql
mysql -u root -p --default-character-set=utf8mb4 felica < db\seed.sql
```

**注意:** `seed.sql` は `lab_calendar_day` / `duty_weekday_roster` / `duty_schedule` を削除しません。カレンダー・日直を初期化したいときだけ `seed_demo_reset.sql` を使ってください。

日本語パスで失敗する場合は、一時フォルダ経由で投入する:

```powershell
Copy-Item db\seed.sql $env:TEMP\lab_seed.sql
Get-Content $env:TEMP\lab_seed.sql -Raw -Encoding UTF8 | mysql --default-character-set=utf8mb4 -u labapp -plabapp_pw felica
```

## アプリ用ユーザー

`schema.sql` 内で以下のユーザーを作成している：

- ユーザー名: `labapp`
- パスワード: `labapp_pw`
- 認証方式: `mysql_native_password`（MySql.Data 5.0.9 との互換のため）
- 権限: `felica.*` に対する SELECT / INSERT / UPDATE / DELETE

ローカル開発専用なので、研究室DBには絶対に同じパスワードを使わないこと。

## データベース `felica` のデータ一覧

### LabManager が使うテーブル

#### personal_info（学生マスタ）

| 列 | 型 | 意味 |
|----|-----|------|
| student_id | VARCHAR | 学籍番号（主キー） |
| name | VARCHAR | 氏名 |
| mail | VARCHAR | メールアドレス |
| penalty_count | INT | 罰直累計回数（18:00 バッチで更新。メイン画面・日直管理で表示） |

#### chip_list（ICカード ↔ 学生）

| 列 | 型 | 意味 |
|----|-----|------|
| chip_id | VARCHAR | FeliCa 等の chip ID（主キー） |
| student_id | VARCHAR | 学籍番号（personal_info へ外部キー） |
| system_id | VARCHAR | システム識別子 |

#### touch_log（タッチ履歴）

| 列 | 型 | 意味 |
|----|-----|------|
| id | INT | 連番（主キー・自動採番） |
| time_stamp | DATETIME | タッチ日時 |
| terminal_id | VARCHAR | ターミナル（リーダー）ID |
| chip_id | VARCHAR | タッチされた chip ID |

**メイン画面での派生データ（DB 列ではない）**

| 表示 | 算出方法 |
|------|----------|
| 在室 / 不在 | 当日のタッチ回数が奇数 → 在室、偶数 → 不在 |
| 在室人数 | 在室判定の人数（学生 + 先生） |
| 本日来室人数 | 当日に1回以上タッチした人数 |
| 初回タッチ時刻 | 当日 MIN(time_stamp) |
| 最終タッチ時刻 | 当日 MAX(time_stamp) |
| 学生一覧 | `personal_info` 全員（学籍番号順）。タッチ未登録も表示 |
| 先生行 | `student_id = '@'`（内部ID。画面では学籍番号非表示）。日直候補から除外 |

#### duty_schedule（日直スケジュール・出席状況）

| 列 | 型 | 意味 |
|----|-----|------|
| duty_date | DATE | 日直の日付（複合主キー） |
| student_id | VARCHAR | 担当学籍番号（複合主キー） |
| duty_status | INT | **出席状況**（下表参照） |
| duty_type | VARCHAR | **日直種別**（下表参照） |
| penalty_count | INT | 当該日の罰直カウント（旧コード互換。累計は personal_info.penalty_count） |

**duty_status（出席状況）**

| 値 | 意味 | 更新タイミング |
|----|------|----------------|
| 0 | 未出席 | 初期値。8:50 前は「待機中」、8:50 過ぎ未タッチは「未タッチ」と表示 |
| 1 | 出席 | 8:50 基準で10分以内にタッチがあった場合（自動更新） |
| 2 | 遅刻 | 8:50 基準で10分超のタッチ（自動更新） |

**duty_type（日直種別）**

| 値 | 意味 |
|----|------|
| 0 | 通常日直 |
| 1 | 罰直 |

**メイン画面の日直欄で表示する列**

- 学籍番号、日直氏名、出席時刻（touch_log から MIN）、出席状況（duty_status）、罰直回数（personal_info.penalty_count）

#### lab_calendar_day（授業日・予定メモ）

| 列 | 型 | 意味 |
|----|-----|------|
| calendar_date | DATE | 日付（主キー） |
| is_class_day | TINYINT | 授業日フラグ（1=授業日） |
| memo | VARCHAR(7) | 予定メモ（最大7文字） |

授業回数は曜日ごとに独立カウント（①～⑬）。編集: Form10（CalendarEditor）、表示: Form14（TvCalendarPanel）。

#### duty_weekday_roster（曜日別日直担当）

| 列 | 型 | 意味 |
|----|-----|------|
| weekday | TINYINT | 曜日（1=月 … 5=金） |
| slot | TINYINT | 枠（0/1 = 1人目/2人目） |
| student_id | VARCHAR | 担当学籍番号 |

Form5 で設定。授業日追加時に `duty_schedule` へ自動登録（土曜は日直なし）。

#### duty_edit_log（出席状況編集ログ）

出席状況編集フォームでの追加・更新・削除を記録する（migrate_007）。

| 列 | 型 | 意味 |
|----|-----|------|
| id | INT | 連番 |
| edited_at | DATETIME | 編集日時 |
| action | VARCHAR | INSERT / UPDATE / DELETE |
| duty_date | DATE | 対象日 |
| student_id | VARCHAR | 対象学籍番号 |
| old_duty_status | INT | 変更前の出席状況（NULL 可） |
| new_duty_status | INT | 変更後の出席状況（NULL 可） |
| old_duty_type | VARCHAR | 変更前の種類 |
| new_duty_type | VARCHAR | 変更後の種類 |

#### diary_log（日誌）— 先輩機能

| 列 | 型 | 意味 |
|----|-----|------|
| student_id | VARCHAR | 提出者学籍番号 |
| dialy_date | DATE | 日誌の日付（旧DBの typo を踏襲） |
| title | VARCHAR | タイトル |
| content | TEXT | 本文 |

#### references_list（文献）— 先輩機能

| 列 | 型 | 意味 |
|----|-----|------|
| id | INT | 文献 ID |
| title, author, year, notes, url, … | 各種 | 文献メタデータ（Form8 で CRUD） |

---

### 研究室DBに存在・LabManager 未使用

| テーブル | 推定用途 |
|----------|----------|
| class_info | 授業情報 |
| semester | 学期 |
| timetable | 時間割 |
| terminal_info | ターミナルマスタ（touch_log.terminal_id と対応想定） |
| touch_log2 | 旧ログ・バックアップ |
| personal_info?back | バックアップ表 |

---

### テーブル間の関係（コア）

```
personal_info ← chip_list ← touch_log
      ↑
duty_schedule ← duty_weekday_roster（曜日マスタ）
      ↑
duty_edit_log（編集履歴のみ参照）

lab_calendar_day（授業日・予定・授業回数算出）
```

タッチ → chip_list で student_id を特定 → touch_log に記録 → メイン画面で在席判定・日直の出席時刻表示。日直担当は duty_schedule に登録され、duty_status が自動または出席状況編集で更新される。授業日でない日は日直を表示しない。
