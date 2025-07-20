#!/bin/bash

echo "=== PostgreSQL 安裝和設置腳本 ==="
echo "請在執行此腳本前確保您有 sudo 權限"
echo ""

# 更新套件列表
echo "1. 更新套件列表..."
sudo apt update

# 安裝 PostgreSQL
echo "2. 安裝 PostgreSQL..."
sudo apt install -y postgresql postgresql-contrib

# 啟動 PostgreSQL 服務
echo "3. 啟動 PostgreSQL 服務..."
sudo systemctl start postgresql
sudo systemctl enable postgresql

# 檢查服務狀態
echo "4. 檢查 PostgreSQL 服務狀態..."
sudo systemctl status postgresql --no-pager

# 創建資料庫和用戶
echo "5. 設置資料庫..."
sudo -u postgres psql << EOF
-- 創建資料庫用戶
CREATE USER badmintonuser WITH PASSWORD 'REMOVED';

-- 創建資料庫
CREATE DATABASE BadmintonForumDb OWNER badmintonuser;

-- 授予所有權限
GRANT ALL PRIVILEGES ON DATABASE BadmintonForumDb TO badmintonuser;

-- 顯示創建的資料庫
\l BadmintonForumDb
EOF

echo ""
echo "=== 安裝完成！==="
echo ""
echo "資料庫資訊："
echo "  資料庫名稱: BadmintonForumDb"
echo "  用戶名: badmintonuser"
echo "  密碼: REMOVED"
echo ""
echo "請記得更新 appsettings.json 中的連接字串！"