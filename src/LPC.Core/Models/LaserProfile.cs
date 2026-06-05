/*
 * This file is part of LaserParamsConverter (cross-platform fork).
 * Copyright (c) 2022 David Christian (upstream).
 * Copyright (c) 2026 Charles Cassagnol (fork).
 *
 * Licensed under the GNU General Public License, version 3.
 * See LICENSE in the repository root, or <http://www.gnu.org/licenses/>.
 */

namespace LPC.Core.Models;

/// <summary>
/// Identity of a laser/lens/wattage combination. Used as both the source and
/// destination descriptor for a conversion.
/// </summary>
public sealed record LaserProfile(
    LaserType Laser,
    int MaxPower,
    int Lens,
    int Wattage);
