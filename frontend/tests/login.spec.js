import { test, expect } from "@playwright/test";

test.describe("Login Page", () => {
  test.beforeEach(async ({ page }) => {
    await page.goto("http://localhost:5173/login");
  });

  test("should display required validation errors", async ({ page }) => {
    await page.click('button[type="submit"]');

    await expect(
      page.locator("text=يرجى إدخال البريد الالكترونى")
    ).toBeVisible();
    await expect(page.locator("text=يرجى إدخال كلمة المرور")).toBeVisible();
  });

  test("should toggle password visibility", async ({ page }) => {
    const passwordInput = page.locator('input[type="password"]');
    await expect(passwordInput).toBeVisible();

    await page.getByTestId("toggle-password").click(); // Eye Icon Button
    const passwordVisibleInput = page.locator('input[type="text"]');
    await expect(passwordVisibleInput).toBeVisible();
  });
  test("should show error message on invalid credentials", async ({ page }) => {
    await page.fill('input[name="email"]', "wrong@example.com");
    await page.fill('input[name="password"]', "wrongpassword");
    await page.click('button[type="submit"]');

    await expect(
      page.locator("text=Email or password is incorrect")
    ).toBeVisible(); // لازم تعدل النص حسب اللي بيطلع من API
  });
  test("admin can login successfully", async ({ page }) => {
    await page.route("**/api/Account/Login", async (route) => {
      await route.fulfill({
        status: 200,
        contentType: "application/json",
        body: JSON.stringify({
          email: "testuser@example.com",
          roles: ["Admin"],
          token: "fake-jwt-token",
          id: 1,
          isEmailConfirmed: true,
        }),
      });
    });

    // املأ البريد وكلمة السر
    await page.fill('input[name="email"]', "testuser@example.com");
    await page.fill('input[name="password"]', "testpassword");

    // دوس على زر تسجيل الدخول
    await page.click('button[type="submit"]');

    // توقع مثلاً إن المستخدم يتحول للصفحة الرئيسية
    await expect(page).toHaveURL("http://localhost:5173/admin/profile");
    await expect(page.locator("text=تعديل البيانات")).toBeVisible();
  });

  test("user can login successfully", async ({ page }) => {
    await page.route("**/api/Account/Login", async (route) => {
      await route.fulfill({
        status: 200,
        contentType: "application/json",
        body: JSON.stringify({
          email: "testuser@example.com",
          roles: ["User"],
          token: "fake-jwt-token",
          id: 1,
          isEmailConfirmed: true,
        }),
      });
    });
    await page.goto("http://localhost:5173/login"); // غيّر الرابط حسب مشروعك لو مختلف

    // املأ البريد وكلمة السر
    await page.fill('input[name="email"]', "testuser@example.com");
    await page.fill('input[name="password"]', "testpassword");

    // دوس على زر تسجيل الدخول
    await page.click('button[type="submit"]');

    // توقع مثلاً إن المستخدم يتحول للصفحة الرئيسية
    await expect(page).toHaveURL("http://localhost:5173/");
    await expect(page.locator("text=وسائل دفع مريحة و آمنة")).toBeVisible();
  });
});
