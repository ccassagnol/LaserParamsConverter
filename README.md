# LaserParamsConverter (cross-platform fork)

Cross-platform fork of [shark92651/LaserParamsConverter](https://github.com/shark92651/LaserParamsConverter)
by David Christian. Converts laser parameter libraries (EZCAD2, EZCAD3,
LightBurn) between different wattages and lens sizes.

The original is Windows-only WinForms. This fork rebuilds the app as a
lightweight local Blazor Server process that runs on Windows, macOS, and Linux.

> Status: **v2.0.0-alpha** — core engine + LightBurn parser + tests are in
> place. UI wiring is in progress. The original WinForms source is preserved
> under [`legacy/winforms/`](legacy/winforms/) for reference.

## Repository layout

| Path | Purpose |
|---|---|
| `src/LPC.Core/` | Format parsers, conversion math, naming helpers. No UI. |
| `src/LPC.App/` | Blazor Server desktop app. Binds to `127.0.0.1`, auto-launches the default browser. |
| `tests/LPC.Core.Tests/` | xUnit tests, including golden-file tests against real `.clb` samples. |
| `legacy/winforms/` | Frozen copy of the upstream Windows-only WinForms source. **Do not modify.** |
| `docs/` | User docs (getting started, macOS Gatekeeper workaround). |
| `.github/workflows/` | Build matrix for Windows / macOS / Linux. |

## Build and run

Requires the **.NET 10 SDK** (pinned in `global.json`).

```pwsh
dotnet build LaserParamsConverter.sln
dotnet test  LaserParamsConverter.sln
dotnet run  --project src/LPC.App
```

When the app starts, it prints a `http://127.0.0.1:<port>/` URL and opens it in
your default browser.

## Cross-platform packaging

Self-contained single-file binaries per OS:

```pwsh
# Windows
dotnet publish src/LPC.App -c Release -r win-x64   --self-contained -p:PublishSingleFile=true

# macOS (Intel + Apple Silicon)
dotnet publish src/LPC.App -c Release -r osx-x64   --self-contained -p:PublishSingleFile=true
dotnet publish src/LPC.App -c Release -r osx-arm64 --self-contained -p:PublishSingleFile=true

# Linux
dotnet publish src/LPC.App -c Release -r linux-x64 --self-contained -p:PublishSingleFile=true
```

### macOS install note

The macOS binary is **not signed or notarized** (no Apple Developer ID). After
unpacking the `.tar.gz`, run once:

```bash
xattr -dr com.apple.quarantine LaserParamsConverter.app
```

Full procedure in [`docs/macos-gatekeeper.md`](docs/macos-gatekeeper.md).

## License

GPL-3.0 — same as upstream. See [`LICENSE`](LICENSE). Source obligations are
preserved even though this repository is private.

## Credit

Original work © 2022 David Christian.
Cross-platform port © 2026 Charles Cassagnol.
