import { defineConfig, devices } from "@playwright/test";
import path from "node:path";

const repoRoot = path.resolve(__dirname, "../../..");
const desktopRoot = path.join(repoRoot, "apps", "desktop");
const storageRoot = path.join(repoRoot, ".ui-e2e-data");

export default defineConfig({
  testDir: __dirname,
  timeout: 60_000,
  expect: {
    timeout: 10_000
  },
  reporter: [
    ["list"],
    ["junit", { outputFile: path.join(repoRoot, "tests", "ui", "Reports", "E2E", "ui-e2e.junit.xml") }],
    ["html", { open: "never", outputFolder: path.join(repoRoot, "tests", "ui", "Reports", "E2E", "html") }]
  ],
  use: {
    baseURL: "http://127.0.0.1:5173",
    trace: "retain-on-failure"
  },
  webServer: [
    {
      command: `powershell -NoProfile -ExecutionPolicy Bypass -Command "$env:CHARACTER_MANAGER_STORAGE_ROOT='${storageRoot}'; dotnet run --project src/CharacterManager.Api/CharacterManager.Api.csproj --urls http://127.0.0.1:53987"`,
      cwd: repoRoot,
      reuseExistingServer: false,
      timeout: 120_000,
      url: "http://127.0.0.1:53987/health"
    },
    {
      command: "npm.cmd run dev -- --host 127.0.0.1",
      cwd: desktopRoot,
      reuseExistingServer: false,
      timeout: 120_000,
      url: "http://127.0.0.1:5173"
    }
  ],
  projects: [
    {
      name: "chromium",
      use: { ...devices["Desktop Chrome"] }
    }
  ]
});
