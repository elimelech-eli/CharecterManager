import { expect, request, test } from "@playwright/test";

const apiBaseUrl = "http://127.0.0.1:53987";

test.beforeEach(async () => {
  const api = await request.newContext({ baseURL: apiBaseUrl });
  const characters = await api.get("/api/v1/characters");
  expect(characters.ok()).toBeTruthy();

  for (const character of (await characters.json()).items) {
    await api.delete(`/api/v1/characters/${character.id}`);
  }

  await api.dispose();
});

test("creates, validates, finalizes, and reopens a character", async ({ page }) => {
  await page.goto("/");

  await expect(page.getByRole("heading", { name: "Character Library" })).toBeVisible();
  await expect(page.getByRole("heading", { name: "No local characters yet" })).toBeVisible();

  await page.getByRole("button", { name: "New Character" }).first().click();
  await expect(page.getByRole("heading", { name: "Create Character" })).toBeVisible();
  await page.getByRole("button", { name: /Review/ }).click();
  await page.getByRole("button", { name: "Finalize" }).click();
  await expect(page.getByText("Name is required before finalizing.")).toBeVisible();

  await page.getByRole("button", { name: "Open Concept" }).click();
  await page.getByLabel("Name").fill("Sable Ward");
  await page.getByLabel("Concept").fill("Exiled warden seeking a lost hearth-path.");
  await page.getByRole("button", { name: "Continue" }).click();

  await page.getByLabel("Edge").selectOption("3");
  await page.getByLabel("Heart").selectOption("2");
  await page.getByLabel("Iron").selectOption("2");
  await page.getByLabel("Shadow").selectOption("1");
  await page.getByLabel("Wits").selectOption("1");
  await page.getByRole("button", { name: "Continue" }).click();

  await page.locator("article", { hasText: "Archer" }).getByRole("checkbox", { name: "Select" }).check({ force: true });
  await page.locator("article", { hasText: "Duelist" }).getByRole("checkbox", { name: "Select" }).check({ force: true });
  await page.locator("summary", { hasText: "Path" }).click();
  await page.locator("article", { hasText: "Alchemist" }).getByRole("checkbox", { name: "Select" }).check({ force: true });
  await page.getByRole("button", { name: "Continue" }).click();

  await page.getByRole("button", { name: "Continue" }).click();
  await page.locator("article", { hasText: "Background vow" }).getByRole("button", { name: "Add" }).click();
  await page.locator("article", { hasText: "Background vow" }).getByLabel("Title").fill("Find the lost hearth-path");
  await page.locator("article", { hasText: "Background vow" }).getByLabel("Rank").selectOption("dangerous");
  await page.locator("article", { hasText: "Inciting-incident vow" }).getByRole("button", { name: "Add" }).click();
  await page.locator("article", { hasText: "Inciting-incident vow" }).getByLabel("Title").fill("Recover the wardens' iron ring");
  await page.locator("article", { hasText: "Inciting-incident vow" }).getByLabel("Rank").selectOption("troublesome");
  await page.getByRole("button", { name: "Continue" }).click();

  await expect(page.getByRole("heading", { name: "Review" })).toBeVisible();
  await page.getByRole("button", { name: "Finalize" }).click();
  await expect(page.getByRole("heading", { name: "Sable Ward" })).toBeVisible();
  await expect(page.getByText("Exiled warden seeking a lost hearth-path.")).toBeVisible();

  await page.getByRole("button", { name: "Decrease Health" }).click();
  await expect(page.getByText("4").first()).toBeVisible();
  await page.getByRole("button", { name: "Increase Momentum" }).click();
  await expect(page.getByText("3").first()).toBeVisible();
  await page.getByRole("tab", { name: "Conditions" }).click();
  await page.getByRole("checkbox", { name: "Encumbered" }).check({ force: true });
  await expect(page.getByText("Encumbered").first()).toBeVisible();
  await page.getByRole("tab", { name: "Vows" }).click();
  await page.getByRole("button", { name: "Increase Progress for Find the lost hearth-path" }).click();
  await expect(page.getByText("1 of 40 ticks")).toBeVisible();

  await page.getByRole("button", { name: "Characters" }).click();
  await expect(page.getByRole("heading", { name: "Character Library" })).toBeVisible();
  await page.getByRole("button", { name: "Open" }).click();
  await expect(page.getByRole("heading", { name: "Sable Ward" })).toBeVisible();
  await page.getByRole("tab", { name: "Conditions" }).click();
  await expect(page.getByRole("checkbox", { name: "Encumbered" })).toBeChecked();
});
