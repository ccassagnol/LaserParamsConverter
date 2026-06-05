/*
 * This file is part of LaserParamsConverter (cross-platform fork).
 * Licensed under the GNU General Public License, version 3.
 */

using LPC.Core.Conversion;
using LPC.Core.Models;
using Xunit;

namespace LPC.Core.Tests;

public class LaserParamScalerTests
{
    private static readonly LaserProfile Source =
        new(LaserType.Fiber, MaxPower: 100, Lens: 210, Wattage: 30);

    private static readonly LaserProfile Target290Mm =
        new(LaserType.Fiber, MaxPower: 100, Lens: 290, Wattage: 30);

    [Fact]
    public void Co2_Ignores_Lens_On_Both_Sides()
    {
        var inProfile = new LaserProfile(LaserType.Co2, 100, 50, 60);
        var outProfile = new LaserProfile(LaserType.Co2, 100, 999, 80);

        // For CO2 the lens is forced to 1. With identical lenses, the only
        // scaling is wattage: outPower = inPower * (60 / 80).
        var (speed, power) = LaserParamScaler.Scale(1000, 80, inProfile, outProfile);

        Assert.Equal(1000, speed);
        Assert.Equal(60, power);
    }

    [Fact]
    public void Fiber_Scales_Power_Up_When_Lens_Grows()
    {
        // 5W input at 210mm -> at 290mm with same wattage, outPower ~= 5 * (290/210) ~= 6.
        var (_, power) = LaserParamScaler.Scale(1000, 5, Source, Target290Mm);
        Assert.Equal(6, power);
    }

    [Fact]
    public void Fiber_Slows_Speed_When_Power_Would_Clip_Above_MaxPower()
    {
        // Force a clip: input maxPower 100, very high source power so out > max.
        var lowCapTarget = new LaserProfile(LaserType.Fiber, MaxPower: 10, Lens: 290, Wattage: 30);
        var (speed, power) = LaserParamScaler.Scale(1000, 100, Source, lowCapTarget);

        Assert.Equal(10, power);                 // clipped to MaxPower
        Assert.True(speed < 1000, "Speed should be reduced when power clips.");
    }
}
