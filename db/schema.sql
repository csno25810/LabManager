-- LabManager: 研究室統合管理システム スキーマ定義
-- 自宅PCでの開発用ローカルDB、および研究室DBの再現に使用する

CREATE DATABASE IF NOT EXISTS felica
    DEFAULT CHARACTER SET utf8mb4
    DEFAULT COLLATE utf8mb4_unicode_ci;

USE felica;

-- 研究生情報
CREATE TABLE IF NOT EXISTS personal_info (
    student_id    VARCHAR(20)  NOT NULL,
    name          VARCHAR(50)  NOT NULL,
    penalty_count INT          NOT NULL DEFAULT 0,
    PRIMARY KEY (student_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- NFCチップと学籍番号の対応
CREATE TABLE IF NOT EXISTS chip_list (
    chip_id    VARCHAR(50) NOT NULL,
    student_id VARCHAR(20) NOT NULL,
    PRIMARY KEY (chip_id),
    KEY idx_chip_list_student (student_id),
    CONSTRAINT fk_chip_list_student
        FOREIGN KEY (student_id) REFERENCES personal_info(student_id)
        ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- タッチ履歴
CREATE TABLE IF NOT EXISTS touch_log (
    id         INT          NOT NULL AUTO_INCREMENT,
    time_stamp DATETIME     NOT NULL,
    chip_id    VARCHAR(50)  NOT NULL,
    PRIMARY KEY (id),
    KEY idx_touch_log_chip (chip_id),
    KEY idx_touch_log_time (time_stamp),
    CONSTRAINT fk_touch_log_chip
        FOREIGN KEY (chip_id) REFERENCES chip_list(chip_id)
        ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 日直スケジュール
-- duty_status:   0=未出席 / 1=出席 / 2=遅刻
-- duty_type:     '0'=通常日直 / '1'=罰直
-- penalty_count: 当該日の罰直カウント (旧コードの Form9 等が duty_schedule から直接参照する)
CREATE TABLE IF NOT EXISTS duty_schedule (
    duty_date     DATE         NOT NULL,
    student_id    VARCHAR(20)  NOT NULL,
    duty_status   INT          NOT NULL DEFAULT 0,
    penalty_count INT          NOT NULL DEFAULT 0,
    duty_type     VARCHAR(10)  NOT NULL DEFAULT '0',
    PRIMARY KEY (duty_date, student_id),
    KEY idx_duty_student (student_id),
    CONSTRAINT fk_duty_student
        FOREIGN KEY (student_id) REFERENCES personal_info(student_id)
        ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- アプリ用の専用ユーザー作成
-- 古い MySql.Data 5.0.9 と互換を取るため mysql_native_password を使用
CREATE USER IF NOT EXISTS 'labapp'@'localhost'
    IDENTIFIED WITH mysql_native_password BY 'labapp_pw';

GRANT SELECT, INSERT, UPDATE, DELETE, ALTER, CREATE, DROP, REFERENCES, INDEX
    ON felica.* TO 'labapp'@'localhost';
FLUSH PRIVILEGES;
