========================================
  LabManager 研究室テレビPC デプロイ手順
========================================

【目的】
  旧 MySQL_ConnectionTest.exe + GoogleCalenderReader.exe を
  LabManager 1 本に置き換え、Screen.bat から起動する。
  左半分のカレンダーは Google 連携ではなく DB（lab_calendar_day）を表示する。

【対象環境（研究室実機）】
  OS       : Windows 10/11（テレビPC）
  MySQL    : 5.1.x（例: 5.1.41 / XAMPP）
  DB       : felica
  DB ユーザー: SQLReader.ini の内容（例: Neko）
  .NET     : .NET Framework 4.8（未インストールなら先に入れる）

【旧システムとの対応】
  旧 Screen.bat
    start MySQL_ConnectionTest.exe   → LabManager.exe（画面右半分・在席・日直）
    start GoogleCalenderReader.exe   → 廃止（LabManager 起動時に Form14 が左半分を表示）
  設定ファイルの場所は変更なし: C:\MyReader\SQLReader.ini
  GoogleCalenderReader.ini は不要（残っていても無視される）。

========================================
  1. 開発PCでビルド
========================================

  Visual Studio で LabManager.sln を開く。
  構成: Release | x86
  ビルド → ソリューションのビルド

  出力先: bin\Release\

  ※ x86（32bit）でビルドすること。旧環境との互換のため。

========================================
  2. テレビPCへコピーするファイル
========================================

  旧「表示ツール」フォルダ（例: デスクトップ\表示ツール\）に
  以下をまとめてコピーする。

  [必須]
    Screen.bat              … この deploy フォルダのもの
    LabManager.exe
    LabManager.exe.config
    MySql.Data.dll
    Google.Protobuf.dll
    BouncyCastle.Crypto.dll
    CsvHelper.dll
    Ical.Net.dll
    K4os.Compression.LZ4.dll
    K4os.Compression.LZ4.Streams.dll
    K4os.Hash.xxHash.dll
    Microsoft.Bcl.AsyncInterfaces.dll
    Microsoft.Bcl.HashCode.dll
    NodaTime.dll
    Portable.System.DateTimeOnly.dll
    System.Buffers.dll
    System.IO.Pipelines.dll
    System.Memory.dll
    System.Numerics.Vectors.dll
    System.Runtime.CompilerServices.Unsafe.dll
    System.Threading.Tasks.Extensions.dll

  [コピー不要（旧 exe は退避または削除）]
    MySQL_ConnectionTest.exe
    GoogleCalenderReader.exe

  [スコープ外（そのまま残してよい）]
    安否確認システム管理ツール.exe

  コピー前に旧 exe を別フォルダへバックアップしておくと
  問題が起きたとき旧システムへ戻しやすい。

========================================
  3. 設定ファイル（C:\MyReader\）
========================================

  LabManager は ini を C:\MyReader\ 固定で読む。
  旧システムからそのまま流用できる。

  ── SQLReader.ini（DB 接続）──
  例（研究室）:
    UserID =Neko
    PassWd =neko
    ServerIP =192.168.0.2
    ReloadTime =5
    DataBaseName =felica

  キー名は上記のとおり（大文字小文字は Program.cs が Trim して読む）。
  ReloadTime はメイン画面の自動更新間隔（秒）。

  初回起動で ini が無い場合、アプリ内「設定」画面から入力すると
  C:\MyReader\SQLReader.ini が自動作成される。

  ※ DB 接続情報・パスワードは Git にコミットしないこと。

========================================
  4. データベース（MySQL 5.1 注意）
