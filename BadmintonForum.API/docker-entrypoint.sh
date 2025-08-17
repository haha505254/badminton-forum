#!/bin/bash
set -e

# 從環境變數取得資料庫連線資訊
DB_HOST="${DB_HOST:-db}"
DB_USER="${MARIADB_USER:-badmintonuser}"
DB_PASSWORD="${MARIADB_PASSWORD:-BadmintonPass123}"
DB_NAME="${MARIADB_DATABASE:-badmintonforumdb}"

echo "等待資料庫就緒..."
echo "連線資訊: $DB_USER@$DB_HOST (資料庫: $DB_NAME)"

# 等待資料庫可用 (MariaDB)
until mariadb -h $DB_HOST -P 3306 -u $DB_USER -p$DB_PASSWORD -e "SELECT 1" > /dev/null 2>&1; do
  echo "資料庫尚未就緒，等待中..."
  sleep 2
done

echo "資料庫已就緒！"

# 啟動應用程式
echo "啟動 API 服務..."
if [ "$ASPNETCORE_ENVIRONMENT" = "Development" ]; then
  echo "以開發模式啟動 (支援熱重載)..."
  # 使用 idempotent SQL 執行資料庫遷移（更安全）
  echo "生成 idempotent migration SQL..."
  dotnet ef migrations script --idempotent -o /tmp/migrations.sql
  
  echo "執行資料庫遷移..."
  mariadb -h $DB_HOST -P 3306 -u $DB_USER -p$DB_PASSWORD --default-character-set=utf8mb4 $DB_NAME < /tmp/migrations.sql
  
  echo "資料庫遷移完成！"
  dotnet watch run --no-launch-profile --urls http://+:5246
else
  echo "以生產模式啟動..."
  # 生產環境執行 DLL
  exec dotnet BadmintonForum.API.dll
fi