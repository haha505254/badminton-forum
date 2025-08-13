# 開發階段
FROM node:18-alpine AS development

WORKDIR /app

# 安裝 pnpm (更快的套件管理器，選用)
RUN corepack enable && corepack prepare pnpm@latest --activate

# 重要：將 /app 目錄的擁有者改為 node
RUN chown -R node:node /app

# 複製 package.json 和 lock 檔案，並設定權限給 node 使用者
COPY --chown=node:node package*.json ./
COPY --chown=node:node pnpm-lock.yaml* ./

# 切換到 node 使用者（UID 1000）
USER node

# 安裝相依套件（包含開發依賴）
RUN if [ -f pnpm-lock.yaml ]; then pnpm install --frozen-lockfile; \
    elif [ -f package-lock.json ]; then npm ci; \
    else npm install; fi

# 複製所有檔案並設定權限
COPY --chown=node:node . .

# 開發模式
EXPOSE 5173
CMD ["npm", "run", "dev", "--", "--host", "0.0.0.0"]

# 建置階段
FROM node:18-alpine AS build

WORKDIR /app

# 複製 package.json 和 lock 檔案
COPY package*.json ./
COPY pnpm-lock.yaml* ./

# 安裝相依套件
RUN if [ -f pnpm-lock.yaml ]; then \
      corepack enable && corepack prepare pnpm@latest --activate && pnpm install --frozen-lockfile; \
    elif [ -f package-lock.json ]; then npm ci; \
    else npm install; fi

# 複製所有檔案
COPY . .

# 設定建置時的環境變數
ARG VITE_API_URL=http://localhost:5246/api
ENV VITE_API_URL=$VITE_API_URL

# 建置專案
RUN npm run build

# 生產階段 - 使用 nginx
FROM nginx:alpine AS production

# 複製建置結果
COPY --from=build /app/dist /usr/share/nginx/html

# 複製 nginx 設定
COPY nginx.conf /etc/nginx/conf.d/default.conf

# 健康檢查
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD wget --no-verbose --tries=1 --spider http://localhost || exit 1

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]