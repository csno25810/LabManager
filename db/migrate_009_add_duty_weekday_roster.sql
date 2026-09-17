-- 曜日別日直担当（最大2名/曜日）
USE felica;

CREATE TABLE IF NOT EXISTS duty_weekday_roster (
    weekday    TINYINT      NOT NULL COMMENT '1=月 … 5=金',
    slot       TINYINT      NOT NULL COMMENT '1 or 2',
    student_id VARCHAR(20)  NOT NULL DEFAULT '',
    PRIMARY KEY (weekday, slot)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
