-- マイグレーション 011: タッチ履歴の手動編集ログ
-- Form17（タッチ履歴編集）からの touch_log 追加・変更・削除を記録する。

USE felica;

CREATE TABLE IF NOT EXISTS touch_edit_log (
    id              INT          NOT NULL AUTO_INCREMENT,
    edited_at       DATETIME     NOT NULL,
    action          VARCHAR(10)  NOT NULL,
    touch_id        INT          NULL,
    student_id      VARCHAR(20)  NOT NULL,
    chip_id         VARCHAR(50)  NULL,
    old_time_stamp  DATETIME     NULL,
    new_time_stamp  DATETIME     NULL,
    terminal_id     VARCHAR(10)  NULL,
    PRIMARY KEY (id),
    KEY idx_touch_edit_time (edited_at),
    KEY idx_touch_edit_student (student_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
