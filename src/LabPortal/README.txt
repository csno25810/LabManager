========================================
  LabPortal（マルチデバイス在席確認）
========================================

テレビ用 LabManager とは別アプリです。
同じ MySQL（felica）を読み、ブラウザからログインして在席を見ます。

【ビルド】
  MSBuild LabManager.sln /p:Configuration=Release /p:Platform=x86
  出力: bin\Release\LabPortal.exe

【起動】
  1. C:\MyReader\SQLReader.ini があること（LabManager と同じ）
  2. bin\Release\LabPortal.exe を起動
  3. ブラウザで http://localhost:8080/ を開く
  ポートを変える場合: LabPortal.exe 8081

【ログイン】
  学生: 学籍番号 / 初期パスワード lab2026
  先生: teacher / lab2026
  初回起動時、personal_info から lab_user を自動作成します。
  既に lab_user がある場合は上書きしません。

【スマホ・他PC（同一 LAN）】
  コンソールに表示される http://<このPCのIP>:8080/ を開く。
  Windows ファイアウォールで TCP 8080 を許可する。

【テレビ画面との違い】
  LabManager /tv … 部屋のテレビ。ログインなし。公開してよい情報だけ。
  LabPortal     … 個人端末。ログイン必須。今後カルテ等を足す側。
