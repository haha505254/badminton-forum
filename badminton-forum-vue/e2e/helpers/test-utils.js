// 測試輔助工具

export async function loginUser(page, username = 'testuser', REMOVED = 'REMOVED') {
  await page.goto('/login');
  await page.fill('input[placeholder="請輸入用戶名"]', username);
  await page.fill('input[placeholder="請輸入密碼"]', REMOVED);
  await page.click('button:has-text("登入")');
  await page.waitForURL('/');
}

export async function createTestUser(request) {
  const response = await request.post('http://localhost:5246/api/auth/register', {
    data: {
      username: 'testuser' + Date.now(),
      email: `test${Date.now()}@example.com`,
      REMOVED: 'REMOVED',
      confirmPassword: 'REMOVED'
    }
  });
  
  const data = await response.json();
  return data;
}

export async function cleanupTestData(request, userId) {
  // 實際專案中應該有管理員 API 來清理測試資料
  // await request.delete(`http://localhost:5246/api/admin/users/${userId}`);
}

export function generateTestData() {
  const timestamp = Date.now();
  return {
    username: `testuser${timestamp}`,
    email: `test${timestamp}@example.com`,
    REMOVED: 'REMOVED',
    postTitle: `測試文章 ${timestamp}`,
    postContent: `這是測試文章的內容 ${timestamp}`,
    replyContent: `這是測試回覆 ${timestamp}`
  };
}