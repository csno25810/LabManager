# -*- coding: utf-8 -*-
"""
研究室テレビPC用: felica DB の状態を db_info.txt に書き出す。
mysql.exe が無くても、Python + mysql.connector があれば動く。
"""

from __future__ import print_function

import os
import sys
from datetime import datetime

try:
    import mysql.connector
    from mysql.connector import Error
except ImportError:
    print("エラー: mysql.connector が見つかりません。")
    print("  pip install mysql-connector-python")
    print("  または、研究室PCの「その他\\タッチする.py」が動く環境なら同じ Python を使ってください。")
    input("Enter で終了...")
    sys.exit(1)

SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))
OUTPUT_FILE = os.path.join(SCRIPT_DIR, "db_info.txt")

INI_PATHS = [
    r"C:\MyReader\SQLReader.ini",
    os.path.join(SCRIPT_DIR, "SQLReader.ini"),
]

TABLES_TO_CHECK = [
    "personal_info",
    "chip_list",
    "touch_log",
    "duty_schedule",
    "diary_log",
    "references_list",
]


def read_ini(path):
    settings = {}
    if not os.path.isfile(path):
        return settings
    with open(path, "r", encoding="utf-8", errors="replace") as f:
        for line in f:
            line = line.strip()
            if not line or "=" not in line:
                continue
            key, value = line.split("=", 1)
            settings[key.strip()] = value.strip()
    return settings


def load_settings():
    settings = {
        "UserID": "Neko",
        "PassWd": "neko",
        "ServerIP": "127.0.0.1",
        "DataBaseName": "felica",
    }
    ini_used = None
    for path in INI_PATHS:
        if os.path.isfile(path):
            settings.update(read_ini(path))
            ini_used = path
            break
    return settings, ini_used


def connect(host, user, password, database):
    return mysql.connector.connect(
        host=host,
        user=user,
        password=password,
        database=database,
        charset="utf8",
        connection_timeout=10,
    )


def try_connect(settings):
    hosts = []
    for h in ("localhost", "127.0.0.1", settings.get("ServerIP", "")):
        if h and h not in hosts:
            hosts.append(h)

    errors = []
    for host in hosts:
        try:
            cnx = connect(
                host,
                settings["UserID"],
                settings["PassWd"],
                settings["DataBaseName"],
            )
            return cnx, host
        except Error as err:
            errors.append("{0}: {1}".format(host, err))
    raise Error("接続できませんでした:\n" + "\n".join(errors))


def write_line(out, text=""):
    out.write(text + "\n")
    print(text)


def collect_db_info(cnx, host, settings, ini_used):
    lines = []
    out = lines.append

    out("=" * 60)
    out("LabManager 移行用 DB 情報 (Phase 0)")
    out("取得日時: {0}".format(datetime.now().strftime("%Y-%m-%d %H:%M:%S")))
    out("=" * 60)
    out("")
    out("[接続情報]")
    out("  接続先ホスト: {0}".format(host))
    out("  ユーザー    : {0}".format(settings.get("UserID", "")))
    out("  データベース: {0}".format(settings.get("DataBaseName", "")))
    out("  ini ファイル: {0}".format(ini_used or "(デフォルト値を使用)"))
    out("")

    cur = cnx.cursor()

    cur.execute("SELECT VERSION()")
    version = cur.fetchone()[0]
    out("[MySQL バージョン]")
    out("  {0}".format(version))
    out("")

    cur.execute("SELECT DATABASE()")
    out("[現在の DB]")
    out("  {0}".format(cur.fetchone()[0]))
    out("")

    cur.execute("SHOW TABLES")
    tables = [row[0] for row in cur.fetchall()]
    out("[テーブル一覧] ({0} 件)".format(len(tables)))
    for name in tables:
        out("  - {0}".format(name))
    out("")

    for table in TABLES_TO_CHECK:
        out("-" * 60)
        out("[DESCRIBE {0}]".format(table))
        if table not in tables:
            out("  (テーブルなし)")
            out("")
            continue
        cur.execute("DESCRIBE `{0}`".format(table))
        rows = cur.fetchall()
        out("  {0:<20} {1:<20} {2:<6} {3:<6} {4}".format(
            "Field", "Type", "Null", "Key", "Default"))
        for row in rows:
            out("  {0:<20} {1:<20} {2:<6} {3:<6} {4}".format(
                str(row[0]), str(row[1]), str(row[2]), str(row[3]), str(row[4])))
        out("")

    out("-" * 60)
    out("[LabManager が追加で使うテーブル/列の有無]")
    checks = [
        ("personal_info.penalty_count", "personal_info", "penalty_count"),
        ("duty_schedule", "duty_schedule", None),
        ("diary_log", "diary_log", None),
        ("references_list", "references_list", None),
    ]
    table_set = set(tables)
    col_cache = {}
    for label, table, column in checks:
        if table not in table_set:
            out("  {0}: なし (テーブル自体が無い)".format(label))
            continue
        if column is None:
            out("  {0}: あり".format(label))
            continue
        if table not in col_cache:
            cur.execute("DESCRIBE `{0}`".format(table))
            col_cache[table] = [r[0] for r in cur.fetchall()]
        if column in col_cache[table]:
            out("  {0}: あり".format(label))
        else:
            out("  {0}: なし (列が無い → マイグレで追加が必要)".format(label))
    out("")

    cur.close()
    return "\n".join(lines)


