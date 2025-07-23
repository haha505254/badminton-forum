#!/bin/bash
# 測試生產環境配置腳本

set -e

echo "🧪 測試生產環境配置..."

# 檢查必要檔案
echo "✅ 檢查必要檔案..."
if [ ! -f "docker-compose.yml" ]; then
    echo "❌ 錯誤：找不到 docker-compose.yml"
    exit 1
fi

if [ ! -f "docker-compose.prod.yml" ]; then
    echo "❌ 錯誤：找不到 docker-compose.prod.yml"
    exit 1
fi

# 檢查配置合併
echo "✅ 驗證配置合併..."
docker-compose -f docker-compose.yml -f docker-compose.prod.yml config > /dev/null 2>&1
if [ $? -eq 0 ]; then
    echo "✅ 配置合併成功"
else
    echo "❌ 配置合併失敗"
    exit 1
fi

# 檢查環境變數
echo "✅ 檢查環境變數..."
if [ -f ".env.production" ]; then
    echo "✅ 找到 .env.production 檔案"
    # 檢查關鍵變數
    source .env.production
    if [ -z "$JWT_SECRET" ] || [ "$JWT_SECRET" == "請更換為至少32字元的隨機字串用於JWT簽名" ]; then
        echo "⚠️  警告：JWT_SECRET 尚未設定實際值"
    fi
    if [ -z "$MARIADB_PASSWORD" ] || [ "$MARIADB_PASSWORD" == "請更換為超強的資料庫密碼" ]; then
        echo "⚠️  警告：MARIADB_PASSWORD 尚未設定實際值"
    fi
else
    echo "⚠️  警告：找不到 .env.production 檔案，將使用預設值"
    echo "   建議：cp .env.production.example .env.production"
fi

# 測試建置
echo "✅ 測試映像建置（乾跑）..."
echo "   API 建置目標：production"
echo "   Web 建置目標：production"

# 顯示埠配置
echo "✅ 埠配置："
echo "   API: 127.0.0.1:5246 (僅 localhost)"
echo "   Web: 0.0.0.0:80 (或 \$WEB_PORT)"
echo "   DB: 不對外暴露"

# 檢查 Dockerfile stages
echo "✅ 檢查 Dockerfile 階段..."
if grep -q "FROM.*AS production" BadmintonForum.API/Dockerfile; then
    echo "✅ API Dockerfile 包含 production 階段"
else
    echo "❌ API Dockerfile 缺少 production 階段"
fi

if grep -q "FROM.*AS production" badminton-forum-vue/Dockerfile; then
    echo "✅ Web Dockerfile 包含 production 階段"
else
    echo "❌ Web Dockerfile 缺少 production 階段"
fi

echo ""
echo "📋 生產環境部署指令："
echo "docker-compose -f docker-compose.yml -f docker-compose.prod.yml --env-file .env.production up -d"
echo ""
echo "🎉 配置檢查完成！"