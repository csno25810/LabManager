-- マイグレーション 003: references_list テーブルを追加（Form8 文献管理用）
-- 研究室DB (felica) の DESCRIBE 結果に合わせる。

USE felica;

CREATE TABLE IF NOT EXISTS references_list (
    id             INT          NOT NULL AUTO_INCREMENT,
    title          VARCHAR(255) NOT NULL,
    author         VARCHAR(255) NOT NULL,
    thesis_type    VARCHAR(255) NULL,
    year           VARCHAR(11)  NOT NULL,
    notes          TEXT         NOT NULL,
    url            VARCHAR(255) NULL,
    site_name      VARCHAR(255) NULL,
    open_date      VARCHAR(11)  NOT NULL,
    reference_type VARCHAR(10)  NULL,
    pages          VARCHAR(50)  NOT NULL,
    genre          VARCHAR(50)  NOT NULL,
    PRIMARY KEY (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

SHOW TABLES LIKE 'references_list';
DESCRIBE references_list;