========================================

  LabManager が参照する主なテーブル:
    personal_info, chip_list, touch_log, duty_schedule,
    duty_weekday_roster, lab_calendar_day,
    diary_log, references_list, duty_edit_log

  研究室 DB（2026-06 時点の Phase0 調査）ではコア表は既に存在。
  新規列・新規表が足りない場合のみ db\migrate_*.sql を適用する。

  [適用前に必ずバックアップ]
    mysqldump 例:
      "C:\xampp\mysql\bin\mysqldump.exe" -u Neko -p felica > backup_felica.sql

  [マイグレーション（必要なものだけ）]
    db\migrate_002_add_diary_log.sql           … 日誌Form 用
    db\migrate_003_add_references_list.sql     … 文献管理 用
    db\migrate_004_add_personal_info_mail.sql
    db\migrate_005_add_chip_list_system_id.sql
    db\migrate_006_add_touch_log_terminal_id.sql
    db\migrate_007_add_duty_edit_log.sql       … 出席状況編集ログ
    db\migrate_008_add_lab_calendar_day.sql    … 授業日・予定（必須）
    db\migrate_009_add_duty_weekday_roster.sql … 曜日別日直（必須）

  seed.sql は学生マスタのみ更新し、カレンダー・日直データは消さない。
  デモ初期化が必要なときだけ db\seed_demo_reset.sql を使う。

  実行例（MySQL 5.1 / コマンドプロンプト）:
    "C:\xampp\mysql\bin\mysql.exe" -u Neko -p felica < migrate_002_add_diary_log.sql

  ── MySQL 5.1 での注意点 ──

  ■ バージョン
    研究室は MySQL 5.1。開発PCが 8.0 でも、5.1 向け SQL 構文で
    migrate ファイルは書いてある（IF NOT EXISTS 等は使わない）。

  ■ sql_mode
    ONLY_FULL_GROUP_BY は MySQL 5.7 以降の話。
    5.1 では通常問題にならない（LabManager は接続時に 8.0 向け
    回避コードを入れているが、失敗しても無視される）。

  ■ 接続エラーが出る場合
    - MySQL サービスが起動しているか確認
    - SQLReader.ini の ServerIP（localhost / 192.168.0.2 等）を確認
    - ファイアウォールで 3306 が開いているか
    - MySql.Data 8.x と 5.1 の組み合わせで SSL エラーが出たら
      管理者に相談（接続文字列に SslMode=none が必要な場合あり）

  ■ 文字コード
    接続は Charset=utf8。5.1 の utf8 テーブルと互換。

  ■ 触らないテーブル
    class_info, semester, timetable, touch_log2 等は
    旧システム・他ツール用。LabManager は基本触らない。

========================================
  5. 起動手順
========================================

  1. MySQL が起動していることを確認
  2. C:\MyReader\SQLReader.ini があることを確認
  3. 表示ツール\Screen.bat をダブルクリック

  正常時:
    - 画面左半分: DBカレンダー（TvCalendarPanel / Form14）
    - 画面右半分: メイン画面（在席・日直・先生行）
    - タイトルバー: LabManager（未接続時は [未接続]）

  開発PCで単体確認するとき:
    bin\Release\LabManager.exe          … 右半分 + 左カレンダーが自動表示

  Windows ログイン時に自動起動させる場合:
    Screen.bat のショートカットを
    スタートアップフォルダに置く（任意）。

========================================
  6. Smart App Control / セキュリティ
========================================

  開発中は Smart App Control をオフにしてビルド・実行している。
  テレビPC で初回実行時にブロックされた場合:
    - 署名付きビルドでない exe のため警告が出ることがある
    - 「詳細情報」→ 実行を許可、または SAC をオフにする
    - 研究室の運用ポリシーに従うこと

========================================
  7. 動作確認チェックリスト
========================================

  [ ] Screen.bat で LabManager が起動する
  [ ] 右画面に全学生（学籍番号順）と先生行が表示される
  [ ] 左半分に DB カレンダー（授業日・予定・日直苗字）が表示される
  [ ] MENU → CalendarEditor で授業日・予定の追加/削除ができる
  [ ] MENU → 日直管理 で曜日担当を保存できる
  [ ] migrate_008 / migrate_009 適用済みである
  [ ] 「終了」でアプリが閉じる

  DB 中身の事前調査には tools\lab_phase0\ を使う（Python 単体）。

========================================
  8. トラブルシューティング
========================================

  ■ LabManager.exe が見つからない（Screen.bat）
    → bin\Release の中身を Screen.bat と同じフォルダへコピー

  ■ .NET Framework 4.8 が必要
    → Microsoft 公式から 4.8 をインストール

  ■ 接続に失敗しました
    → SQLReader.ini / MySQL 起動 / ユーザー権限を確認
    → アプリは未接続モードで起動する（UI は触れる）

  ■ 左カレンダーが空 / 授業日が反映されない
    → migrate_008_add_lab_calendar_day.sql を適用
    → MENU → CalendarEditor で授業日を設定

  ■ 当日タッチが無い
    → 正常（警告は出さない仕様）。日直表示は DB の duty_schedule 次第

  ■ 旧システムへ戻す
    → バックアップした MySQL_ConnectionTest.exe 等で Screen.bat を
      旧内容に戻す

========================================
  9. 関連パス一覧
========================================

  リポジトリ
    https://github.com/csno25810/LabManager

  ビルド出力
    bin\Release\LabManager.exe

  デプロイ用スクリプト
    deploy\Screen.bat
    deploy\README.txt（このファイル）

  DB 定義・マイグレーション
    db\schema.sql, db\migrate_*.sql

  研究室 DB 調査ツール
    tools\lab_phase0\
