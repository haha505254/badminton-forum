# 多階段建置 - 開發階段
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS development

WORKDIR /app

# 安裝 Entity Framework Core 工具
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

# 安裝 postgresql-client 用於健康檢查
RUN apt-get update && apt-get install -y postgresql-client && rm -rf /var/lib/apt/lists/*

# 複製專案檔案
COPY *.csproj ./
RUN dotnet restore

# 複製所有檔案
COPY . .

# 複製並設定啟動腳本
COPY docker-entrypoint.sh ./
RUN chmod +x docker-entrypoint.sh

# 開發環境使用 dotnet watch
EXPOSE 5246
ENTRYPOINT ["./docker-entrypoint.sh"]

# 多階段建置 - 建置階段
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
COPY *.csproj ./
RUN dotnet restore

COPY . .
RUN dotnet build -c Release -o /app/build

# 多階段建置 - 發布階段
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

# 多階段建置 - 生產階段
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS production

WORKDIR /app
EXPOSE 5246

# 安裝 postgresql-client 用於健康檢查和遷移
RUN apt-get update && apt-get install -y postgresql-client && rm -rf /var/lib/apt/lists/*

# 只複製發布後的檔案
COPY --from=publish /app/publish .

# 複製 entrypoint script
COPY docker-entrypoint.sh ./
RUN chmod +x docker-entrypoint.sh

# 使用 entrypoint script 來執行應用程式
ENTRYPOINT ["./docker-entrypoint.sh"]