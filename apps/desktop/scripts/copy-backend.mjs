import { cp, mkdir, rm } from "node:fs/promises";
import path from "node:path";
import { fileURLToPath } from "node:url";

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);
const desktopRoot = path.resolve(__dirname, "..");
const publishFlavor = process.argv[2] ?? "framework";
const source = path.join(desktopRoot, "backend-publish", publishFlavor);
const target = path.join(desktopRoot, "backend-runtime");

await rm(target, { recursive: true, force: true });
await mkdir(target, { recursive: true });
await cp(source, target, { recursive: true });

console.log(`Copied .NET backend from ${source} to ${target}`);
