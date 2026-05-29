-- マイグレーション 001: duty_schedule に penalty_count を追加し、
-- labapp ユーザーに ALTER 系の権限を付与する。
-- 旧コード (Form9 等) が duty_schedule.penalty_count を直接参照するためのスキーマ修正。

USE felica;

-- penalty_count カラムが無ければ追加 (MySQL 8.0 は ADD COLUMN IF NOT EXISTS 非対応のため動的SQLで判定)
SET @col_exists := (
    SELECT COUNT(*) FROM information_schema.columns
    WHERE table_schema = 'felica'
      AND table_name   = 'duty_schedule'
      AND column_name  = 'penalty_count'
);

SET @ddl := IF(
    @col_exists = 0,
    'ALTER TABLE duty_schedule ADD COLUMN penalty_count INT NOT NULL DEFAULT 0 AFTER duty_status',
    'SELECT "penalty_count already exists" AS status'
);

PREPARE stmt FROM @ddl;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- 既存行の値を 0 に揃える (NOT NULL DEFAULT 0 で既に 0 になっているはずだが念のため)
UPDATE duty_schedule SET penalty_count = 0 WHERE penalty_count IS NULL;

-- 動作確認用: 今日の日直の片方を 2 に揃える
UPDATE duty_schedule SET penalty_count = 2
 WHERE duty_date = CURDATE() AND student_id = '7026';

-- 今後 labapp 経由でスキーマ変更できるように権限を拡張
GRANT SELECT, INSERT, UPDATE, DELETE, ALTER, CREATE, DROP, REFERENCES, INDEX
    ON felica.* TO 'labapp'@'localhost';
FLUSH PRIVILEGES;

SHOW COLUMNS FROM duty_schedule;
