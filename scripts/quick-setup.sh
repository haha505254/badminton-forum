#!/bin/bash

# ==========================================
# Badminton Forum 快速設定腳本
# ==========================================
# 此腳本協助新使用者快速設定開發環境

set -e  # 遇到錯誤時停止執行

# 顏色定義
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# 顯示歡迎訊息
echo -e "${BLUE}=====================================${NC}"
echo -e "${BLUE}  Badminton Forum 環境設定工具${NC}"
echo -e "${BLUE}=====================================${NC}"
echo ""

# 檢查是否在專案根目錄
if [ ! -f "docker-compose.yml" ]; then
    echo -e "${RED}錯誤: 請在專案根目錄執行此腳本${NC}"
    exit 1
fi

# 函數：產生隨機密碼
generate_password() {
    openssl rand -base64 32 2>/dev/null || cat /dev/urandom | tr -dc 'a-zA-Z0-9!@#$%^&*()_+' | fold -w 32 | head -n 1
}

# 函數：產生 JWT Secret
generate_jwt_secret() {
    openssl rand -base64 48 2>/dev/null || cat /dev/urandom | tr -dc 'a-zA-Z0-9' | fold -w 48 | head -n 1
}

# 步驟 1: 設定後端環境變數
echo -e "${YELLOW}步驟 1: 設定環境變數${NC}"
echo "------------------------"

if [ -f ".env" ]; then
    echo -e "${YELLOW}發現既有的 .env 檔案${NC}"
    read -p "要覆蓋現有設定嗎？ (y/N): " -n 1 -r
    echo
    if [[ ! $REPLY =~ ^[Yy]$ ]]; then
        echo "保留現有 .env 檔案"
    else
        cp .env.example .env
        echo -e "${GREEN}✓ 已從範例檔案建立新的 .env${NC}"
    fi
else
    cp .env.example .env
    echo -e "${GREEN}✓ 已建立 .env 檔案${NC}"
fi

# 步驟 2: 設定前端環境變數
echo ""
echo -e "${YELLOW}步驟 2: 設定前端環境變數${NC}"
echo "------------------------"

FRONTEND_ENV="badminton-forum-vue/.env.development"
FRONTEND_EXAMPLE="badminton-forum-vue/.env.development.example"

if [ -f "$FRONTEND_ENV" ]; then
    echo -e "${YELLOW}發現既有的前端環境檔案${NC}"
    read -p "要覆蓋現有設定嗎？ (y/N): " -n 1 -r
    echo
    if [[ ! $REPLY =~ ^[Yy]$ ]]; then
        echo "保留現有前端環境檔案"
    else
        cp "$FRONTEND_EXAMPLE" "$FRONTEND_ENV"
        echo -e "${GREEN}✓ 已從範例檔案建立新的前端環境檔案${NC}"
    fi
else
    cp "$FRONTEND_EXAMPLE" "$FRONTEND_ENV"
    echo -e "${GREEN}✓ 已建立前端環境檔案${NC}"
fi

# 步驟 3: 安全性設定
echo ""
echo -e "${YELLOW}步驟 3: 安全性設定${NC}"
echo "------------------------"

read -p "要自動產生安全的密碼嗎？ (推薦用於生產環境) (Y/n): " -n 1 -r
echo
if [[ ! $REPLY =~ ^[Nn]$ ]]; then
    # 產生隨機密碼
    DB_PASSWORD=$(generate_password)
    ROOT_PASSWORD=$(generate_password)
    JWT_SECRET=$(generate_jwt_secret)
    
    # 更新 .env 檔案（使用更安全的方式處理特殊字元）
    # 先備份
    cp .env .env.bak
    
    # 使用 awk 來更新，避免 sed 的特殊字元問題
    awk -v db_pass="$DB_PASSWORD" -v root_pass="$ROOT_PASSWORD" -v jwt="$JWT_SECRET" '
    /^MARIADB_PASSWORD=/ {print "MARIADB_PASSWORD=" db_pass; next}
    /^MARIADB_ROOT_PASSWORD=/ {print "MARIADB_ROOT_PASSWORD=" root_pass; next}
    /^JWT_SECRET=/ {print "JWT_SECRET=" jwt; next}
    {print}
    ' .env.bak > .env
    
    rm .env.bak
    
    echo -e "${GREEN}✓ 已產生並設定安全密碼${NC}"
    echo -e "${YELLOW}  請妥善保存這些密碼！${NC}"
else
    echo -e "${YELLOW}⚠ 使用預設密碼（請記得之後更改）${NC}"
