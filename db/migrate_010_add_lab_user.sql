-- マルチデバイス用ログイン（LabPortal）
USE felica;

CREATE TABLE IF NOT EXISTS lab_user (
    login_id      VARCHAR(40)  NOT NULL,
    student_id    VARCHAR(20)  NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    role          VARCHAR(20)  NOT NULL DEFAULT 'student',
    PRIMARY KEY (login_id),
    KEY idx_lab_user_student (student_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- 行の投入は LabPortal 初回起動時に personal_info から行う（初期パスワード lab2026）。
-- 既に行がある場合は上書きしない。
