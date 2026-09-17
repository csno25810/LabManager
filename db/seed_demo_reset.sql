-- デモ用タッチ・日直・カレンダーをまとめて消す（通常のアップデートでは使わない）
USE felica;

DELETE FROM touch_log;
DELETE FROM duty_schedule;
DELETE FROM duty_weekday_roster;
DELETE FROM lab_calendar_day;
