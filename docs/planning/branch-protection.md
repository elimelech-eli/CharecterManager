# Branch Protection

GitHub Actions can run the checks, but the repository must also require those checks before `main` can be merged.

## Required PR Checks

Enable branch protection or a repository ruleset for `main` and require these status checks:

- `CI / .NET unit tests`
- `CI / Desktop typecheck and build`
- `CI / UI component tests`
- `CI / UI E2E tests`

Recommended settings:

- Require a pull request before merging.
- Require status checks to pass before merging.
- Require branches to be up to date before merging.
- Do not require `E2E After Merge / Packaged app smoke` before merging; it runs after merges to `main`.

## Post-Merge E2E

The `E2E After Merge` workflow runs on every push to `main`. It currently performs the closest available end-to-end check:

- Installs desktop dependencies.
- Runs .NET tests.
- Runs UI component tests.
- Runs UI E2E tests.
- Builds the React/Electron renderer.
- Publishes the .NET backend.
- Packages the unpacked Windows app.
- Uploads the generated app artifact.
