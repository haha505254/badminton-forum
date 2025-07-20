#!/bin/bash
set -e

echo "等待資料庫就緒..."

# 等待資料庫可用
until pg_isready -h db -p 5432 -U badmintonuser; do
  echo "資料庫尚未就緒，等待中..."
  sleep 2
done

echo "資料庫已就緒！"

# 執行資料庫遷移
echo "執行資料庫遷移..."
dotnet ef database update

# 啟動應用程式 (開發模式使用 watch)
echo "啟動 API 服務..."
if [ "$ASPNETCORE_ENVIRONMENT" = "Development" ]; then
  echo "以開發模式啟動 (支援熱重載)..."
  dotnet watch run --no-launch-profile --urls http://+:5246
else
  echo "以生產模式啟動..."
  dotnet BadmintonForum.API.dll
fi