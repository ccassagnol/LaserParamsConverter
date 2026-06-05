# Getting started

> The original WinForms-era user guide is available as
> [`upstream-getting-started.md`](upstream-getting-started.md) (converted from
> [`legacy/winforms/Getting Started Guide.pdf`](../legacy/winforms/Getting%20Started%20Guide.pdf)).
> This page focuses on the cross-platform fork.

## Quick start (developer)

```pwsh
git clone https://github.com/ccassagnol/LaserParamsConverter.git
cd LaserParamsConverter
dotnet test  LaserParamsConverter.sln
dotnet run  --project src/LPC.App
```

The app prints a `http://127.0.0.1:<port>/` URL and opens your default browser.

## What's in the engine today

The conversion engine (`src/LPC.Core`) covers:

* LightBurn `.clb` library load / save / convert (lens + wattage scaling).
* Pure speed/power scaling math (`LaserParamScaler.Scale`).
* Output filename + embedded `DisplayName` rewriting so a converted file
  doesn't lie about its profile — for example,
  `30W_210mm_LASER-SECRETS_1.clb` converted to a 290mm lens becomes
  `30W_290mm_LASER-SECRETS_1.clb` (and the `<LightBurnLibrary DisplayName=...>`
  attribute inside the XML is updated to match).

## What's wired in the UI today

`/convert` — full LightBurn `.clb` conversion flow:

1. Drag-and-drop or browse to a `.clb` file.
2. Set source profile (laser type, wattage, lens).
3. Set target profile (laser type, max-power cap, wattage, lens).
4. Preview the output filename and embedded `DisplayName`.
5. Click **Convert and download** — the browser saves the converted file.

`/convert-one` and `/advanced` are stubs.

## What's still TODO

* EZCAD2 `.lib` parser (need real samples to validate against).
* EZCAD3 `.ini` parser (same).
* CSV export.
* Single-pair calculator page (`/convert-one`).
* Advanced overrides page (`/advanced`).
* GitHub release publishing for binaries (see
  [`docs/release-workflow.md`](release-workflow.md)).
