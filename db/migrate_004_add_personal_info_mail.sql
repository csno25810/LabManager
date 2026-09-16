-- マイグレーション 004: personal_info に mail 列を追加
-- 研究室DB (felica) の DESCRIBE 結果に合わせる。

USE felica;

SET @col_exists := (
    SELECT COUNT(*) FROM information_schema.columns
    WHERE table_schema = 'felica'
      AND table_name   = 'personal_info'
      AND column_name  = 'mail'
);

SET @ddl := IF(
    @col_exists = 0,
    'ALTER TABLE personal_info ADD COLUMN mail VARCHAR(40) NOT NULL DEFAULT '''' AFTER name',
    'SELECT "mail already exists" AS status'
);

PREPARE stmt FROM @ddl;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- 既存行にプレースホルダを入れる（空のまま残さない）
UPDATE personal_info
SET mail = CONCAT(student_id, '@example.local')
WHERE mail = '' OR mail IS NULL;

SHOW COLUMNS FROM personal_info LIKE 'mail';