fi

# 步驟 4: Google OAuth 設定（選用）
echo ""
echo -e "${YELLOW}步驟 4: Google OAuth 設定（選用）${NC}"
echo "------------------------"

read -p "要設定 Google OAuth 登入嗎？ (y/N): " -n 1 -r
echo
if [[ $REPLY =~ ^[Yy]$ ]]; then
    echo ""
    echo "請前往 Google Cloud Console 取得 Client ID："
    echo -e "${BLUE}https://console.cloud.google.com/apis/credentials${NC}"
    echo ""
    echo "設定步驟："
    echo "1. 建立或選擇專案"
    echo "2. 建立 OAuth 2.0 用戶端 ID"
    echo "3. 授權的 JavaScript 來源加入: http://localhost:5173"
    echo "4. 複製 Client ID"
    echo ""
    read -p "請輸入你的 Google Client ID: " GOOGLE_CLIENT_ID
    
    if [ ! -z "$GOOGLE_CLIENT_ID" ]; then
        # 更新兩個環境檔案（使用更安全的方式）
        # 更新 .env
        cp .env .env.bak
        awk -v client_id="$GOOGLE_CLIENT_ID" '
        /^GOOGLE_CLIENT_ID=/ {print "GOOGLE_CLIENT_ID=" client_id; next}
        {print}
        ' .env.bak > .env
        rm .env.bak
        
        # 更新前端環境檔案
        cp "$FRONTEND_ENV" "$FRONTEND_ENV.bak"
        awk -v client_id="$GOOGLE_CLIENT_ID" '
        /^VITE_GOOGLE_CLIENT_ID=/ {print "VITE_GOOGLE_CLIENT_ID=" client_id; next}
        {print}
        ' "$FRONTEND_ENV.bak" > "$FRONTEND_ENV"
        rm "$FRONTEND_ENV.bak"
        
        echo -e "${GREEN}✓ 已設定 Google OAuth${NC}"
    else
        echo -e "${YELLOW}跳過 Google OAuth 設定${NC}"
    fi
else
    echo -e "${YELLOW}跳過 Google OAuth 設定（可之後再設定）${NC}"
fi

# 步驟 5: Docker 檢查
echo ""
echo -e "${YELLOW}步驟 5: 檢查 Docker 環境${NC}"
echo "------------------------"

if command -v docker &> /dev/null && command -v docker-compose &> /dev/null; then
    echo -e "${GREEN}✓ Docker 和 Docker Compose 已安裝${NC}"
    
    echo ""
    read -p "要現在啟動服務嗎？ (Y/n): " -n 1 -r
    echo
    if [[ ! $REPLY =~ ^[Nn]$ ]]; then
        echo -e "${BLUE}正在啟動服務...${NC}"
        docker-compose up -d
        
        echo ""
        echo -e "${GREEN}=====================================${NC}"
        echo -e "${GREEN}  設定完成！${NC}"
        echo -e "${GREEN}=====================================${NC}"
        echo ""
        echo "服務正在啟動中，請稍候約 30 秒..."
        echo ""
        echo "可以訪問："
        echo -e "  ${BLUE}前端: http://localhost:5173${NC}"
        echo -e "  ${BLUE}API: http://localhost:5246${NC}"
        echo -e "  ${BLUE}API 文檔: http://localhost:5246/swagger${NC}"
        echo ""
        echo "查看服務狀態："
        echo "  docker-compose ps"
        echo ""
        echo "查看日誌："
        echo "  docker-compose logs -f"
    else
        echo ""
        echo -e "${GREEN}=====================================${NC}"
        echo -e "${GREEN}  設定完成！${NC}"
        echo -e "${GREEN}=====================================${NC}"
        echo ""
        echo "環境已設定完成。要啟動服務，請執行："
        echo -e "  ${BLUE}docker-compose up -d${NC}"
    fi
else
    echo -e "${RED}⚠ Docker 或 Docker Compose 未安裝${NC}"
    echo ""
    echo "請先安裝 Docker："
    echo -e "  ${BLUE}https://docs.docker.com/get-docker/${NC}"
    echo ""
    echo "安裝完成後，執行："
    echo -e "  ${BLUE}docker-compose up -d${NC}"
fi

echo ""
echo -e "${YELLOW}提示：${NC}"
echo "- 查看更多設定選項，請編輯 .env 檔案"
echo "- 詳細文檔請參考 README.md"
echo "- 如需協助，請查看 https://github.com/haha505254/badminton-forum/issues"