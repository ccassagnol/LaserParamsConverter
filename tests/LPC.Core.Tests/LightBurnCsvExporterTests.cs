/*
 * This file is part of LaserParamsConverter (cross-platform fork).
 * Licensed under the GNU General Public License, version 3.
 */

using LPC.Core.Formats;
using Xunit;

namespace LPC.Core.Tests;

public class LightBurnCsvExporterTests
{
    private static string SamplePath(string name)
        => Path.Combine(AppContext.BaseDirectory, "Samples", name);

    [Fact]
    public void Exports_Header_And_At_Least_One_Row()
    {
        var lib = LightBurnLibrary.Load(SamplePath("30W_210mm_LASER-SECRETS_1.clb"));
        var csv = LightBurnCsvExporter.ToCsv(lib);

        var lines = csv.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        Assert.StartsWith("Name,Pass,Speed,Power,Freq (KHz),Mode,Interval", lines[0]);
        Assert.True(lines.Length > 1, "Should have at least one data row.");
    }

    [Fact]
    public void First_Row_Reflects_First_Entry_Values()
    {
        // First entry: Material=ABS, NoThickTitle=Black, Desc=Lt. Tan,
        //              maxPower=18, speed=1000, type=Scan -> Mode=Fill.
        // There's no <numPasses> on this entry so Pass defaults to 1.
        var lib = LightBurnLibrary.Load(SamplePath("30W_210mm_LASER-SECRETS_1.clb"));
        var csv = LightBurnCsvExporter.ToCsv(lib);

        var lines = csv.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        var first = lines[1].TrimEnd('\r');

        Assert.StartsWith("\"ABS Black Lt. Tan\",", first);
        Assert.Contains(",1000,", first); // speed
        Assert.Contains(",18,", first);   // maxPower
        Assert.EndsWith(",Fill,0.0635", first); // mode + interval
    }

    [Fact]
    public void Embedded_Quotes_In_Names_Are_Escaped()
    {
        // Construct a tiny library with a quote in the material name.
        var xml = """
            <?xml version="1.0" encoding="UTF-8"?>
            <LightBurnLibrary DisplayName="test">
              <Material name='Has "Quote"'>
                <Entry Thickness="-1.0000" Desc="d" NoThickTitle="t">
                  <CutSetting type="Scan">
                    <speed Value="500" />
                    <minPower Value="1" />
                    <maxPower Value="2" />
                    <frequency Value="20000" />
                    <interval Value="0.025" />
                  </CutSetting>
                </Entry>
              </Material>
            </LightBurnLibrary>
            """;

        var lib = LightBurnLibrary.Parse(xml);
        var csv = LightBurnCsvExporter.ToCsv(lib);
        var second = csv.Split('\n')[1];

        Assert.StartsWith("\"Has \"\"Quote\"\" t d\",", second);
    }
}
