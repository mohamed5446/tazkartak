import test, { expect } from "@playwright/test";
async function login(page) {
  await page.goto("http://localhost:5173/login");
  await page.fill('input[name="email"]', "user@example.com");
  await page.fill('input[name="password"]', "yourPassword");
  await page.click('button[type="submit"]');
}
test("Admin can add a new company", async ({ page }) => {
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

  await page.route("**/api/Companies", async (route) => {
    if (route.request().method() === "POST") {
      await route.fulfill({
        status: 200,
        contentType: "application/json",
        body: JSON.stringify({}),
      });
    } else {
      // Let other methods (GET, PUT, DELETE) continue normally
      await route.fulfill({
        status: 200,
        contentType: "application/json",
        body: JSON.stringify([
          {
            balance: 0,
            city: "string",
            email: "hotralidre@gufum.com",
            id: 3,
            logo: "https://res.cloudinary.com/dji7qd816/image/upload/v1744056447/hi6wf65dflrwy385tl3l.png",
            name: "buss",
            phone: "01111110000",
            street: "الرحبات",
          },
        ]),
      });
    }
  });
  // 1. Navigate to the users page
  await login(page);
  await page.goto("http://localhost:5173/admin/profile/companies"); // Change URL if needed

  // 2. Click the "Add User" button
  await page.getByRole("button", { name: "اضافة شركة" }).click();

  // 3. Fill the form
  await page.getByLabel("الاسم").fill("TesttName");
  await page.getByLabel("البريد الإلكتروني").fill("testuser@example.com");
  await page.getByLabel("رقم الهاتف").fill("01123456789");
  await page.getByLabel("كلمة المرور").fill("TestPassword123!");
  await page.getByLabel("تأكيد كلمة السر").fill("TestPassword123!");
  await page.getByLabel("الشارع").fill("teststreet");
  await page.getByLabel("المدينة").selectOption("بورسعيد");

  // 4. Submit the form
  await page.getByRole("button", { name: "اضافة" }).click();

  await expect(page.getByText("added successfully")).toBeVisible();
});
