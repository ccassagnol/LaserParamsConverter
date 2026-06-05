/*
 * This file is part of LaserParamsConverter (cross-platform fork).
 * Copyright (c) 2022 David Christian (upstream).
 * Copyright (c) 2026 Charles Cassagnol (fork).
 *
 * Licensed under the GNU General Public License, version 3.
 * See LICENSE in the repository root, or <http://www.gnu.org/licenses/>.
 *
 * Port of LaserParam.Convert from upstream LaserLibrary.cs. The arithmetic is
 * preserved exactly so output remains bit-compatible with the original tool.
 */

using LPC.Core.Models;

namespace LPC.Core.Conversion;

/// <summary>Pure speed/power scaling between two laser profiles.</summary>
public static class LaserParamScaler
{
    /// <summary>
    /// Scales a (speed, power) pair from the input profile to the output profile.
    /// </summary>
    /// <remarks>
    /// Mirrors the upstream algorithm exactly:
    /// <code>
    ///   powerMod = (power * outLens) / inLens
    ///   outPower = (inWatts * powerMod) / outWatts
    ///   outSpeed = inSpeed
    ///   if outPower > maxPower:
    ///       speedMod = power / outPower
    ///       outSpeed = inSpeed * speedMod
    ///       outPower = maxPower
    /// </code>
    /// For <see cref="LaserType.Co2"/>, lens is forced to 1 on both sides.
    /// </remarks>
    public static (int Speed, int Power) Scale(
        int inSpeed,
        int inPower,
        LaserProfile input,
        LaserProfile output)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(output);

        var inLens = output.Laser == LaserType.Co2 ? 1 : input.Lens;
        var outLens = output.Laser == LaserType.Co2 ? 1 : output.Lens;

        var powerMod = ((decimal)inPower * outLens) / inLens;
        var outPower = decimal.ToInt32((input.Wattage * powerMod) / output.Wattage);
        var outSpeed = inSpeed;

        if (outPower > output.MaxPower)
        {
            var speedMod = (decimal)inPower / outPower;
            outSpeed = decimal.ToInt32(inSpeed * speedMod);
            outPower = output.MaxPower;
        }

        return (outSpeed, outPower);
    }
}
