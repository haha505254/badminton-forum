#!/bin/bash

echo "=== PostgreSQL 狀態檢查 ==="
echo ""

# 檢查 PostgreSQL 是否已安裝
if command -v psql &> /dev/null; then
    echo "✓ PostgreSQL 已安裝"
    psql --version
else
    echo "✗ PostgreSQL 未安裝"
    echo "請執行: ./setup-postgresql.sh 來安裝"
    exit 1
fi

echo ""

# 檢查服務狀態
if systemctl is-active --quiet postgresql; then
    echo "✓ PostgreSQL 服務正在運行"
else
    echo "✗ PostgreSQL 服務未運行"
    echo "請執行: sudo systemctl start postgresql"
fi

echo ""

# 測試連接
echo "測試資料庫連接..."
PGPASSWORD='REMOVED' psql -h localhost -U badmintonuser -d badmintonforumdb -c '\l' &> /dev/null

if [ $? -eq 0 ]; then
    echo "✓ 成功連接到 BadmintonForumDb 資料庫"
    echo ""
    echo "資料庫已準備就緒！"
else
    echo "✗ 無法連接到資料庫"
    echo ""
    echo "可能的原因："
    echo "1. PostgreSQL 未安裝或未運行"
    echo "2. 資料庫或用戶尚未創建"
    echo "3. 密碼不正確"
    echo ""
    echo "請執行安裝腳本: ./setup-postgresql.sh"
fi