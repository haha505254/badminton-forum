# GitHub Actions Workflows

## 目前使用的 Workflows

### ci-cd.yml (主要 CI/CD Pipeline)
這是整合後的主要 CI/CD 流程，包含：
- **測試階段**：執行 API 測試、前端建置測試
- **建置階段**：建置 Docker 映像並推送到 AWS ECR（僅 main 分支）
- **部署階段**：自動部署到 EC2 生產環境（僅 main 分支）

觸發條件：
- Push 到 main 或 develop 分支
- Pull Request 到 main 分支
- 手動觸發 (workflow_dispatch)

### security.yml
定期執行的安全掃描，使用 CodeQL 分析程式碼漏洞。

## 已棄用的 Workflows（建議刪除）

以下檔案的功能已整合到 `ci-cd.yml` 中：

1. **ci.yml** - 原本的 CI pipeline，現已整合
2. **cd.yml** - 原本的 CD pipeline，現已整合  
3. **deploy-aws.yml** - 原本的 AWS 部署流程，現已整合

建議執行以下命令刪除舊檔案：
```bash
git rm .github/workflows/ci.yml
git rm .github/workflows/cd.yml
git rm .github/workflows/deploy-aws.yml
git commit -m "refactor(CI/CD): 整合多個 workflow 為單一 ci-cd.yml"
```

## 環境變數和 Secrets

需要在 GitHub Repository Settings 中設定以下 Secrets：

### AWS 相關
- `AWS_ACCESS_KEY_ID`
- `AWS_SECRET_ACCESS_KEY`

### EC2 部署相關
- `EC2_HOST` - EC2 實例的 IP 或域名
- `EC2_USER` - SSH 使用者名稱（通常是 ubuntu）
- `EC2_SSH_KEY` - SSH 私鑰內容