# Branch Protection

GitHub Actions can run the checks, but the repository must also require those checks before `main` can be merged.

## Required PR Checks

Enable branch protection or a repository ruleset for `main` and require these status checks:

- `CI / .NET unit tests`
- `CI / Desktop typecheck and build`

Recommended settings:

- Require a pull request before merging.
- Require status checks to pass before merging.
- Require branches to be up to date before merging.
- Do not require `E2E After Merge / Packaged app smoke` before merging; it runs after merges to `main`.

## Post-Merge E2E

The `E2E After Merge` workflow runs on every push to `main`. It currently performs the closest available end-to-end check:

- Installs desktop dependencies.
- Runs .NET tests.
- Builds the React/Electron renderer.
- Publishes the .NET backend.
- Packages the Windows portable app.
- Uploads the generated `.exe` artifact.

Dedicated UI and browser-driven E2E tests can be added to this workflow later.
