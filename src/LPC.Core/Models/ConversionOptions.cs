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
/// Optional advanced overrides applied during a conversion.
/// Currently mirrors the upstream "Advanced" form: fixed Q-Pulse Width.
/// </summary>
public sealed record ConversionOptions
{
    /// <summary>If <c>true</c>, override <c>QPulseWidth</c> with <see cref="PulseWidth"/>.</summary>
    public bool SetPulseWidth { get; init; }

    /// <summary>Pulse width (only used when <see cref="SetPulseWidth"/> is true).</summary>
    public decimal PulseWidth { get; init; }

    public static ConversionOptions Default { get; } = new();
}
