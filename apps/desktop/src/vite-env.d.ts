/// <reference types="vite/client" />

interface CharacterManagerBridge {
  getBackendBaseUrl: () => Promise<string>;
}

interface Window {
  characterManager?: CharacterManagerBridge;
}
