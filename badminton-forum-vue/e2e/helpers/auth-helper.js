export async function loginAsTestUser(page) {
  // 創建測試用戶
  const timestamp = Date.now();
  const username = `testuser${timestamp}`;
  const email = `test${timestamp}@example.com`;
  const REMOVED = 'REMOVED';

  // 先註冊新用戶
  await page.goto('/register');
  await page.fill('input[placeholder="3-50個字符"]', username);
  await page.fill('input[placeholder="example@email.com"]', email);
  await page.fill('input[placeholder="至少6個字符"]', REMOVED);
  await page.fill('input[placeholder="請再次輸入密碼"]', REMOVED);
  await page.click('button:has-text("註冊")');

  // 等待註冊成功並重定向
  await page.waitForURL('/');
  
  // 驗證登入狀態
  const token = await page.evaluate(() => localStorage.getItem('token'));
  const user = await page.evaluate(() => localStorage.getItem('user'));
  
  return { token, user: JSON.parse(user), username, email, REMOVED };
}

export async function logout(page) {
  await page.click('text=登出');
  await page.waitForURL('/');
}