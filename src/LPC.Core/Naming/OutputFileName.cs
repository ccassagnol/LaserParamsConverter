/*
 * This file is part of LaserParamsConverter (cross-platform fork).
 * Copyright (c) 2026 Charles Cassagnol.
 *
 * Licensed under the GNU General Public License, version 3.
 * See LICENSE in the repository root, or <http://www.gnu.org/licenses/>.
 *
 * Naming helper: rewrites a source filename / DisplayName so the converted
 * output reflects the new wattage and lens instead of the original values.
 *
 * Upstream bug being fixed: a 210mm -> 290mm conversion was being saved as
 * "30W_210mm_LASER-SECRETS_1.clb" with DisplayName="30W_210mm_LASER-SECRETS"
 * even though the contents were now 290mm-scaled.
 */

using System.Globalization;
using System.Text.RegularExpressions;
using LPC.Core.Models;

namespace LPC.Core.Naming;

/// <summary>
/// Generates output filenames and embedded <c>DisplayName</c> values that
/// reflect a converted library's destination profile.
/// </summary>
public static class OutputFileName
{
    // Matches "30W", "150W", "1500W" anywhere in a name.
    private static readonly Regex WattagePattern = new(
        @"(?<![A-Za-z0-9])(\d{1,5})W(?![A-Za-z0-9])",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    // Matches "210mm", "100mm", "1200mm" anywhere in a name.
    private static readonly Regex LensPattern = new(
        @"(?<![A-Za-z0-9])(\d{1,5})mm(?![A-Za-z0-9])",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    /// <summary>
    /// Compute a filename for a converted library by substituting any wattage
    /// and lens tokens found in the source filename with the destination
    /// profile's values. If a token is absent, it is appended as a suffix
    /// (before the extension).
    /// </summary>
    public static string ForConvertedLibrary(
        string sourcePath,
        LaserProfile output)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sourcePath);
        ArgumentNullException.ThrowIfNull(output);

        var directory = Path.GetDirectoryName(sourcePath) ?? string.Empty;
        var stem = Path.GetFileNameWithoutExtension(sourcePath);
        var extension = Path.GetExtension(sourcePath);

        var newStem = RewriteTokens(stem, output);

        return string.IsNullOrEmpty(directory)
            ? newStem + extension
            : Path.Combine(directory, newStem + extension);
    }

    /// <summary>
    /// Rewrite an embedded <c>DisplayName</c> string to match the destination
    /// profile, using the same wattage / lens token substitution as the
    /// filename.
    /// </summary>
    public static string ForDisplayName(string sourceDisplayName, LaserProfile output)
    {
        ArgumentNullException.ThrowIfNull(output);
        if (string.IsNullOrEmpty(sourceDisplayName))
        {
            return $"{output.Wattage}W_{output.Lens}mm";
        }
        return RewriteTokens(sourceDisplayName, output);
    }

    private static string RewriteTokens(string input, LaserProfile output)
    {
        var wattageToken = output.Wattage.ToString(CultureInfo.InvariantCulture) + "W";
        var lensToken = output.Lens.ToString(CultureInfo.InvariantCulture) + "mm";

        var hadWattage = WattagePattern.IsMatch(input);
        var hadLens = LensPattern.IsMatch(input);

        var result = input;
        if (hadWattage)
        {
            result = WattagePattern.Replace(result, wattageToken);
        }
        if (hadLens)
        {
            result = LensPattern.Replace(result, lensToken);
        }

        // If neither token was present in the source, append both so the
        // output filename at least identifies the destination profile.
        if (!hadWattage && !hadLens)
        {
            result = $"{result}_{wattageToken}_{lensToken}";
        }
        else if (!hadWattage)
        {
            result = $"{wattageToken}_{result}";
        }
        else if (!hadLens)
        {
            result = $"{result}_{lensToken}";
        }

        return result;
    }
}
