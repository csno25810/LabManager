-- LabManager: 開発用マスタデータ（学生・先生）
-- ※ lab_calendar_day / duty_weekday_roster / duty_schedule は削除しません。
--    カレンダー・日直データを初期化したい場合のみ seed_demo_reset.sql を使ってください。
-- ※ lab_user（LabPortal ログイン）も削除しません。初期ユーザは LabPortal 初回起動で作ります。
-- 実行例: mysql --default-character-set=utf8mb4 -u labapp -p felica < db\seed.sql

USE felica;

DELETE FROM personal_info WHERE student_id IN ('T001', 'T002');
DELETE FROM duty_weekday_roster WHERE student_id IN ('T001', 'T002');

INSERT INTO personal_info (student_id, name, mail, penalty_count) VALUES
    ('0001', CONVERT(UNHEX('E794B0E4B8ADE581A5E5A4AA') USING utf8mb4), '0001@example.local', 0),
    ('0002', CONVERT(UNHEX('E4BD90E897A4E7BE8EE592B2') USING utf8mb4), '0002@example.local', 0),
    ('0003', CONVERT(UNHEX('E988B4E69CA8E5A4A7E8BC94') USING utf8mb4), '0003@example.local', 0),
    ('0004', CONVERT(UNHEX('E9AB98E6A98BE7B590E8A1A3') USING utf8mb4), '0004@example.local', 0),
    ('0005', CONVERT(UNHEX('E4BC8AE897A4E7BF94E5A4AA') USING utf8mb4), '0005@example.local', 0),
    ('0006', CONVERT(UNHEX('E6B8A1E8BEBAE38195E3818FE38289') USING utf8mb4), '0006@example.local', 0),
    ('M0007', CONVERT(UNHEX('E4B8ADE69D91E7A094E4BA8C') USING utf8mb4), 'm0007@example.local', 0),
    ('@', CONVERT(UNHEX('E58588E7949F') USING utf8mb4), 'teacher@example.local', 0)
ON DUPLICATE KEY UPDATE
    name          = VALUES(name),
    mail          = VALUES(mail),
    penalty_count = VALUES(penalty_count);

INSERT INTO chip_list (chip_id, student_id, system_id) VALUES
    ('CHIP001', '0001', 'S000000001'),
    ('CHIP002', '0002', 'S000000002'),
    ('CHIP003', '0003', 'S000000003'),
    ('CHIP004', '0004', 'S000000004'),
    ('CHIP005', '0005', 'S000000005'),
    ('CHIP006', '0006', 'S000000006'),
    ('CHIP007', 'M0007', 'S0000M0007')
ON DUPLICATE KEY UPDATE
    student_id = VALUES(student_id),
    system_id  = VALUES(system_id);
