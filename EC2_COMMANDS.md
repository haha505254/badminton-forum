# EC2 實例管理指令

## 當前配置
- **Instance ID**: i-06dc8b0e59e1a523f
- **Instance Type**: c5.large
- **Elastic IP**: 15.168.229.18
- **Region**: ap-northeast-3 (Osaka)
- **Volume**: 30GB (vol-00f7b8aa7fb4d1ebe)

## 🚀 自動化部署 (使用 deploy.sh)

### deploy.sh 腳本功能
部署腳本會自動執行以下操作：
- ✅ 檢查環境設定 (.env)
- ✅ 從 Git 拉取最新代碼
- ✅ 停止並清理舊容器
- ✅ 建立並啟動所有服務
- ✅ 執行健康檢查
- ✅ 顯示服務 URL

### 一鍵啟動並部署
```bash
# 啟動 EC2 + 自動部署（推薦）
aws ec2 start-instances --region ap-northeast-3 --instance-ids i-06dc8b0e59e1a523f && \
aws ec2 wait instance-running --region ap-northeast-3 --instance-ids i-06dc8b0e59e1a523f && \
sleep 30 && \
ssh -i ~/.ssh/badminton-forum-osaka-key.pem ubuntu@15.168.229.18 "cd /home/ubuntu/badminton-forum && ./deploy.sh"
```

### 更新並重新部署
```bash
# 拉取最新代碼並重新部署
ssh -i ~/.ssh/badminton-forum-osaka-key.pem ubuntu@15.168.229.18 "cd /home/ubuntu/badminton-forum && git pull && ./deploy.sh"

# 強制重建所有映像檔（當有問題時使用）
ssh -i ~/.ssh/badminton-forum-osaka-key.pem ubuntu@15.168.229.18 "cd /home/ubuntu/badminton-forum && ./deploy.sh --force"
```

## 🟢 啟動實例（需要使用時）

### 1. 啟動 EC2 實例
```bash
aws ec2 start-instances --region ap-northeast-3 --instance-ids i-06dc8b0e59e1a523f

# 等待實例完全啟動
aws ec2 wait instance-running --region ap-northeast-3 --instance-ids i-06dc8b0e59e1a523f

# 確認狀態
aws ec2 describe-instances --region ap-northeast-3 --instance-ids i-06dc8b0e59e1a523f --query 'Reservations[0].Instances[0].State.Name' --output text
```

### 2. SSH 連線並啟動服務
```bash
# 方法 A: 使用 deploy.sh 自動部署（推薦）
ssh -i ~/.ssh/badminton-forum-osaka-key.pem ubuntu@15.168.229.18 "cd /home/ubuntu/badminton-forum && ./deploy.sh"

# 方法 B: 手動啟動（快速但不更新代碼）
ssh -i ~/.ssh/badminton-forum-osaka-key.pem ubuntu@15.168.229.18 "cd /home/ubuntu/badminton-forum && docker-compose -f docker-compose.prod.yml up -d"

# 檢查服務狀態
ssh -i ~/.ssh/badminton-forum-osaka-key.pem ubuntu@15.168.229.18 "docker ps"
```

### 3. 應用程式 URLs
- **主論壇**: http://15.168.229.18:5173
- **管理後台**: http://15.168.229.18:5174
- **API Swagger**: http://15.168.229.18:5246/swagger
- **Adminer**: http://15.168.229.18:8080

## 🔴 停止實例（節省成本）

```bash
# 停止 EC2 實例
aws ec2 stop-instances --region ap-northeast-3 --instance-ids i-06dc8b0e59e1a523f

# 等待實例完全停止
aws ec2 wait instance-stopped --region ap-northeast-3 --instance-ids i-06dc8b0e59e1a523f

# 確認狀態
aws ec2 describe-instances --region ap-northeast-3 --instance-ids i-06dc8b0e59e1a523f --query 'Reservations[0].Instances[0].State.Name' --output text
```

## 📊 成本分析

### 實例運行時
- EC2 (c5.large): $69/月
- EBS (30GB): $3/月
- Elastic IP: $0/月（使用中免費）
- **總計**: $72/月

### 實例停止時
- EC2: $0/月
- EBS (30GB): $3/月
- Elastic IP: $3.60/月（未使用費用）
- **總計**: $6.60/月

**每月節省**: $65.40（91%）

## 🔧 其他常用指令

### 檢查實例狀態
```bash
aws ec2 describe-instances --region ap-northeast-3 --instance-ids i-06dc8b0e59e1a523f --query 'Reservations[0].Instances[0].[State.Name,PublicIpAddress]' --output table
```

