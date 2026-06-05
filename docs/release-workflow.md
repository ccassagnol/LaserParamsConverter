# Release workflow

The release workflow at [`.github/workflows/release.yml`](../.github/workflows/release.yml)
is **in place and ready to use**. It triggers automatically on every tag push
matching `v*`, and can also be run manually via the Actions UI
(`workflow_dispatch`).

The separate CI build at [`.github/workflows/build.yml`](../.github/workflows/build.yml)
runs on every push / PR and only builds + tests — it does not produce
downloadable binaries.

## What the release workflow produces

Per tag, the workflow attaches these artifacts to a GitHub Release:

| Artifact | RID | Wrapper |
|---|---|---|
| `LaserParamsConverter-{tag}-win-x64.zip` | `win-x64` | ZIP with self-contained single-file `LaserParamsConverter.exe` |
| `LaserParamsConverter-{tag}-osx-x64.tar.gz` | `osx-x64` | TAR.GZ with `LaserParamsConverter.app` bundle + README.txt |
| `LaserParamsConverter-{tag}-osx-arm64.tar.gz` | `osx-arm64` | TAR.GZ with `LaserParamsConverter.app` bundle + README.txt |
| `LaserParamsConverter-{tag}-linux-x64.tar.gz` | `linux-x64` | TAR.GZ with self-contained single-file binary |

If the tag contains `-alpha`, `-beta`, or `-rc`, the release is marked as a
**pre-release** automatically. Release notes are auto-generated from PR titles
since the previous tag.

## Cutting a release

```pwsh
cd C:\repos\LaserParamsConverter
# Ensure main is clean and pushed.
git tag -a v2.0.0-alpha.1 -m "First cross-platform alpha"
git push origin v2.0.0-alpha.1
```

Watch the Actions tab. When it finishes, the release will be at:

```
https://github.com/ccassagnol/LaserParamsConverter/releases/tag/v2.0.0-alpha.1
```

You can also re-run a release manually via **Actions → release → Run workflow**
and supply the existing tag name (useful if you fix something in the workflow
and want to regenerate the binaries against the same code).

## Versioning convention

- Tag format: `v<semver>` (e.g. `v2.0.0-alpha.1`, `v2.0.0`, `v2.1.3-rc.2`).
- Alpha / beta / rc tags ship as **pre-releases**.
- The `.NET` assembly metadata is set per-build:
  - `Version` = the full SemVer (e.g. `2.0.0-alpha.1`)
  - `AssemblyVersion` / `FileVersion` = the numeric prefix only (e.g. `2.0.0.0`),
    because .NET assembly versions must be `MAJOR.MINOR.BUILD.REVISION` digits.

## macOS users

The macOS bundle is unsigned and unnotarized (no Apple Developer ID). The
artifact ships with a `README.txt` next to the `.app` explaining the one-time
`xattr -dr com.apple.quarantine LaserParamsConverter.app` step. Same workaround
documented at [`macos-gatekeeper.md`](macos-gatekeeper.md).

## What you do NOT need

- No NuGet account.
- No Microsoft Store / App Store / Snap Store / Flatpak account.
- No Apple Developer ID (yet).
- No Windows Authenticode certificate (users will see SmartScreen on first run
  — same as the upstream did before they got a cert).
- No external secrets configuration. The workflow uses only the built-in
  `GITHUB_TOKEN`.

## When you DO want signing

For Windows Authenticode (if you ever buy a cert):
```yaml
- name: Sign Windows binary
  if: matrix.rid == 'win-x64'
  run: |
    signtool sign /tr http://timestamp.sectigo.com /td sha256 /fd sha256 \
      /f cert.pfx /p "${{ secrets.WIN_CERT_PASSWORD }}" \
      publish/win-x64/LaserParamsConverter.exe
```

For macOS Developer ID (if you ever enroll in the Apple Developer Program):
```yaml
- name: Sign macOS bundle
  if: startsWith(matrix.rid, 'osx-')
  run: |
    codesign --deep --force --options runtime \
      --sign "Developer ID Application: Your Name" \
      "staged/${stem}/LaserParamsConverter.app"
    xcrun notarytool submit "dist/${stem}.tar.gz" \
      --apple-id "${{ secrets.APPLE_ID }}" \
      --team-id "${{ secrets.APPLE_TEAM_ID }}" \
      --password "${{ secrets.APPLE_APP_PASSWORD }}" \
      --wait
```
