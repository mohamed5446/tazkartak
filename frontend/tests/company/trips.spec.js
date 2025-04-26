import { test, expect } from "@playwright/test";

test.describe("CompanyTrips page", () => {
  test.beforeEach(async ({ page }) => {
    // Mock login
    await page.route("**/api/Account/Login", async (route) => {
      await route.fulfill({
        status: 200,
        contentType: "application/json",
        body: JSON.stringify({
          email: "admin@example.com",
          roles: ["Company"],
          token: "fake-jwt-token",
          id: 123,
          isEmailConfirmed: true,
        }),
      });
    });

    // Simulate login
    await page.goto("http://localhost:5173/login");
    await page.fill('input[name="email"]', "admin@example.com");
    await page.fill('input[name="password"]', "password123");
    await page.click('button[type="submit"]');
    await page.waitForURL("http://localhost:5173/company/profile");
  });

  test('should show "no trips" message when there are no trips', async ({
    page,
  }) => {
    await page.route("**/api/123/Trips", (route) =>
      route.fulfill({ status: 200, body: JSON.stringify([]) })
    );

    await page.goto("http://localhost:5173/company/profile/trips");

    await expect(page.getByText("لا توجد رحلات")).toBeVisible();
  });

  test("should display a list of trips when data exists", async ({ page }) => {
    await page.route("**/api/123/Trips", (route) =>
      route.fulfill({
        status: 200,
        body: JSON.stringify([
          {
            tripId: 1,
            companyName: "شركة الاختبار",
            price: 150,
            from: "القاهرة",
            to: "الإسكندرية",
            departureDay: "الخميس",
            departureDate: "2025-05-01",
            arrivalDay: "الخميس",
            arrivalTime: "10:00 ص",
            location: "محطة مصر",
          },
        ]),
      })
    );

    await page.goto("http://localhost:5173/company/profile/trips");

    await expect(page.getByText("شركة الاختبار")).toBeVisible();
    await expect(page.getByText("القاهرة")).toBeVisible();
    await expect(page.getByText("الإسكندرية")).toBeVisible();
    await expect(page.getByRole("button", { name: "حذف" })).toBeVisible();
  });

  test('should open modal when clicking "اضافة رحلة"', async ({ page }) => {
    await page.route("**/api/123/Trips", (route) =>
      route.fulfill({ status: 200, body: JSON.stringify([]) })
    );

    await page.goto("http://localhost:5173/company/profile/trips");

    await page.getByRole("button", { name: "اضافة رحلة" }).click();

    // Wait for TripForm modal
    await expect(page.locator("form")).toBeVisible();
  });
  test("should add a new trip successfully", async ({ page }) => {
    await page.route("**/api/123/Trips", (route) =>
      route.fulfill({ status: 200, body: JSON.stringify([]) })
    );

    await page.route("**/api/Trips/123", async (route, request) => {
      // You can validate request body here if you want
      if (route.request().method() === "POST") {
        await route.fulfill({
          status: 200,
          contentType: "application/json",
          body: JSON.stringify({}),
        });
      } else {
        // Let other methods (GET, PUT, DELETE) continue normally

        route.continue();
      }
    });

    await page.goto("http://localhost:5173/company/profile/trips");

    // Open the modal
    await page.getByRole("button", { name: "اضافة رحلة" }).click();

    // Fill the form (TripForm fields)
    await page.locator('select[name="from"]').selectOption("القاهرة");
    await page.locator('select[name="to"]').selectOption("الجيزة");
    await page.getByLabel("فئة الرحلة").selectOption("VIP");
    await page.getByLabel("تاريخ الرحلة").fill("2025-06-01");
    await page.getByLabel("وقت المغادرة").fill("12:00:00");
    await page.getByLabel("مكان التحرك").fill("محطة أسيوط");
    await page.getByLabel("السعر").fill("250");

    // Submit the form
    await page.getByRole("button", { name: "إضافة الرحلة" }).click();
    // Expect the modal to close
    await expect(page.locator("form")).toBeHidden();

    // Optionally, check if page reloaded trips
    await expect(page.getByText("تم إضافة الرحلة بنجاح!")).toBeVisible();
  });

  //   test('should delete a trip and show success message', async ({ page }) => {
  //     await page.route('**/api/123/Trips', route =>
  //       route.fulfill({
  //         status: 200,
  //         body: JSON.stringify([
  //           {
  //             tripId: 1,
  //             companyName: 'شركة الاختبار',
  //             price: 150,
  //             from: 'القاهرة',
  //             to: 'الإسكندرية',
  //             departureDay: 'الخميس',
  //             departureDate: '2025-05-01',
  //             arrivalDay: 'الخميس',
  //             arrivalTime: '10:00 ص',
  //             location: 'محطة مصر',
  //           },
  //         ]),
  //       })
  //     );

  //     await page.route('**/api/Trips/1', route =>
  //       route.fulfill({ status: 200, body: JSON.stringify({}) })
  //     );

  //     await page.goto('http://localhost:5173/company/trips');

  //     await page.getByRole('button', { name: 'حذف' }).click();

  //     await expect(page.getByText('تم حذف الرحلة بنجاح!')).toBeVisible();
  //   });

  //   test('should show error toast when deleting fails', async ({ page }) => {
  //     await page.route('**/api/123/Trips', route =>
  //       route.fulfill({
  //         status: 200,
  //         body: JSON.stringify([
  //           {
  //             tripId: 1,
  //             companyName: 'شركة الاختبار',
  //             price: 150,
  //             from: 'القاهرة',
  //             to: 'الإسكندرية',
  //             departureDay: 'الخميس',
  //             departureDate: '2025-05-01',
  //             arrivalDay: 'الخميس',
  //             arrivalTime: '10:00 ص',
  //             location: 'محطة مصر',
  //           },
  //         ]),
  //       })
  //     );

  //     await page.route('**/api/Trips/1', route =>
  //       route.fulfill({
  //         status: 400,
  //         body: JSON.stringify({ message: 'فشل حذف الرحلة' }),
  //       })
  //     );

  //     await page.goto('http://localhost:5173/company/trips');

  //     await page.getByRole('button', { name: 'حذف' }).click();

  //     await expect(page.getByText('فشل حذف الرحلة')).toBeVisible();
  //   });
});
