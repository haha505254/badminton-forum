# 個人資料頁面 URL 修復測試

## 變更內容

已將個人資料頁面的 URL 從使用 email/username 改為使用數字 ID：

- ❌ 舊格式：`/profile/bagaty@gmail.com`
- ✅ 新格式：`/profile/123`

## 測試步驟

### 1. 重啟開發伺服器

```bash
# 如果正在執行，先停止 (Ctrl+C)
cd badminton-forum-vue
npm run dev
```

### 2. 測試新路由

1. **登入系統**
   - 訪問 http://localhost:5173/login
   - 使用您的帳號登入

2. **點擊導航欄的使用者名稱**
   - 應該跳轉到類似 `/profile/1` 的頁面（數字是您的使用者 ID）

3. **重新整理頁面**
   - 按 F5 重新整理
   - 頁面應該正常顯示，不會出現 404 錯誤

4. **直接訪問 URL**
   - 在瀏覽器輸入：http://localhost:5173/profile/1
   - 應該能正常顯示個人資料頁面

### 3. Docker 環境測試

如果使用 Docker Compose：

```bash
docker-compose down
docker-compose up --build
```

然後重複上述測試步驟。

## 修改的檔案

1. **後端**
   - `ProfileController.cs` - 新增 `/api/profile/by-id/{id}` 端點

2. **前端**
   - `profile.js` - 新增 `getProfileById()` 方法
   - `router/index.js` - 修改路由接受數字 ID
   - `ProfileView.vue` - 使用 ID 查詢使用者
   - `App.vue` - 導航連結使用使用者 ID

## 優點

1. **URL 更簡潔**：`/profile/1` 比 `/profile/user@example.com` 簡潔
2. **避免特殊字符問題**：不再有 `@` 符號造成的問題
3. **更安全**：不暴露使用者的 email
4. **重新整理正常**：解決了 404 錯誤

## 注意事項

- 舊的 username 端點仍然保留，確保向後相容
- 如果您有書籤指向舊的 URL，需要更新

## 未來改進

可以考慮為作者名稱加上可點擊的連結：
- 在文章列表顯示作者時
- 在文章詳情頁顯示作者時
- 在回覆顯示作者時

這樣使用者可以點擊作者名稱查看其個人資料。