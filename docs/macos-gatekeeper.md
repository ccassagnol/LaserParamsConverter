# Running on macOS without a Developer ID

This fork ships an unsigned, unnotarized macOS binary because we don't (yet)
have an Apple Developer ID ($99/yr). Gatekeeper will refuse to run the app on
first launch and silently quarantine it. There is a one-time workaround.

## One-time setup

After downloading and unpacking `LaserParamsConverter-osx-*.tar.gz`:

```bash
cd /path/to/extracted/folder
xattr -dr com.apple.quarantine LaserParamsConverter.app
```

The `xattr -dr` (recursive, delete) removes the quarantine extended attribute
that the browser/finder attached to every file inside the bundle. After this,
double-clicking the `.app` works normally.

## Why we don't sign

Apple charges $99/year for a Developer ID Application certificate, plus
notarization round-trips for every release. For a private fork with a small
user base it's not justified yet. If you decide to pay later:

1. Enroll in the Apple Developer Program.
2. Create a "Developer ID Application" cert in your keychain.
3. Replace the macOS publish step in `.github/workflows/build.yml` with
   `codesign --deep --sign "Developer ID Application: …"` then
   `xcrun notarytool submit --wait`.

## Troubleshooting

* **"App is damaged and can't be opened"**: re-run `xattr -dr com.apple.quarantine`.
* **"App cannot be opened because the developer cannot be verified"**:
  right-click the `.app` → Open → Open. macOS will remember the exception.
* **Apple Silicon vs Intel**: download the `osx-arm64` build for Apple Silicon
  (M1/M2/M3/M4) and the `osx-x64` build for Intel Macs.
