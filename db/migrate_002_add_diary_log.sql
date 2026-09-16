-- マイグレーション 002: diary_log テーブルを追加（Form6 日誌提出用）
-- 研究室DB (felica) の DESCRIBE 結果に合わせる。

USE felica;

CREATE TABLE IF NOT EXISTS diary_log (
    student_id  VARCHAR(20)  NOT NULL,
    dialy_date  DATE         NOT NULL,
    title       VARCHAR(255) NOT NULL,
    content     TEXT         NOT NULL,
    KEY idx_diary_student (student_id),
    KEY idx_diary_date (dialy_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

SHOW TABLES LIKE 'diary_log';
DESCRIBE diary_log;
