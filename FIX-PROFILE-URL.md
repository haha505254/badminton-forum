# 修復個人資料頁面 URL 問題

## 問題描述
當使用 email 作為 URL 參數（如 `/profile/bagaty@gmail.com`）時，重新整理頁面會出現 404 錯誤。

## 問題原因
1. Vite 開發伺服器沒有正確處理 SPA 路由
2. URL 中的 `@` 符號可能被誤解析

## 解決方案

### 方案 1：更新 Vite 設定（已完成）
在 `vite.config.js` 中加入 `historyApiFallback: true`

### 方案 2：使用使用者 ID 而非 email（建議）
修改路由使用數字 ID：
- 從：`/profile/bagaty@gmail.com`
- 到：`/profile/123`

### 方案 3：URL 編碼
在傳遞 email 時進行編碼：
```javascript
// 編碼
const encodedEmail = encodeURIComponent('bagaty@gmail.com')
router.push(`/profile/${encodedEmail}`)

// 解碼
const email = decodeURIComponent(route.params.username)
```

## 立即修復步驟

1. **重啟 Vite 開發伺服器**
   ```bash
   # 停止當前伺服器 (Ctrl+C)
   # 重新啟動
   npm run dev
   ```

2. **測試**
   - 訪問：http://localhost:5173/profile/bagaty@gmail.com
   - 重新整理頁面
   - 應該能正常顯示

## Docker 環境修復

如果使用 Docker，需要確保 nginx 設定正確：

```nginx
location / {
    try_files $uri $uri/ /index.html;
}
```

這個設定已經在 `nginx.conf` 中包含了。

## 長期建議

考慮修改系統使用使用者 ID 而非 email 作為 URL 參數，這樣更安全且避免特殊字符問題：

1. 修改後端 API 支援透過 ID 查詢使用者
2. 修改前端路由使用 ID
3. 在需要顯示 email 的地方從 API 獲取

這樣 URL 會是：`/profile/1` 而非 `/profile/bagaty@gmail.com`