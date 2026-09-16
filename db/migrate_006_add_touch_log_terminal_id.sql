-- マイグレーション 006: touch_log に terminal_id 列を追加
-- 研究室DB (felica) の DESCRIBE 結果に合わせる。

USE felica;

SET @col_exists := (
    SELECT COUNT(*) FROM information_schema.columns
    WHERE table_schema = 'felica'
      AND table_name   = 'touch_log'
      AND column_name  = 'terminal_id'
);

SET @ddl := IF(
    @col_exists = 0,
    'ALTER TABLE touch_log ADD COLUMN terminal_id VARCHAR(10) NOT NULL DEFAULT ''001'' AFTER time_stamp',
    'SELECT "terminal_id already exists" AS status'
);

PREPARE stmt FROM @ddl;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

SHOW COLUMNS FROM touch_log LIKE 'terminal_id';
