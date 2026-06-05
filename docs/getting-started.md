# Getting started

> The full upstream guide is preserved at
> [`legacy/winforms/Getting Started Guide.pdf`](../legacy/winforms/Getting%20Started%20Guide.pdf).
> This page will replace it once the new UI is wired.

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

## What's still TODO

* EZCAD2 `.lib` parser (need real samples to validate against).
* EZCAD3 `.ini` parser (same).
* CSV export.
* UI wiring (file picker, profile inputs, preview pane, save).
* GitHub release publishing for binaries.
