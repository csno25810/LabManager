-- マイグレーション 005: chip_list に system_id 列を追加
-- 研究室DB (felica) の DESCRIBE 結果に合わせる。

USE felica;

SET @col_exists := (
    SELECT COUNT(*) FROM information_schema.columns
    WHERE table_schema = 'felica'
      AND table_name   = 'chip_list'
      AND column_name  = 'system_id'
);

SET @ddl := IF(
    @col_exists = 0,
    'ALTER TABLE chip_list ADD COLUMN system_id VARCHAR(10) NOT NULL DEFAULT '''' AFTER student_id',
    'SELECT "system_id already exists" AS status'
);

PREPARE stmt FROM @ddl;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- 既存行にプレースホルダを入れる
UPDATE chip_list
SET system_id = CONCAT('S', LPAD(SUBSTRING(student_id, 1, 9), 9, '0'))
WHERE system_id = '' OR system_id IS NULL;

SHOW COLUMNS FROM chip_list LIKE 'system_id';
