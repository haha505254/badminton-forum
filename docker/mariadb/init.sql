-- MariaDB 初始化腳本
-- 這個檔案會在資料庫第一次啟動時執行

-- 確保資料庫編碼為 UTF-8
ALTER DATABASE badmintonforumdb CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- 給予必要的權限
GRANT ALL PRIVILEGES ON badmintonforumdb.* TO 'badmintonuser'@'%';
FLUSH PRIVILEGES;

-- 建立資料表時的預設設定
SET default_storage_engine = InnoDB;

-- 可以在這裡加入初始資料
-- 例如：預設分類、管理員帳號等