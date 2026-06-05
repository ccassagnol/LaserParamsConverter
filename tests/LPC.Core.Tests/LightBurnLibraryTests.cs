/*
 * This file is part of LaserParamsConverter (cross-platform fork).
 * Licensed under the GNU General Public License, version 3.
 */

using LPC.Core.Formats;
using LPC.Core.Models;
using Xunit;

namespace LPC.Core.Tests;

public class LightBurnLibraryTests
{
    private static string SamplePath(string name)
        => Path.Combine(AppContext.BaseDirectory, "Samples", name);

    [Fact]
    public void Load_30W_210mm_Source_Has_Expected_Display_Name()
    {
        var lib = LightBurnLibrary.Load(SamplePath("30W_210mm_LASER-SECRETS_1.clb"));
        Assert.Equal("30W_210mm_LASER-SECRETS", lib.DisplayName);
        Assert.True(lib.MaterialCount > 0, "Sample library should contain materials.");
        Assert.True(lib.EntryCount > 0, "Sample library should contain entries.");
    }

    [Fact]
    public void Load_Converted_Sample_Round_Trips()
    {
        var path = SamplePath("210-290-Conversion.clb");
        var lib = LightBurnLibrary.Load(path);
        var xml = lib.ToXmlString();
        Assert.Contains("<LightBurnLibrary", xml, StringComparison.Ordinal);
        Assert.Equal(lib.MaterialCount, LightBurnLibrary.Parse(xml).MaterialCount);
    }

    [Fact]
    public void Convert_30W_210mm_To_30W_290mm_Scales_Power_By_Lens_Ratio()
    {
        // Source first entry: ABS / Black / Lt. Tan with minPower=4,
        // maxPower=18, speed=1000. Scaling 30W/210mm fiber -> 30W/290mm fiber
        // with MaxPower=100 (no clip):
        //   minPower: (4  * 290 / 210) * (30 / 30) = 5.52 -> 5 (truncated)
        //   maxPower: (18 * 290 / 210) * (30 / 30) = 24.86 -> 24 (truncated)
        //   speed:    unchanged (no clip)
        var source = LightBurnLibrary.Load(SamplePath("30W_210mm_LASER-SECRETS_1.clb"));

        var inProfile = new LaserProfile(LaserType.Fiber, 100, 210, 30);
        var outProfile = new LaserProfile(LaserType.Fiber, 100, 290, 30);

        var converted = source.Convert(inProfile, outProfile);

        var firstEntry = converted.Document
            .SelectSingleNode("//LightBurnLibrary/Material/Entry/CutSetting");
        Assert.NotNull(firstEntry);

        var minPower = firstEntry!.SelectSingleNode("./minPower")!.Attributes!["Value"]!.Value;
        var maxPower = firstEntry.SelectSingleNode("./maxPower")!.Attributes!["Value"]!.Value;
        var speed = firstEntry.SelectSingleNode("./speed")!.Attributes!["Value"]!.Value;

        Assert.Equal("5", minPower);
        Assert.Equal("24", maxPower);
        Assert.Equal("1000", speed);
    }

    [Fact]
    public void Convert_30W_To_100W_Same_Lens_Matches_Upstream_Converted_Sample()
    {
        // Comparing the two sample files in the repo, the actual conversion
        // that produced 210-290-Conversion.clb was wattage 30W -> 100W with
        // lens unchanged (not the 210mm -> 290mm the filename suggests).
        // First entry: minPower 4 -> 1, maxPower 18 -> 5 ((18 * 30) / 100 = 5.4 -> 5).
        var source = LightBurnLibrary.Load(SamplePath("30W_210mm_LASER-SECRETS_1.clb"));

        var inProfile = new LaserProfile(LaserType.Fiber, 100, 210, 30);
        var outProfile = new LaserProfile(LaserType.Fiber, 100, 210, 100);

        var converted = source.Convert(inProfile, outProfile);
        var converted100W = LightBurnLibrary.Load(SamplePath("210-290-Conversion.clb"));

        var ourFirst = converted.Document
            .SelectSingleNode("//LightBurnLibrary/Material/Entry/CutSetting")!;
        var refFirst = converted100W.Document
            .SelectSingleNode("//LightBurnLibrary/Material/Entry/CutSetting")!;

        Assert.Equal(
            refFirst.SelectSingleNode("./minPower")!.Attributes!["Value"]!.Value,
            ourFirst.SelectSingleNode("./minPower")!.Attributes!["Value"]!.Value);
        Assert.Equal(
            refFirst.SelectSingleNode("./maxPower")!.Attributes!["Value"]!.Value,
            ourFirst.SelectSingleNode("./maxPower")!.Attributes!["Value"]!.Value);
    }

    [Fact]
    public void Convert_Does_Not_Mutate_Source_Library()
    {
        var source = LightBurnLibrary.Load(SamplePath("30W_210mm_LASER-SECRETS_1.clb"));
        var originalXml = source.ToXmlString();

        var inProfile = new LaserProfile(LaserType.Fiber, 100, 210, 30);
        var outProfile = new LaserProfile(LaserType.Fiber, 100, 290, 30);

        _ = source.Convert(inProfile, outProfile);
        Assert.Equal(originalXml, source.ToXmlString());
    }
}
