# db/

LabManager の開発用 MySQL スキーマ定義とサンプルデータ。

## 内容

| ファイル | 内容 |
|----------|------|
| `schema.sql` | データベース `felica` とテーブル5つ（`personal_info` / `chip_list` / `touch_log` / `duty_schedule` / `diary_log`）、およびアプリ用ユーザー `labapp` を作成 |
| `seed.sql`   | 開発・動作確認用のダミーデータを投入（当日のタッチ履歴・日直情報・日誌サンプルを含む） |
| `migrate_002_add_diary_log.sql` | 既存DBに `diary_log` テーブルを追加（Form6 用） |

## 適用方法（ローカルMySQLに対して）

PowerShell で以下を順に実行する。`-p` オプションでパスワードが対話入力で求められる。

```powershell
mysql -u root -p < db\schema.sql
mysql -u root -p < db\seed.sql
```

既存DBを更新する場合（テーブル追加のみ）:

```powershell
mysql -u root -p < db\migrate_002_add_diary_log.sql
mysql -u root -p < db\seed.sql
```

## アプリ用ユーザー

`schema.sql` 内で以下のユーザーを作成している：

- ユーザー名: `labapp`
- パスワード: `labapp_pw`
- 認証方式: `mysql_native_password`（MySql.Data 5.0.9 との互換のため）
- 権限: `felica.*` に対する SELECT / INSERT / UPDATE / DELETE

ローカル開発専用なので、研究室DBには絶対に同じパスワードを使わないこと。
