-- マイグレーション 012: 個人カルテ用
-- progress_log を追加し、references_list に student_id を足す。
-- Form8 の文献登録はそのまま使える（student_id は空のまま）。

USE felica;

CREATE TABLE IF NOT EXISTS progress_log (
    id            INT          NOT NULL AUTO_INCREMENT,
    student_id    VARCHAR(20)  NOT NULL,
    progress_date DATE         NOT NULL,
    title         VARCHAR(255) NOT NULL,
    content       TEXT         NOT NULL,
    PRIMARY KEY (id),
    KEY idx_progress_student (student_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

SET @col_exists := (
    SELECT COUNT(*) FROM information_schema.columns
    WHERE table_schema = 'felica'
      AND table_name   = 'references_list'
      AND column_name  = 'student_id'
);

SET @ddl := IF(
    @col_exists = 0,
    'ALTER TABLE references_list ADD COLUMN student_id VARCHAR(20) NOT NULL DEFAULT '''' AFTER id',
    'SELECT "student_id already exists" AS status'
);

PREPARE stmt FROM @ddl;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

SHOW TABLES LIKE 'progress_log';
SHOW COLUMNS FROM references_list LIKE 'student_id';