def find_mysqldump():
    candidates = [
        r"C:\Program Files\MySQL\MySQL Server 8.0\bin\mysqldump.exe",
        r"C:\Program Files\MySQL\MySQL Server 5.7\bin\mysqldump.exe",
        r"C:\Program Files (x86)\MySQL\MySQL Server 5.7\bin\mysqldump.exe",
        r"C:\Program Files (x86)\MySQL\MySQL Server 5.6\bin\mysqldump.exe",
        r"C:\Program Files (x86)\MySQL\MySQL Server 5.5\bin\mysqldump.exe",
        r"C:\xampp\mysql\bin\mysqldump.exe",
    ]
    for path in candidates:
        if os.path.isfile(path):
            return path
    return None


def main():
    print("")
    print("研究室 DB 情報取得ツール (Phase 0)")
    print("")

    settings, ini_used = load_settings()
    print("設定を読み込みました。")
    if ini_used:
        print("  ini: {0}".format(ini_used))
    else:
        print("  ini: 見つからないためデフォルト (Neko/neko/felica) を使用")

    try:
        cnx, host = try_connect(settings)
        print("接続成功: {0}".format(host))
    except Error as err:
        print("")
        print("接続に失敗しました:")
        print(err)
        print("")
        print("確認すること:")
        print("  1. MySQL サービスが起動しているか")
        print("  2. C:\\MyReader\\SQLReader.ini の内容")
        print("  3. ユーザー名・パスワード (Neko / neko)")
        input("Enter で終了...")
        sys.exit(1)

    try:
        text = collect_db_info(cnx, host, settings, ini_used)
        with open(OUTPUT_FILE, "w", encoding="utf-8") as f:
            f.write(text)

        dump_path = find_mysqldump()
        with open(OUTPUT_FILE, "a", encoding="utf-8") as f:
            f.write("\n")
            f.write("-" * 60 + "\n")
            f.write("[バックアップ (mysqldump) の案内]\n")
            if dump_path:
                f.write("  mysqldump.exe の場所: {0}\n".format(dump_path))
                f.write("  実行例 (PowerShell):\n")
                f.write('    & "{0}" -u {1} -p {2} > backup_felica.sql\n'.format(
                    dump_path, settings["UserID"], settings["DataBaseName"]))
            else:
                f.write("  mysqldump.exe はこの PC 上では見つかりませんでした。\n")
                f.write("  db_info.txt だけでも持ち帰れば Phase 0 は可能です。\n")

        print("")
        print("完了しました。")
        print("  出力ファイル: {0}".format(OUTPUT_FILE))
        print("")
        print("この db_info.txt を USB に入れて自宅PCへ持ち帰ってください。")
    finally:
        cnx.close()

    input("Enter で終了...")


if __name__ == "__main__":
    main()
