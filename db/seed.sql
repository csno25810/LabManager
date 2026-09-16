-- LabManager: 開発用ダミーデータ
-- アプリの動作確認に使うため、当日の在席状況・日直状況を含めて投入する

USE felica;

-- 研究生情報
INSERT INTO personal_info (student_id, name, mail, penalty_count) VALUES
    ('7011',  '田中 太郎', '7011@example.local', 0),
    ('7041',  '佐藤 花子', '7041@example.local', 1),
    ('7111',  '鈴木 一郎', '7111@example.local', 0),
    ('7026',  '高橋 真理', '7026@example.local', 2),
    ('M9013', '山田 次郎', 'm9013@example.local', 0),
    ('M9014', '伊藤 美咲', 'm9014@example.local', 0)
ON DUPLICATE KEY UPDATE
    name          = VALUES(name),
    mail          = VALUES(mail),
    penalty_count = VALUES(penalty_count);

-- NFCチップ対応
INSERT INTO chip_list (chip_id, student_id, system_id) VALUES
    ('CHIP001', '7011',  'S000007011'),
    ('CHIP002', '7041',  'S000007041'),
    ('CHIP003', '7111',  'S000007111'),
    ('CHIP004', '7026',  'S000007026'),
    ('CHIP005', 'M9013', 'S0000M9013'),
    ('CHIP006', 'M9014', 'S0000M9014')
ON DUPLICATE KEY UPDATE
    student_id = VALUES(student_id),
    system_id  = VALUES(system_id);

-- 当日のタッチ履歴（既存があれば残す形で追加）
-- 田中: 入(09:05) → 退(12:00) → 入(13:00) ⇒ 在席
-- 佐藤: 入(09:12) → 退(10:15) → 入(10:45)  ⇒ 在席
-- 鈴木: 入(09:30)                          ⇒ 在席
-- 高橋: 入(11:00)                          ⇒ 在席（遅刻）
-- 山田: 入(13:30)                          ⇒ 在席
-- 伊藤: 入(09:00) → 退(11:30)              ⇒ 不在
INSERT INTO touch_log (time_stamp, terminal_id, chip_id) VALUES
    (CONCAT(CURDATE(), ' 09:05:00'), '001', 'CHIP001'),
    (CONCAT(CURDATE(), ' 09:12:00'), '001', 'CHIP002'),
    (CONCAT(CURDATE(), ' 09:30:00'), '001', 'CHIP003'),
    (CONCAT(CURDATE(), ' 10:15:00'), '001', 'CHIP002'),
    (CONCAT(CURDATE(), ' 10:45:00'), '001', 'CHIP002'),
    (CONCAT(CURDATE(), ' 11:00:00'), '001', 'CHIP004'),
    (CONCAT(CURDATE(), ' 12:00:00'), '001', 'CHIP001'),
    (CONCAT(CURDATE(), ' 13:00:00'), '001', 'CHIP001'),
    (CONCAT(CURDATE(), ' 13:30:00'), '001', 'CHIP005'),
    (CONCAT(CURDATE(), ' 09:00:00'), '001', 'CHIP006'),
    (CONCAT(CURDATE(), ' 11:30:00'), '001', 'CHIP006');

-- 当日の日直
INSERT INTO duty_schedule (duty_date, student_id, duty_status, penalty_count, duty_type) VALUES
    (CURDATE(), '7011', 1, 0, '0'),
    (CURDATE(), '7026', 2, 2, '0')
ON DUPLICATE KEY UPDATE
    duty_status   = VALUES(duty_status),
    penalty_count = VALUES(penalty_count),
    duty_type     = VALUES(duty_type);

-- 日誌サンプル（Form6 動作確認用）
INSERT INTO diary_log (student_id, dialy_date, title, content) VALUES
    ('7011', CURDATE(), '本日の作業', 'LabManager の schema 整備を進めた。'),
    ('7041', DATE_SUB(CURDATE(), INTERVAL 1 DAY), '文献調査', '関連論文を3本読んだ。');

-- 文献サンプル（Form8 動作確認用）
INSERT INTO references_list
    (title, author, thesis_type, year, notes, url, site_name, open_date, reference_type, pages, genre)
VALUES
    ('Example Paper Title', 'Tanaka Taro', 'journal', '2024', 'Survey chapter 2', '', '', '2024-06-01', '', '12-20', '経路'),
    ('Example Web Article', '', '', '2025', 'Useful reference page', 'https://example.com/article', 'Example Site', '2025-09-01', '', '', 'WEB');
