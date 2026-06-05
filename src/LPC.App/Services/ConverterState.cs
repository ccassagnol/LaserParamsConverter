/*
 * This file is part of LaserParamsConverter (cross-platform fork).
 * Copyright (c) 2026 Charles Cassagnol.
 *
 * Licensed under the GNU General Public License, version 3.
 * See LICENSE in the repository root, or <http://www.gnu.org/licenses/>.
 *
 * Process-scoped state shared across Razor pages. Keeps the user's source +
 * target profiles and advanced options in one place so the Convert,
 * ConvertOne, and Advanced pages stay in sync without prop drilling.
 */

using LPC.Core.Models;

namespace LPC.App.Services;

/// <summary>App-wide singleton holding the user's current conversion inputs.</summary>
public sealed class ConverterState
{
    public LaserType SourceLaser { get; set; } = LaserType.Fiber;
    public int SourceWattage { get; set; } = 30;
    public int SourceLens { get; set; } = 210;

    public LaserType TargetLaser { get; set; } = LaserType.Fiber;
    public int TargetMaxPower { get; set; } = 100;
    public int TargetWattage { get; set; } = 30;
    public int TargetLens { get; set; } = 290;

    public ConversionOptions AdvancedOptions { get; set; } = ConversionOptions.Default;

    public LaserProfile SourceProfile =>
        new(SourceLaser, MaxPower: 100, Lens: SourceLens, Wattage: SourceWattage);

    public LaserProfile TargetProfile =>
        new(TargetLaser, MaxPower: TargetMaxPower, Lens: TargetLens, Wattage: TargetWattage);
}
