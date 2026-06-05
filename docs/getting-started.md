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

* `/` — landing page with navigation.
* `/convert` — full LightBurn `.clb` conversion flow: upload, set source/target
  profiles, preview output filename + embedded `DisplayName`, **Convert and
  download** as `.clb` or **Export as CSV**. Advanced overrides are applied
  automatically.
* `/convert-one` — single (speed, power) pair calculator using the same source
  and target profiles set on `/convert`. Shows when speed was reduced due to a
  power clip at MaxPower.
* `/advanced` — `QPulseWidth` override toggle and value. Settings are
  process-scoped and apply to every subsequent conversion until changed or
  reset.

State is shared across the three pages via a singleton `ConverterState` service
so navigating between them doesn't reset your inputs.

## What's still TODO

* EZCAD2 `.lib` parser (need real samples to validate against).
* EZCAD3 `.ini` parser (same).
* Combine / extract libraries page (lower priority).
