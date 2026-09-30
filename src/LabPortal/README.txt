========================================
  LabPortal（マルチデバイス在席確認）
========================================

テレビ用 LabManager とは別アプリです。
同じ MySQL（felica）を読み、ブラウザからログインして在席を見ます。
「自分の履歴」では、ログインした本人の来室日・タッチ・日直だけを表示します。

【ビルド】
  MSBuild LabManager.sln /p:Configuration=Release /p:Platform=x86
  出力: bin\Release\LabPortal.exe

【起動】
  1. C:\MyReader\SQLReader.ini があること（LabManager と同じ）
  2. bin\Release\LabPortal.exe を起動
  3. ブラウザで http://localhost:8080/ を開く
     ログイン後、「全員の在席」と「自分の履歴」を切り替えられる。
  ポートを変える場合: LabPortal.exe 8081

【ログイン】
  学籍番号 + パスワード（先生は teacher）。
  初期パスワード: lab2026
  初回起動時、personal_info から lab_user を自動作成します。
  既に lab_user がある場合は上書きしません。
  ログイン後の「パスワード」から変更できます（4文字以上）。
  学生情報の追加に学生証は不要です。
  Gmailログインは下の手順。学籍番号ログインとは別ボタンです。

【Gmailアカウントでログイン（詳細）】
  目的:
    LabPortal に Gmail のパスワードは入れない。
    Google の画面で本人確認し、戻ってきたメールが
    personal_info.mail と一致すればログインする。

  A. Google Cloud プロジェクトを作る
    1. ブラウザで https://console.cloud.google.com/ を開く
    2. Google アカウントでログインする（自分の Gmail でよい）
    3. 画面上のプロジェクト名をクリック → 「新しいプロジェクト」
    4. 名前例: LabPortal → 作成 → そのプロジェクトを選択する

  B. OAuth 同意画面（初回だけ）
    1. 左メニュー「APIとサービス」→「OAuth 同意画面」
       （新しい画面では「Google Auth platform」→「ブランディング」のこともある）
    2. User Type は「外部」（個人の Google アカウントのとき）
    3. アプリ名: LabPortal
       ユーザーサポートメール: 自分の Gmail
       デベロッパーの連絡先メール: 同じ Gmail
    4. 保存して次へ。スコープは追加しなくてよい（openid / email / profile は標準）
    5. テストユーザーに、ログインさせたい Gmail を追加する
       （公開していない間は、ここに書いた人だけログインできる）

  C. OAuth クライアント ID を作る
    1. 「APIとサービス」→「認証情報」→「認証情報を作成」→「OAuth クライアント ID」
    2. アプリケーションの種類: 「ウェブアプリケーション」
    3. 名前: LabPortal Web
    4. 「承認済みのリダイレクト URI」に次の2つだけを追加して作成
         http://localhost:8080/auth/google/callback
         http://127.0.0.1:8080/auth/google/callback
       http://192.168.x.x:8080/... は入れない。
       Google は社内IPをリダイレクト先に認めない（末尾を .com 等にしろ、と出る）。
    5. 表示された「クライアント ID」と「クライアント シークレット」を控える
       ※ リダイレクト URI は1文字でも違うと redirect_uri_mismatch になる
       ※ Gmailログインは、LabPortal を動かしている PC の
         http://localhost:8080/ で行う。
         スマホからは学籍番号＋パスワードでログインする。
       ※ スマホでも Gmail ログインしたい場合は、あとから
         本物のドメイン＋HTTPS が必要（今の LAN IP では不可）

  D. LabPortal に書き込む
    1. deploy\LabPortal.ini.sample を C:\MyReader\LabPortal.ini にコピー
    2. 次のように書く（引用符は付けない）
         GoogleClientId=（控えたクライアント ID）.apps.googleusercontent.com
         GoogleClientSecret=（控えたシークレット）
    3. LabPortal.exe をいったん終了して、もう一度起動する
    4. コンソールに「Gmailログイン: 有効」と出れば成功
       「未設定」なら ini の場所・キー名・再起動を確認する

  E. 学生情報のメールを Gmail と同じにする
    1. LabManager → 学生情報管理 → 検索・編集
    2. 使う学生（例: 0001）のメールを、Google ログインするアドレスに変更
       例: niko@gmail.com や 日大の Google アカウント
    3. 今の開発データ 0001@example.local のままでは一致しない

  F. 動作確認
    1. LabPortal を起動し、http://localhost:8080/ を開く
    2. 「Gmailアカウントでログイン」を押す
    3. Google のアカウント選択（または日大ログイン）へ移る
    4. 許可すると LabPortal の在席画面に戻る
    5. 「この Gmail は学生情報に登録されていません」と出たら E をやり直す

  うまくいかないとき
    - 未設定表示 … LabPortal.ini が無い／空／再起動していない
    - redirect_uri_mismatch … C の URI と、実際に開いた URL のホストが違う
    - access_denied / テストユーザー … B のテストユーザーに自分の Gmail を足す
    - 学生情報に登録されていません … E のメールが、選んだ Google アカウントと違う

【入室ボタン】
  ログイン後の「全員の在席」に表示します。
  LabPortal と同じ LAN（自宅ならこのPC、研究室なら同じ Wi-Fi）のときだけ押せます。
  押すと touch_log に記録され、テレビの在席と同じ判定になります。
  学生証がなくても使えます。

【スマホ・他PC（同一 LAN）】
  コンソールに表示される http://<このPCのIP>:8080/ を開く。
  Windows ファイアウォールで TCP 8080 を許可する。

【テレビ画面との違い】
  LabManager /tv … 部屋のテレビ。ログインなし。公開してよい情報だけ。
  LabPortal     … 個人端末。ログイン必須。今後カルテ等を足す側。