### 查看所有資源
```bash
# 查看實例
aws ec2 describe-instances --region ap-northeast-3 --output table

# 查看 Volumes
aws ec2 describe-volumes --region ap-northeast-3 --output table

# 查看 Snapshots
aws ec2 describe-snapshots --region ap-northeast-3 --owner-ids self --output table
```

## 🔧 故障排除

### 查看服務日誌
```bash
# 查看所有服務日誌
ssh -i ~/.ssh/badminton-forum-osaka-key.pem ubuntu@15.168.229.18 "cd /home/ubuntu/badminton-forum && docker-compose -f docker-compose.prod.yml logs -f"

# 查看特定服務日誌
ssh -i ~/.ssh/badminton-forum-osaka-key.pem ubuntu@15.168.229.18 "cd /home/ubuntu/badminton-forum && docker-compose -f docker-compose.prod.yml logs -f api"
ssh -i ~/.ssh/badminton-forum-osaka-key.pem ubuntu@15.168.229.18 "cd /home/ubuntu/badminton-forum && docker-compose -f docker-compose.prod.yml logs -f web"
ssh -i ~/.ssh/badminton-forum-osaka-key.pem ubuntu@15.168.229.18 "cd /home/ubuntu/badminton-forum && docker-compose -f docker-compose.prod.yml logs -f admin"
```

### 重啟服務
```bash
# 重啟特定服務
ssh -i ~/.ssh/badminton-forum-osaka-key.pem ubuntu@15.168.229.18 "cd /home/ubuntu/badminton-forum && docker-compose -f docker-compose.prod.yml restart api"

# 停止並重新啟動所有服務
ssh -i ~/.ssh/badminton-forum-osaka-key.pem ubuntu@15.168.229.18 "cd /home/ubuntu/badminton-forum && docker-compose -f docker-compose.prod.yml down && docker-compose -f docker-compose.prod.yml up -d"

# 清理並重建（最徹底的解決方案）
ssh -i ~/.ssh/badminton-forum-osaka-key.pem ubuntu@15.168.229.18 "cd /home/ubuntu/badminton-forum && ./deploy.sh --force"
```

### 資料庫操作
```bash
# 進入資料庫 shell
ssh -i ~/.ssh/badminton-forum-osaka-key.pem ubuntu@15.168.229.18 "docker exec -it badminton-forum-db-1 mysql -u badmintonuser -pBadmintonPass123 badmintonforumdb"

# 備份資料庫
ssh -i ~/.ssh/badminton-forum-osaka-key.pem ubuntu@15.168.229.18 "docker exec badminton-forum-db-1 mysqldump -u badmintonuser -pBadmintonPass123 badmintonforumdb > ~/backup-$(date +%Y%m%d).sql"
```

## 📝 注意事項

1. **Elastic IP 保證 IP 不變**：即使停止/啟動，IP 始終是 15.168.229.18
2. **資料完全保留**：停止實例不會遺失任何資料
3. **啟動時間**：約 1-2 分鐘實例即可運行
4. **自動部署**：使用 `deploy.sh` 會自動更新代碼、建立容器並執行健康檢查
5. **預設密碼警告**：如果看到安全警告，請修改 .env 中的預設密碼

## 🚀 快速啟動腳本

可以建立一個 bash 腳本 `start-ec2.sh`:

```bash
#!/bin/bash
echo "Starting EC2 instance..."
aws ec2 start-instances --region ap-northeast-3 --instance-ids i-06dc8b0e59e1a523f

echo "Waiting for instance to be running..."
aws ec2 wait instance-running --region ap-northeast-3 --instance-ids i-06dc8b0e59e1a523f

echo "Instance is running! Waiting 30 seconds for SSH to be ready..."
sleep 30

echo "Deploying services with health checks..."
ssh -i ~/.ssh/badminton-forum-osaka-key.pem ubuntu@15.168.229.18 "cd /home/ubuntu/badminton-forum && ./deploy.sh"

echo "✅ Deployment complete!"
echo "Forum: http://15.168.229.18:5173"
echo "Admin: http://15.168.229.18:5174"
echo "API: http://15.168.229.18:5246/swagger"
```

### 停止腳本 `stop-ec2.sh`:

```bash
#!/bin/bash
echo "Stopping EC2 instance to save costs..."
aws ec2 stop-instances --region ap-northeast-3 --instance-ids i-06dc8b0e59e1a523f

echo "Waiting for instance to stop..."
aws ec2 wait instance-stopped --region ap-northeast-3 --instance-ids i-06dc8b0e59e1a523f

echo "✅ Instance stopped!"
echo "Monthly cost reduced from \$72 to \$6.60"
```

記得執行 `chmod +x start-ec2.sh stop-ec2.sh` 讓腳本可執行。