#!/bin/bash
set -e

echo "等待資料庫就緒..."

# 等待資料庫可用 (MariaDB)
until mariadb -h db -P 3306 -u badmintonuser -pBadmintonPass123 -e "SELECT 1" > /dev/null 2>&1; do
  echo "資料庫尚未就緒，等待中..."
  sleep 2
done

echo "資料庫已就緒！"

# 啟動應用程式
echo "啟動 API 服務..."
if [ "$ASPNETCORE_ENVIRONMENT" = "Development" ]; then
  echo "以開發模式啟動 (支援熱重載)..."
  # 執行資料庫遷移 (開發環境有 ef 工具)
  dotnet ef database update
  dotnet watch run --no-launch-profile --urls http://+:5246
else
  echo "以生產模式啟動..."
  # 生產環境直接執行已編譯的 dll
  exec dotnet BadmintonForum.API.dll
fi