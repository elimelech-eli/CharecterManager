import { contextBridge, ipcRenderer } from "electron";

contextBridge.exposeInMainWorld("characterManager", {
  getBackendBaseUrl: (): Promise<string> => ipcRenderer.invoke("backend:getBaseUrl")
});
