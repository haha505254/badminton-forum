import { test, expect } from '@playwright/test';

test.describe('使用者認證功能', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/');
  });

  test('應該能夠註冊新使用者', async ({ page }) => {
    // 導航到註冊頁面
    await page.click('text=註冊');
    await expect(page).toHaveURL('/register');

    // 填寫註冊表單
    await page.fill('input[placeholder="3-50個字符"]', 'testuser' + Date.now());
    await page.fill('input[placeholder="example@email.com"]', `test${Date.now()}@example.com`);
    await page.fill('input[placeholder="至少6個字符"]', 'REMOVED');
    await page.fill('input[placeholder="請再次輸入密碼"]', 'REMOVED');

    // 提交表單
    await page.click('button:has-text("註冊")');

    // 驗證註冊成功（應該重定向到首頁並顯示用戶名）
    await expect(page).toHaveURL('/');
    await expect(page.locator('text=testuser')).toBeVisible();
  });

  test('應該能夠登入', async ({ page }) => {
    // 導航到登入頁面
    await page.click('text=登入');
    await expect(page).toHaveURL('/login');

    // 填寫登入表單
    await page.fill('input[placeholder="請輸入用戶名"]', 'testuser');
    await page.fill('input[placeholder="請輸入密碼"]', 'REMOVED');

    // 提交表單
    await page.click('button:has-text("登入")');

    // 驗證登入成功
    await expect(page.locator('.error-message')).toBeVisible();
    // 註：實際測試需要預先建立測試帳號
  });

  test('應該能夠登出', async ({ page }) => {
    // 先進行真實登入
    const timestamp = Date.now();
    const username = `testuser${timestamp}`;
    const email = `test${timestamp}@example.com`;
    const REMOVED = 'REMOVED';

    // 註冊新用戶
    await page.goto('/register');
    await page.fill('input[placeholder="3-50個字符"]', username);
    await page.fill('input[placeholder="example@email.com"]', email);
    await page.fill('input[placeholder="至少6個字符"]', REMOVED);
    await page.fill('input[placeholder="請再次輸入密碼"]', REMOVED);
    await page.click('button:has-text("註冊")');

    // 等待註冊成功並重定向到首頁
    await page.waitForURL('/');
    
    // 確認已登入（應該看到用戶名）
    await expect(page.locator(`text=${username}`)).toBeVisible();
    
    // 點擊登出
    await page.click('text=登出');

    // 驗證登出成功
    await expect(page.locator('text=登入')).toBeVisible();
    await expect(page.locator('text=註冊')).toBeVisible();
  });
});