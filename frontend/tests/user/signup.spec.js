import { test, expect } from "@playwright/test";

test.describe("SignUp Page", () => {
  test.beforeEach(async ({ page }) => {
    await page.goto("http://localhost:5173/signup"); // adjust path if necessary
  });

  test("renders all fields and signup button", async ({ page }) => {
    await expect(page.getByText("اسم العائلة")).toBeVisible();
    await expect(page.getByText("الاسم الاول")).toBeVisible();
    await expect(page.getByText("رقم الهاتف")).toBeVisible();
    await expect(page.getByText("البريد الإلكتروني")).toBeVisible();
    await expect(page.getByText("كلمة المرور")).toBeVisible();
    await expect(page.getByText("تأكيد كلمة السر")).toBeVisible();
    await expect(
      page.getByRole("button", { name: "إنشاء حساب" })
    ).toBeVisible();
  });

  test("shows required field validation errors", async ({ page }) => {
    await page.getByRole("button", { name: "إنشاء حساب" }).click();

    await expect(page.getByText("يرجى إدخال اسم العائلة")).toBeVisible();
    await expect(page.getByText("يرجى إدخال الاسم الاول")).toBeVisible();
    await expect(page.getByText("يرجى إدخال رقم الهاتف")).toBeVisible();
    await expect(page.getByText("يرجى إدخال البريد الإلكتروني")).toBeVisible();
    await expect(page.getByText("Password is required")).toBeVisible();
    await expect(page.getByText("هذه الخانة مطلوبة")).toBeVisible();
  });

  test("submits form with valid data", async ({ page }) => {
    // Mock the API response for the signup API
    await page.route("**/api/Account/Register", async (route) => {
      await route.fulfill({
        status: 200,
        contentType: "application/json",
        body: JSON.stringify({
          email: "testuser@example.com",
          roles: ["User"],
        }),
      });
    });

    // Navigate to the SignUp page

    // Fill in form fields
    await page.fill('input[name="lastName"]', "Yahia"); // Make sure the 'name' attribute is correct in the HTML
    await page.fill('input[name="firstName"]', "Ahmed");
    await page.fill('input[name="phoneNumber"]', "0123456789");
    await page.fill('input[name="email"]', "testuser@example.com");
    await page.fill('input[name="password"]', "Password1!");
    await page.fill('input[name="confirmPassword"]', "Password1!");

    // Click on the "Create Account" button

    page.getByRole("button", { name: "إنشاء حساب" }).click();
    // Check if the response was successful

    // After successful signup, the page should redirect to /verify-email
    await expect(page).toHaveURL("http://localhost:5173/verify-email");
  });

  test("shows password mismatch error", async ({ page }) => {
    await page.fill('input[name="password"]', "testpassword");
    await page.fill('input[name="confirmPassword"]', "test1password");

    await page.getByRole("button", { name: "إنشاء حساب" }).click();

    await expect(page.getByText("كلمة السر غير متطابقة")).toBeVisible();
  });
});
