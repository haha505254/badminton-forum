# 開發階段
FROM node:18-alpine AS development

WORKDIR /app

# 設定權限
RUN chown -R node:node /app

# 複製套件檔案
COPY --chown=node:node package*.json ./

# 切換使用者
USER node

# 安裝依賴
RUN npm ci || npm install

# 複製所有檔案
COPY --chown=node:node . .

# 開發模式
EXPOSE 5174
CMD ["npm", "run", "dev", "--", "--host", "0.0.0.0", "--port", "5174"]

# 建置階段
FROM node:18-alpine AS build

WORKDIR /app

COPY package*.json ./
RUN npm ci || npm install

COPY . .

# 建置參數
ARG VITE_API_URL=http://localhost:5246/api
ARG VITE_APP_NAME="羽球論壇管理後台"
ENV VITE_API_URL=$VITE_API_URL
ENV VITE_APP_NAME=$VITE_APP_NAME

RUN npm run build

# 生產階段
FROM nginx:alpine AS production

COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf

HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD wget --no-verbose --tries=1 --spider http://localhost || exit 1

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]