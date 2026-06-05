/*
 * This file is part of LaserParamsConverter (cross-platform fork).
 * Copyright (c) 2022 David Christian (upstream).
 * Copyright (c) 2026 Charles Cassagnol (fork).
 *
 * Licensed under the GNU General Public License, version 3.
 * See LICENSE in the repository root, or <http://www.gnu.org/licenses/>.
 *
 * EZCAD3 .ini parser stub. Same INI-like shape as EZCAD2 but uses FREQF
 * instead of FREQ and writes values in fixed-decimal "0.000000" format
 * rather than the EZCAD2 "E6" scientific format. Real samples are required
 * before this parser is implemented — see legacy/winforms/LaserLibrary.cs
 * methods LoadEZCAD / SaveEZCAD / ConvertEZCAD for the reference logic.
 */

using LPC.Core.Models;

namespace LPC.Core.Formats;

/// <summary>EZCAD3 (.ini) parameter library — not yet implemented in the cross-platform port.</summary>
public sealed class Ezcad3Library
{
    /// <summary>Always throws <see cref="NotImplementedException"/> until sample files are provided.</summary>
    public static Ezcad3Library Load(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        throw new NotImplementedException(
            "EZCAD3 (.ini) support is not yet implemented in the cross-platform port. "
            + "Provide sample .ini files and the reference upstream parser at "
            + "legacy/winforms/LaserLibrary.cs (LoadEZCAD) will be ported with "
            + "golden-file tests.");
    }

    /// <summary>Always throws <see cref="NotImplementedException"/> until sample files are provided.</summary>
    public Ezcad3Library Convert(LaserProfile input, LaserProfile output, ConversionOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(output);
        _ = options;
        throw new NotImplementedException(
            "EZCAD3 conversion is not yet implemented. See the docstring on Load() for next steps.");
    }

    /// <summary>Always throws <see cref="NotImplementedException"/> until sample files are provided.</summary>
    public void Save(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        throw new NotImplementedException("EZCAD3 save is not yet implemented.");
    }
}
