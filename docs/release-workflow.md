# Release workflow — what to do when you're ready

The CI build at `.github/workflows/build.yml` only **builds and tests** on every
push / PR. It does not produce downloadable binaries.

To get cross-platform installable artifacts attached to a GitHub Release, here
is the minimum set of steps. None of this is in the repo yet — that's the
deliberate next step. Everything in this doc happens **after** the `LPC.App`
UI is good enough to ship to others.

## 1. Decide what artifacts you want to ship

For each release, the workflow will produce:

| Artifact | RID | Wrapper |
|---|---|---|
| `LaserParamsConverter-{version}-win-x64.zip` | `win-x64` | ZIP containing `LaserParamsConverter.exe` (self-contained single-file) |
| `LaserParamsConverter-{version}-osx-x64.tar.gz` | `osx-x64` | TAR.GZ containing `LaserParamsConverter.app` bundle |
| `LaserParamsConverter-{version}-osx-arm64.tar.gz` | `osx-arm64` | TAR.GZ containing `LaserParamsConverter.app` bundle |
| `LaserParamsConverter-{version}-linux-x64.tar.gz` | `linux-x64` | TAR.GZ containing the single-file binary |

The `.app` bundle wrapping is a small `Info.plist` + folder layout — about a
dozen lines of shell. We'll generate it in the workflow.

## 2. What you need to do (one-time)

### Required

1. **Enable releases on the private repo** — already on by default for private
   repos. Nothing to do.
2. **Verify GitHub Actions has write access to releases**: Settings → Actions →
   General → Workflow permissions → set to "Read and write permissions".
   This lets the workflow attach files to a release without a Personal Access
   Token.

### Optional (recommended)

3. **Choose a versioning approach**:
   - **Tag-driven** (recommended): you `git tag v2.0.0-alpha.1 && git push --tags`
     and the workflow runs. Version comes from the tag.
   - **Manual dispatch**: trigger via the Actions UI with a typed version.
   - **Calendar-versioned**: `v2026.06.01` etc. Simpler if you don't care about
     semver.
4. **Pre-release vs. final**: until v2.0 is feature-complete, ship as GitHub
   *Pre-release* (the workflow can set this automatically when the tag matches
   `*-alpha*` / `*-beta*`).

### Not required (for unsigned macOS)

5. macOS notarization / Apple Developer ID. Until you have one, the workflow
   ships unsigned `.app` bundles and users follow
   [`macos-gatekeeper.md`](macos-gatekeeper.md). When/if you enroll
   ($99/yr), we add a `codesign` + `xcrun notarytool` step to the macOS job.

## 3. What I'll do when you give the green light

I'll create a `.github/workflows/release.yml` that:

1. Triggers on tag pushes matching `v*` (and supports `workflow_dispatch` for
   manual runs).
2. Reads the version from the tag (`${{ github.ref_name }}`).
3. For each OS / RID, runs:
   ```pwsh
   dotnet publish src/LPC.App `
     -c Release `
     -r <rid> `
     --self-contained `
     -p:PublishSingleFile=true `
     -p:Version=<version> `
     -o publish/<rid>
   ```
4. For macOS, wraps the published output into a `LaserParamsConverter.app`
   bundle with a generated `Info.plist`.
5. Packages each result as ZIP (Windows) or TAR.GZ (macOS / Linux).
6. Creates a GitHub Release with auto-generated release notes from PR
   titles since the previous tag, attaches all four artifacts, marks
   pre-release if the tag contains `-alpha` / `-beta`.

## 4. Cutting your first release (once the workflow exists)

```pwsh
cd C:\repos\LaserParamsConverter
# Make sure everything is committed and pushed.
git tag -a v2.0.0-alpha.1 -m "First cross-platform alpha"
git push origin v2.0.0-alpha.1
```

Then watch the Actions tab on GitHub. When it finishes, the release will be at:

```
https://github.com/ccassagnol/LaserParamsConverter/releases/tag/v2.0.0-alpha.1
```

## 5. What you do NOT need to do

- No NuGet account.
- No Microsoft Store / App Store / Snap Store / Flatpak account.
- No Apple Developer ID (yet).
- No code signing certificate for Windows (Authenticode certs are ~$100/yr from
  Sectigo; the upstream project signed releases, but for private distribution
  this is optional — users will see the Windows SmartScreen prompt on first run).
- No external secrets management. The workflow uses only the built-in
  `GITHUB_TOKEN`.

## TL;DR

**Just say "set up the release workflow" when you're ready.** No external
account setup is required — the only prep step is flipping the workflow
permission switch in repo settings.
