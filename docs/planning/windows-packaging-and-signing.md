# Windows Packaging and Signing

Unsigned portable Electron executables are likely to trigger Windows SmartScreen and antivirus reputation systems, especially while the app is new and unreleased.

## Current Policy

- Local packaging uses `npm run package:win`, which maps to `npm run package:win:dir`.
- Local packaging creates `apps/desktop/release/win-unpacked` instead of a portable self-extracting `.exe`.
- Local packaging publishes the backend as a framework-dependent DLL and launches it through `dotnet`.
- User-facing release builds use `npm run package:win:release`.
- Release builds produce an NSIS installer and require signing secrets.
- The release workflow refuses to create an unsigned user-facing installer.

## Required GitHub Secrets for Release

Add these repository secrets before publishing Windows releases:

- `WINDOWS_CERTIFICATE_BASE64`
- `WINDOWS_CERTIFICATE_PASSWORD`

`WINDOWS_CERTIFICATE_BASE64` should contain the base64-encoded code-signing certificate bundle expected by electron-builder. `WINDOWS_CERTIFICATE_PASSWORD` should contain the certificate password.

## Why Portable Builds Are Disabled by Default

The previous default produced a portable self-extracting `.exe` that bundled Electron plus a generated .NET backend executable. That pattern is high-risk for antivirus false positives because the file is unsigned, newly generated, self-extracting, and contains another executable payload.

Portable builds should not be restored as the default until releases are signed and tested against Windows reputation tooling.

## Local Self-Signed Development Signing

Self-signed signing is available only to reduce local development friction. It does not help public users unless they manually trust the certificate, which they should not be asked to do.

Create and trust a local development certificate:

```powershell
cd apps/desktop
npm run cert:dev:create
```

If Windows still reports the generated installer as signed by an untrusted publisher, run the same command from an elevated PowerShell session or manually import `apps/desktop/.certificates/character-manager-dev-signing.cer` into:

- Trusted Root Certification Authorities
- Trusted Publishers

Build a locally self-signed installer:

```powershell
npm run package:win:self-signed
```

Remove the local development certificate:

```powershell
npm run cert:dev:remove
```

The generated certificate files and random PFX password are stored under `apps/desktop/.certificates`, which is ignored by Git.
