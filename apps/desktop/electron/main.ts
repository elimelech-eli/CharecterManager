import { app, BrowserWindow, ipcMain } from "electron";
import path from "node:path";
import { ChildProcess, spawn } from "node:child_process";
import { existsSync } from "node:fs";
import { fileURLToPath } from "node:url";

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

const backendPort = process.env.CHARACTER_MANAGER_BACKEND_PORT ?? "53987";
const backendBaseUrl = `http://127.0.0.1:${backendPort}`;

let mainWindow: BrowserWindow | null = null;
let backendProcess: ChildProcess | null = null;

function backendExecutablePath(): string {
  if (!app.isPackaged) {
    return "dotnet";
  }

  const bundledExe = path.join(process.resourcesPath, "backend", "CharacterManager.Api.exe");
  return existsSync(bundledExe) ? bundledExe : "dotnet";
}

function backendArguments(): string[] {
  if (!app.isPackaged) {
    return [
      "run",
      "--project",
      path.resolve(__dirname, "../../../src/CharacterManager.Api/CharacterManager.Api.csproj")
    ];
  }

  const bundledDll = path.join(process.resourcesPath, "backend", "CharacterManager.Api.dll");
  if (existsSync(bundledDll) && !existsSync(path.join(process.resourcesPath, "backend", "CharacterManager.Api.exe"))) {
    return [bundledDll];
  }

  return [];
}

function startBackend(): void {
  if (backendProcess) {
    return;
  }

  backendProcess = spawn(backendExecutablePath(), backendArguments(), {
    env: {
      ...process.env,
      ASPNETCORE_URLS: backendBaseUrl,
      CHARACTER_MANAGER_BACKEND_PORT: backendPort
    },
    stdio: app.isPackaged ? "ignore" : "inherit",
    windowsHide: true
  });

  backendProcess.once("exit", () => {
    backendProcess = null;
  });
}

async function createWindow(): Promise<void> {
  mainWindow = new BrowserWindow({
    width: 1280,
    height: 840,
    minWidth: 960,
    minHeight: 680,
    show: false,
    autoHideMenuBar: true,
    webPreferences: {
      preload: path.join(__dirname, "preload.js"),
      contextIsolation: true,
      nodeIntegration: false
    }
  });

  mainWindow.once("ready-to-show", () => {
    mainWindow?.show();
  });

  if (!app.isPackaged) {
    await mainWindow.loadURL("http://127.0.0.1:5173");
    mainWindow.webContents.openDevTools({ mode: "detach" });
    return;
  }

  await mainWindow.loadFile(path.join(__dirname, "../dist/index.html"));
}

ipcMain.handle("backend:getBaseUrl", () => backendBaseUrl);

app.whenReady().then(async () => {
  startBackend();
  await createWindow();

  app.on("activate", async () => {
    if (BrowserWindow.getAllWindows().length === 0) {
      await createWindow();
    }
  });
});

app.on("before-quit", () => {
  backendProcess?.kill();
  backendProcess = null;
});

app.on("window-all-closed", () => {
  if (process.platform !== "darwin") {
    app.quit();
  }
});
