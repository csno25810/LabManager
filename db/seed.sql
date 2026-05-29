-- LabManager: 開発用ダミーデータ
-- アプリの動作確認に使うため、当日の在席状況・日直状況を含めて投入する

USE felica;

-- 研究生情報
INSERT INTO personal_info (student_id, name, penalty_count) VALUES
    ('7011',  '田中 太郎', 0),
    ('7041',  '佐藤 花子', 1),
    ('7111',  '鈴木 一郎', 0),
    ('7026',  '高橋 真理', 2),
    ('M9013', '山田 次郎', 0),
    ('M9014', '伊藤 美咲', 0)
ON DUPLICATE KEY UPDATE
    name          = VALUES(name),
    penalty_count = VALUES(penalty_count);

-- NFCチップ対応
INSERT INTO chip_list (chip_id, student_id) VALUES
    ('CHIP001', '7011'),
    ('CHIP002', '7041'),
    ('CHIP003', '7111'),
    ('CHIP004', '7026'),
    ('CHIP005', 'M9013'),
    ('CHIP006', 'M9014')
ON DUPLICATE KEY UPDATE student_id = VALUES(student_id);

-- 当日のタッチ履歴（既存があれば残す形で追加）
-- 田中: 入(09:05) → 退(12:00) → 入(13:00) ⇒ 在席
-- 佐藤: 入(09:12) → 退(10:15) → 入(10:45)  ⇒ 在席
-- 鈴木: 入(09:30)                          ⇒ 在席
-- 高橋: 入(11:00)                          ⇒ 在席（遅刻）
-- 山田: 入(13:30)                          ⇒ 在席
-- 伊藤: 入(09:00) → 退(11:30)              ⇒ 不在
INSERT INTO touch_log (time_stamp, chip_id) VALUES
    (CONCAT(CURDATE(), ' 09:05:00'), 'CHIP001'),
    (CONCAT(CURDATE(), ' 09:12:00'), 'CHIP002'),
    (CONCAT(CURDATE(), ' 09:30:00'), 'CHIP003'),
    (CONCAT(CURDATE(), ' 10:15:00'), 'CHIP002'),
    (CONCAT(CURDATE(), ' 10:45:00'), 'CHIP002'),
    (CONCAT(CURDATE(), ' 11:00:00'), 'CHIP004'),
    (CONCAT(CURDATE(), ' 12:00:00'), 'CHIP001'),
    (CONCAT(CURDATE(), ' 13:00:00'), 'CHIP001'),
    (CONCAT(CURDATE(), ' 13:30:00'), 'CHIP005'),
    (CONCAT(CURDATE(), ' 09:00:00'), 'CHIP006'),
    (CONCAT(CURDATE(), ' 11:30:00'), 'CHIP006');

-- 当日の日直
INSERT INTO duty_schedule (duty_date, student_id, duty_status, duty_type) VALUES
    (CURDATE(), '7011', 1, '0'),
    (CURDATE(), '7026', 2, '0')
ON DUPLICATE KEY UPDATE
    duty_status = VALUES(duty_status),
    duty_type   = VALUES(duty_type);
