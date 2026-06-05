# Original Getting Started Guide (upstream v1.1.3)

> Verbatim conversion of `legacy/winforms/Getting Started Guide.pdf` to
> Markdown. Kept here so the upstream documentation is searchable, diff-able,
> and survives if the PDF is removed. The cross-platform fork's user-facing
> documentation lives in [`docs/getting-started.md`](./getting-started.md).
>
> © 2022 David Christian. Distributed under GPL-3.0.

---

## Laser Params Converter — Getting Started

### Installation

Copy the installation files into a folder and run `setup.exe`. The software
requires the Microsoft .NET 6.0 runtime to be installed. If your computer does
not have the required runtime installed, you will be presented with a screen to
install the prerequisite. Just click Install to continue. Follow the steps to
complete the installation.

> Note (fork): the cross-platform port targets **.NET 10** and ships as a
> self-contained single-file binary, so no separate runtime install is needed.

### Using Laser Params Converter

The basic operation of Laser Params Converter (LPC) consists of three steps:

1. Open a library file
2. Convert the library
3. Save the converted library

> Note: The power and speed values in the upstream screenshot were obscured to
> protect the proprietary status of the library.

### Library Format

Select either EZCAD2 or LightBurn. LPC has only been tested with EZCAD2. It may
work with EZCAD3 parameter libraries, but that is untested.

### Laser Type

Select either Fiber Laser or CO2 Gantry laser type. The laser type selection
will set default Max Power values. Selecting CO2 laser will disable Lens size
parameters.

### Conversion Parameters

LPC will convert the library using the following parameters. Make sure these
values are correct before you convert:

#### Max Power

Maximum power percentage for the laser. For CO2 lasers you should leave this at
90% as it is not recommended to run a CO2 at full power for an extended time.
It is safe to run Fiber lasers at 100% power.

#### Input Lens and Wattage

The lens size (for fiber) and wattage of the library being converted. This
should match the laser used for the library being converted.

#### Output Lens and Wattage

The lens size and wattage of the laser the library is being converted to. Set
this to match the laser the library is being converted for.

### Conversion Calculator

Click **Convert One** to use the conversion calculator to perform a manual
conversion for a single parameter. If you have an input parameter selected on
the left, the calculator will default to those values for Power and Speed. You
can enter any parameter values you like into this calculator and click Convert
to see the output Power and Speed.

### Converting an EZCAD2 library to LightBurn

LPC can convert a library in EZCAD2 format into LightBurn format. Just select
the Save as type to LightBurn Cut Library (`*.clb`) and then save. The
converted library can then be opened in LightBurn. The converted library has
been tested in LightBurn version 1.2, which has full support for Gantry lasers
that use the EZCAD2 control board.

### Organizing a converted EZCAD2 library in LightBurn

EZCAD2 has a flat parameter library, which is displayed as a simple list.
Different parameters for the same material are usually named in a similar
manner so that the parameters are grouped and sorted together. LightBurn uses a
more advanced hierarchical layout that is represented by a tree structure. To
take advantage of the LightBurn organizational structure, you will need to edit
your converted EZCAD2 library.

The LightBurn library is organized into three levels. The first level is the
Material name, the second level is the Title (or Thickness for cutting material
with CO2 lasers), and the third level is the Description of your parameter
settings. The EZCAD2 library only has a single name for each parameter, so once
imported into LightBurn those levels will have default values of Title and
Description.

To organize and group your parameters for LightBurn, click on each parameter at
the third level and click the **Edit Desc** button. Now enter meaningful values
for Material Name (1st Level), Title (2nd Level), and Description (3rd Level).
LightBurn will automatically group items with the same Material and Title
values. Just play around with the names here and you will get the hang of it.

### Combine / Extract Libraries

The Combine feature provides for advanced library management features that are
not available in the EZCAD or LightBurn products. This feature allows you to
combine parameters from multiple libraries into a single, new library. You can
also use this feature to extract one or more parameters and save them to their
own library, which could be shared with others.

> Note: You cannot mix parameters of different formats (EZCAD or LightBurn) and
> you should take care that any libraries that you combine are already
> converted for the same Wattage laser and Lens size.

You may open one or more libraries, check off the parameters you want to
include, and then click the **Copy Selected** button to copy them to the output
list. You may open as many libraries as you wish to select parameters. When
satisfied with your new library, click **Save Library File** to save the new
file. At any time, press **Reset** to start over.

There is a right-click context menu on each list which will allow you to
quickly Expand/Collapse the tree or to Select/Unselect all parameters. Clicking
a parent material in a LightBurn library will Select/Unselect all child
parameters under it.

### Other Features

#### Save as CSV

Laser Parameters Converter also has the ability to save a converted library in
Excel CSV (`*.csv`) format. This is useful if you would like to view your
library in Excel.

#### Check for Updates

If you check the **Check for updates on Startup** option on the main screen,
the program will check the LaserParamsConverter GitHub site on startup for new
releases. If a new release is available, you will be notified with a link to go
to the download page on the web. If upgrading, please be sure to close your
current version prior to running the install.
