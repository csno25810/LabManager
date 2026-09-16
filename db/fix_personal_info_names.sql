-- personal_info.name repair (ASCII-only SQL; names stored as UTF-8 via UNHEX)
USE felica;

UPDATE personal_info SET name = CONVERT(UNHEX('E794B0E4B8AD20E5A4AAE9838E') USING utf8mb4) WHERE student_id = '7011';
UPDATE personal_info SET name = CONVERT(UNHEX('E4BD90E897A420E88AB1E5AD90') USING utf8mb4) WHERE student_id = '7041';
UPDATE personal_info SET name = CONVERT(UNHEX('E988B4E69CA820E4B880E9838E') USING utf8mb4) WHERE student_id = '7111';
UPDATE personal_info SET name = CONVERT(UNHEX('E9AB98E6A98B20E79C9FE79086') USING utf8mb4) WHERE student_id = '7026';
UPDATE personal_info SET name = CONVERT(UNHEX('E5B1B1E794B020E6ACA1E9838E') USING utf8mb4) WHERE student_id = 'M9013';
UPDATE personal_info SET name = CONVERT(UNHEX('E4BC8AE897A420E7BE8EE592B2') USING utf8mb4) WHERE student_id = 'M9014';
