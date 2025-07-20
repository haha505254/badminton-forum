-- 資料庫初始化腳本
-- 這個檔案會在資料庫第一次啟動時執行

-- 確保資料庫編碼為 UTF-8
ALTER DATABASE badmintonforumdb SET client_encoding = 'UTF8';

-- 給予必要的權限
GRANT ALL PRIVILEGES ON DATABASE badmintonforumdb TO badmintonuser;
GRANT ALL ON SCHEMA public TO badmintonuser;

-- 建立 uuid 擴展（如果需要）
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- 可以在這裡加入初始資料
-- 例如：預設分類、管理員帳號等