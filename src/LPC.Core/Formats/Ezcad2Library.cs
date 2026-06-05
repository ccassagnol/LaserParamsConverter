/*
 * This file is part of LaserParamsConverter (cross-platform fork).
 * Copyright (c) 2022 David Christian (upstream).
 * Copyright (c) 2026 Charles Cassagnol (fork).
 *
 * Licensed under the GNU General Public License, version 3.
 * See LICENSE in the repository root, or <http://www.gnu.org/licenses/>.
 *
 * EZCAD2 .lib parser stub. The upstream parser reads an INI-like format:
 *
 *   [MATERIAL_NAME]
 *   LOOP=1.000000E+000
 *   MARKSPEED=1.000000E+003
 *   POWERRATIO=5.000000E+001
 *   FREQ=2.000000E+004
 *   ...
 *
 * One materal block per group; one CutSetting per material. Real samples are
 * required before this parser is implemented — see legacy/winforms/LaserLibrary.cs
 * methods LoadEZCAD / SaveEZCAD / ConvertEZCAD for the reference logic.
 */

using LPC.Core.Models;

namespace LPC.Core.Formats;

/// <summary>EZCAD2 (.lib) parameter library — not yet implemented in the cross-platform port.</summary>
public sealed class Ezcad2Library
{
    /// <summary>Always throws <see cref="NotImplementedException"/> until sample files are provided.</summary>
    public static Ezcad2Library Load(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        throw new NotImplementedException(
            "EZCAD2 (.lib) support is not yet implemented in the cross-platform port. "
            + "Provide sample .lib files and the reference upstream parser at "
            + "legacy/winforms/LaserLibrary.cs (LoadEZCAD) will be ported with "
            + "golden-file tests.");
    }

    /// <summary>Always throws <see cref="NotImplementedException"/> until sample files are provided.</summary>
    public Ezcad2Library Convert(LaserProfile input, LaserProfile output, ConversionOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(output);
        _ = options;
        throw new NotImplementedException(
            "EZCAD2 conversion is not yet implemented. See the docstring on Load() for next steps.");
    }

    /// <summary>Always throws <see cref="NotImplementedException"/> until sample files are provided.</summary>
    public void Save(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        throw new NotImplementedException("EZCAD2 save is not yet implemented.");
    }
}
