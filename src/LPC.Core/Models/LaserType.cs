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
/// Physical laser type. Lens scaling only applies to fiber lasers; CO2 gantry
/// lasers ignore lens parameters (the original implementation forces inLens =
/// outLens = 1 for CO2).
/// </summary>
public enum LaserType
{
    Co2,
    Fiber
}
