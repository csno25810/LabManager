-- 研究室カレンダー（授業日・予定メモ）ユーザー手動設定用
USE felica;

CREATE TABLE IF NOT EXISTS lab_calendar_day (
    calendar_date DATE         NOT NULL,
    is_class_day  TINYINT(1)   NOT NULL DEFAULT 0,
    memo          VARCHAR(12)  NOT NULL DEFAULT '',
    PRIMARY KEY (calendar_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
