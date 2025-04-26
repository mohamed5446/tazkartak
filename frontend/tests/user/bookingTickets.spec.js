import { test, expect } from "@playwright/test";

// Mocked trip data
const mockTrips = [
  {
    id: "trip-001",
    from: "القاهرة",
    to: "الإسكندرية",
    date: "2025-05-01",
    departureTime: "10:00",
    availableSeats: 20,
    price: 100,
  },
];

// Login helper
async function login(page) {
  await page.goto("http://localhost:5173/login");
  await page.fill('input[name="email"]', "user@example.com");
  await page.fill('input[name="password"]', "yourPassword");
  await page.click('button[type="submit"]');
  await page.waitForURL("http://localhost:5173/");
}

test("user can log in, view mocked trips and book a seat", async ({ page }) => {
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

  // 1. Setup mock API
  await page.route("**/api/Trips/Search**", (route) => {
    route.fulfill({
      status: 200,
      contentType: "application/json",
      body: JSON.stringify(mockTrips),
    });
  });

  await page.route("**/api/Trips/1", async (route) => {
    const json = {
      id: 1,
      companyName: "Test Bus Co",
      price: 100,
      to: "القاهرة",
      from: "الإسكندرية",
      bookedSeats: [2, 3, 4],
      departureDay: "الخميس",
      departureDate: "2025-04-24",
      arrivalDay: "الخميس",
      arrivalTime: "10:00 ص",
      location: "محطة مصر",
    };
    route.fulfill({ json });
  });
  await page.route("**/api/Tickets", async (route, request) => {
    const requestBody = await request.postDataJSON();

    console.log("Intercepted ticket booking request:", requestBody);

    // Return a mock response similar to what your backend returns
    route.fulfill({
      status: 200,
      contentType: "application/json",
      body: JSON.stringify({
        data: "http://localhost:5173/profile/tickets?success=true",
      }),
    });
  });
  // 2. Login first
  await login(page);

  // 3. Go to trip search page
  await page.goto(
    "http://localhost:5173/search-Result?from=القاهرة&to=الإسكندرية&date=2025-05-01"
  );

  // 4. Wait for trips to load
  await page.getByRole("button", { name: "اختر" }).click();

  // 6. Wait for seat map
  await page.waitForSelector("text=اختر رقم مقعدك");

  // 7. Choose seat 1 (available in mock)
  await page.getByRole("button", { name: /^1$/, exact: true }).click();

  // 8. Complete booking
  await page.getByRole("button", { name: "إتمام الحجز" }).click();

  // 9. Expect redirection
  await expect(page.getByText("تم حجز التذكرة بنجاح")).toBeVisible();
});

test("user can log in, view mocked trips and see book a seat error", async ({
  page,
}) => {
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

  // 1. Setup mock API
  await page.route("**/api/Trips/Search**", (route) => {
    route.fulfill({
      status: 200,
      contentType: "application/json",
      body: JSON.stringify(mockTrips),
    });
  });

  await page.route("**/api/Trips/1", async (route) => {
    const json = {
      id: 1,
      companyName: "Test Bus Co",
      price: 100,
      to: "القاهرة",
      from: "الإسكندرية",
      bookedSeats: [2, 3, 4],
      departureDay: "الخميس",
      departureDate: "2025-04-24",
      arrivalDay: "الخميس",
      arrivalTime: "10:00 ص",
      location: "محطة مصر",
    };
    route.fulfill({ json });
  });
  await page.route("**/api/Tickets", async (route, request) => {
    const requestBody = await request.postDataJSON();

    console.log("Intercepted ticket booking request:", requestBody);

    // Return a mock response similar to what your backend returns
    route.fulfill({
      status: 200,
      contentType: "application/json",
      body: JSON.stringify({
        data: "http://localhost:5173/profile/tickets?success=false",
      }),
    });
  });
  // 2. Login first
  await login(page);

  // 3. Go to trip search page
  await page.goto(
    "http://localhost:5173/search-Result?from=القاهرة&to=الإسكندرية&date=2025-05-01"
  );

  // 4. Wait for trips to load
  await page.getByRole("button", { name: "اختر" }).click();

  // 6. Wait for seat map
  await page.waitForSelector("text=اختر رقم مقعدك");

  // 7. Choose seat 1 (available in mock)
  await page.getByRole("button", { name: /^1$/, exact: true }).click();

  // 8. Complete booking
  await page.getByRole("button", { name: "إتمام الحجز" }).click();

  // 9. Expect redirection
  await expect(page.getByText("حدث خطأ اثناء الحجز")).toBeVisible();
});

test("user can cancel a ticket", async ({ page }) => {
  // Mock the ticket data
  await page.route("**/api/1/tickets", async (route) => {
    route.fulfill({
      status: 200,
      contentType: "application/json",
      body: JSON.stringify([
        {
          bookingId: 1,
          from: "القاهرة",
          to: "الإسكندرية",
          departureDate: "2025-05-01",

          seatNumber: 7,
        },
      ]),
    });
  });
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
  await page.route("**/api/Tickets/1/cancel", async (route) => {
    await route.fulfill({
      status: 200,
      contentType: "application/json",
      body: JSON.stringify({ message: "تم إلغاء التذكرة بنجاح" }),
    });
  });

  // Mock the DELETE API for cancellation

  await login(page);
  // Go to profile page
  await page.goto("http://localhost:5173/profile/tickets");

  // Confirm the ticket shows
  await expect(page.getByText("القاهرة")).toBeVisible();
  await expect(page.getByText("الإسكندرية")).toBeVisible();

  // Click the cancel button
  await page.getByRole("button", { name: "الغاء الحجز" }).click();

  // Optionally, confirm a dialog
  // await page.getByRole('dialog').getByRole('button', { name: 'تأكيد' }).click();

  // Confirm ticket is removed
  await expect(page.getByText("القاهرة")).not.toBeVisible();

  // Confirm success message
  await expect(page.getByText("تم إلغاء التذكرة بنجاح")).toBeVisible();
});
