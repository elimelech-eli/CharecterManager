# CharacterManager

Desktop character management for TTRPGs, starting with Ironsworn.

The application is planned as an Electron shell with a React UI and a bundled .NET backend for rules, character state, and future ruleset extensibility.

## Shape

- `apps/desktop` - Electron, React, Vite, and Windows packaging.
- `src/CharacterManager.Api` - local .NET backend launched by Electron.
- `src/CharacterManager.Core` - domain logic for characters, rulesets, moves, and rolls.
- `src/CharacterManager.Infrastructure` - adapters for external data sources such as MongoDB Atlas.
- `tests` - .NET tests.

## Prerequisites

- Git
- Node.js and npm
- .NET 8 SDK

## First Run

```powershell
cd apps/desktop
npm install
npm run dev
```

In another terminal:

```powershell
dotnet run --project ../../src/CharacterManager.Api/CharacterManager.Api.csproj
```

The Electron development process can also launch the backend automatically with `npm run dev:electron` once dependencies are installed.

## Windows Portable Build

```powershell
cd apps/desktop
npm install
npm run package:win
```

This publishes the .NET backend as a self-contained Windows executable, copies it into Electron resources, builds the React UI, and creates a portable Windows `.exe`.

## Repository Note

The GitHub remote is currently named `CharecterManager.git`; the local folder is intentionally named `CharacterManager`.
