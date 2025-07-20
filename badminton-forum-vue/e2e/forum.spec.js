import { test, expect } from '@playwright/test';
import { loginAsTestUser } from './helpers/auth-helper.js';

test.describe('論壇功能', () => {
  test('應該顯示所有論壇版塊', async ({ page }) => {
    await page.goto('/');
    await page.click('text=版塊');
    
    // 驗證四個預設版塊都存在
    await expect(page.locator('text=技術討論')).toBeVisible();
    await expect(page.locator('text=裝備推薦')).toBeVisible();
    await expect(page.locator('text=活動公告')).toBeVisible();
    await expect(page.locator('text=球友交流')).toBeVisible();
  });

  test('應該能瀏覽版塊文章', async ({ page }) => {
    await page.goto('/categories');
    
    // 點擊技術討論版塊
    await page.click('text=技術討論');
    
    // 驗證進入版塊頁面
    await expect(page).toHaveURL(/\/category\/\d+/);
    await expect(page.locator('h1:has-text("技術討論")')).toBeVisible();
  });

  test('應該能查看文章詳情', async ({ page }) => {
    // 先創建一篇測試文章
    const timestamp = Date.now();
    const testUser = await loginAsTestUser(page);
    
    // 發表文章
    await page.goto('/new-post');
    await page.selectOption('select', '1'); // 技術討論
    await page.fill('input[placeholder="請輸入文章標題"]', `測試文章 ${timestamp}`);
    await page.fill('textarea[placeholder="請輸入文章內容..."]', '這是測試文章的內容，用於測試查看文章詳情功能。');
    await page.click('button:has-text("發表文章")');
    
    // 等待重定向到文章頁面
    await page.waitForURL(/\/post\/\d+/);
    const postUrl = page.url();
    
    // 回到版塊列表
    await page.goto('/category/1');
    
    // 點擊剛創建的文章
    await page.click(`text=測試文章 ${timestamp}`);
    
    // 驗證進入文章頁面
    await expect(page).toHaveURL(postUrl);
    await expect(page.locator('.post-content')).toBeVisible();
    await expect(page.locator('text=這是測試文章的內容')).toBeVisible();
  });
});

test.describe('需要登入的功能', () => {
  let testUser;
  
  test.beforeEach(async ({ page }) => {
    // 使用真實的登入流程
    testUser = await loginAsTestUser(page);
  });

  test('應該能發表新文章', async ({ page }) => {
    await page.goto('/');
    await page.click('text=發表文章');
    
    // 驗證進入發文頁面
    await expect(page).toHaveURL('/new-post');
    
    // 選擇版塊
    await page.selectOption('select', '1'); // 技術討論
    
    // 填寫文章內容
    await page.fill('input[placeholder="請輸入文章標題"]', '測試文章標題');
    await page.fill('textarea[placeholder="請輸入文章內容..."]', '這是測試文章的內容。');
    
    // 提交文章
    await page.click('button:has-text("發表文章")');
    
    // 驗證發表成功（應該重定向到文章頁面）
    await expect(page).toHaveURL(/\/post\/\d+/);
  });

  test('應該能回覆文章', async ({ page }) => {
    // 先建立一篇測試文章
    await page.goto('/new-post');
    await page.selectOption('select', '1'); // 技術討論
    await page.fill('input[placeholder="請輸入文章標題"]', '測試文章以供回覆');
    await page.fill('textarea[placeholder="請輸入文章內容..."]', '這是測試文章的內容。');
    await page.click('button:has-text("發表文章")');
    
    // 等待重定向到文章頁面
    await page.waitForURL(/\/post\/\d+/);
    
    // 填寫回覆
    await page.fill('textarea[placeholder="寫下您的回覆..."]', '這是一個測試回覆');
    
    // 發表回覆
    await page.click('button:has-text("發表")');
    
    // 等待回覆出現
    await page.waitForSelector('text=這是一個測試回覆');
    await expect(page.locator('text=這是一個測試回覆')).toBeVisible();
  });
});