import { rm } from "node:fs/promises";
import path from "node:path";
import { fileURLToPath } from "node:url";

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);
const desktopRoot = path.resolve(__dirname, "..");

for (const folder of ["backend-publish", "backend-runtime"]) {
  await rm(path.join(desktopRoot, folder), { recursive: true, force: true });
}

console.log("Cleaned backend publish folders");
