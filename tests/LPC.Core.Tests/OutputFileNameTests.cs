/*
 * This file is part of LaserParamsConverter (cross-platform fork).
 * Licensed under the GNU General Public License, version 3.
 */

using LPC.Core.Models;
using LPC.Core.Naming;
using Xunit;

namespace LPC.Core.Tests;

public class OutputFileNameTests
{
    private static readonly LaserProfile Target =
        new(LaserType.Fiber, MaxPower: 100, Lens: 290, Wattage: 30);

    [Fact]
    public void Rewrites_Lens_Token_When_Wattage_Matches_Source()
    {
        var input = @"C:\stuff\30W_210mm_LASER-SECRETS_1.clb";
        var output = OutputFileName.ForConvertedLibrary(input, Target);
        Assert.Equal(@"C:\stuff\30W_290mm_LASER-SECRETS_1.clb", output);
    }

    [Fact]
    public void Rewrites_Both_Wattage_And_Lens_When_Different()
    {
        var input = @"C:\stuff\50W_100mm_library.clb";
        var output = OutputFileName.ForConvertedLibrary(input, Target);
        Assert.Equal(@"C:\stuff\30W_290mm_library.clb", output);
    }

    [Fact]
    public void Appends_Profile_When_Source_Has_Neither_Token()
    {
        var input = @"C:\stuff\library.clb";
        var output = OutputFileName.ForConvertedLibrary(input, Target);
        Assert.Equal(@"C:\stuff\library_30W_290mm.clb", output);
    }

    [Fact]
    public void Rewrites_Display_Name_Tokens()
    {
        Assert.Equal(
            "30W_290mm_LASER-SECRETS",
            OutputFileName.ForDisplayName("30W_210mm_LASER-SECRETS", Target));
    }

    [Fact]
    public void Display_Name_Falls_Back_When_Source_Is_Empty()
    {
        Assert.Equal("30W_290mm", OutputFileName.ForDisplayName(string.Empty, Target));
    }

    [Fact]
    public void Is_Case_Insensitive_On_W_And_Mm()
    {
        var input = @"C:\stuff\30w_210MM_library.clb";
        var output = OutputFileName.ForConvertedLibrary(input, Target);
        Assert.Equal(@"C:\stuff\30W_290mm_library.clb", output);
    }
}
