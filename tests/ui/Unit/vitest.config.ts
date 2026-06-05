import path from "node:path";

const repoRoot = path.resolve(__dirname, "../../..");
const desktopRoot = path.join(repoRoot, "apps", "desktop");
const desktopModules = path.join(desktopRoot, "node_modules");

export default {
  root: desktopRoot,
  resolve: {
    alias: [
      {
        find: "@testing-library/jest-dom/vitest",
        replacement: path.join(desktopModules, "@testing-library", "jest-dom", "vitest.js")
      },
      {
        find: "@testing-library/react",
        replacement: path.join(desktopModules, "@testing-library", "react", "dist", "@testing-library", "react.esm.js")
      },
      {
        find: "@testing-library/user-event",
        replacement: path.join(desktopModules, "@testing-library", "user-event", "dist", "esm", "index.js")
      },
      {
        find: "vitest",
        replacement: path.join(desktopModules, "vitest", "dist", "index.js")
      },
      {
        find: "react/jsx-runtime",
        replacement: path.join(desktopModules, "react", "jsx-runtime.js")
      },
      {
        find: "react/jsx-dev-runtime",
        replacement: path.join(desktopModules, "react", "jsx-dev-runtime.js")
      },
      {
        find: "react",
        replacement: path.join(desktopModules, "react", "index.js")
      },
      {
        find: "react-dom/test-utils",
        replacement: path.join(desktopModules, "react-dom", "test-utils.js")
      },
      {
        find: "react-dom/client",
        replacement: path.join(desktopModules, "react-dom", "client.js")
      },
      {
        find: "react-dom",
        replacement: path.join(desktopModules, "react-dom", "index.js")
      },
      {
        find: "lucide-react",
        replacement: path.join(desktopModules, "lucide-react", "dist", "esm", "lucide-react.js")
      }
    ]
  },
  test: {
    environment: "jsdom",
    include: ["../../tests/ui/Unit/**/*.test.{ts,tsx}"],
    outputFile: {
      junit: path.join(repoRoot, "tests", "ui", "Reports", "Unit", "ui-unit.junit.xml")
    },
    reporters: ["default", "junit"],
    setupFiles: [path.join(repoRoot, "tests", "ui", "Unit", "setup.ts")]
  }
};
