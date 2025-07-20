# Badminton Forum 專案指導原則

## 專案概述
這是一個羽毛球討論論壇，使用 .NET 8 Web API 作為後端，Vue 3 作為前端的全端網頁應用程式。

## 技術決策
1. **後端使用 .NET SDK 標準結構**：專案應遵循 `dotnet new webapi` 產生的標準結構
2. **前後端分離**：API 和前端是獨立的專案，透過 RESTful API 通訊
3. **資料庫優先**：使用 Entity Framework Core 的 Code First 方式管理資料庫結構

## 開發原則

### 後端開發
- 使用 DTOs 進行資料傳輸，避免直接暴露 Entity 模型
- 所有 API 端點都應有適當的錯誤處理和回傳訊息
- 使用 JWT 進行身份驗證
- API 文檔使用 Swagger 自動產生
- 遵循 RESTful API 設計原則

### 前端開發
- 使用 Vue 3 Composition API 與 `<script setup>` 語法
- 狀態管理使用 Pinia
- API 呼叫集中在 `src/api` 目錄管理
- 使用 Vue Router 進行路由管理
- 支援響應式設計

### 資料庫
- 使用 PostgreSQL 作為主要資料庫
- Migration 檔案應保留在版本控制中
- 種子資料使用靜態值，避免動態日期

## 安全考量
- 密碼必須使用 BCrypt 加密
- JWT Token 有效期限設定為 7 天
- API 需要適當的 CORS 設定
- 敏感資訊不應出現在程式碼中

## 程式碼風格
### C# (.NET)
- 類別和方法名稱使用 PascalCase
- 參數和變數使用 camelCase
- 私有欄位使用 _camelCase
- 使用 async/await 處理非同步操作

### JavaScript/Vue
- 元件名稱使用 PascalCase
- 變數和函數使用 camelCase
- 使用 ES6+ 語法
- 偏好使用 const，必要時才用 let

## 版本控制
- 使用有意義的 commit 訊息
- 功能開發應在獨立分支進行
- main 分支應保持穩定可部署狀態