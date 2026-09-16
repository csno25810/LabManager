-- マイグレーション 007: 出席状況の手動編集ログ（不正防止用）
-- 開発デバッグ・研究室運用時の duty_schedule 変更を記録する。

USE felica;

CREATE TABLE IF NOT EXISTS duty_edit_log (
    id              INT          NOT NULL AUTO_INCREMENT,
    edited_at       DATETIME     NOT NULL,
    action          VARCHAR(10)  NOT NULL,
    duty_date       DATE         NOT NULL,
    student_id      VARCHAR(20)  NOT NULL,
    old_duty_status INT          NULL,
    new_duty_status INT          NULL,
    old_duty_type   VARCHAR(10)  NULL,
    new_duty_type   VARCHAR(10)  NULL,
    PRIMARY KEY (id),
    KEY idx_duty_edit_time (edited_at),
    KEY idx_duty_edit_date (duty_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
