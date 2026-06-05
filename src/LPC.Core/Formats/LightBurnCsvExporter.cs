/*
 * This file is part of LaserParamsConverter (cross-platform fork).
 * Copyright (c) 2022 David Christian (upstream).
 * Copyright (c) 2026 Charles Cassagnol (fork).
 *
 * Licensed under the GNU General Public License, version 3.
 * See LICENSE in the repository root, or <http://www.gnu.org/licenses/>.
 *
 * CSV export for LightBurn libraries. Ported from upstream
 * LaserLibrary.LightBurnSaveAsCSV. Columns and mode mapping match the
 * upstream output byte-for-byte so existing Excel workflows keep working.
 */

using System.Globalization;
using System.Text;
using System.Xml;

namespace LPC.Core.Formats;

/// <summary>CSV exporter for a LightBurn library.</summary>
public static class LightBurnCsvExporter
{
    private const string Header = "Name,Pass,Speed,Power,Freq (KHz),Mode,Interval";

    /// <summary>Render the supplied LightBurn library as CSV text (UTF-8).</summary>
    public static string ToCsv(LightBurnLibrary library)
    {
        ArgumentNullException.ThrowIfNull(library);

        var sb = new StringBuilder();
        sb.AppendLine(Header);

        var materials = library.Document.SelectNodes("//LightBurnLibrary/Material");
        if (materials is null)
        {
            return sb.ToString();
        }

        foreach (XmlNode material in materials)
        {
            var materialName = material.Attributes?[0]?.Value ?? string.Empty;
            var entries = material.SelectNodes(".//Entry");
            if (entries is null)
            {
                continue;
            }

            foreach (XmlNode entry in entries)
            {
                var noThickTitle = GetAttr(entry, "NoThickTitle", string.Empty);
                var desc = GetAttr(entry, "Desc", string.Empty);

                var loop = GetChildValue(entry, ".//CutSetting/numPasses", "1");
                var speed = GetChildValue(entry, ".//CutSetting/speed", "0");
                var power = GetChildValue(entry, ".//CutSetting/maxPower", "0");
                var freq = GetChildValue(entry, ".//CutSetting/frequency", "0");
                var interval = GetChildValue(entry, ".//CutSetting/interval", "0.000");

                var cutSettingType = entry.SelectSingleNode(".//CutSetting")?
                    .Attributes?["type"]?.Value ?? "Scan";
                var mode = cutSettingType switch
                {
                    "Scan" => "Fill",
                    "Cut" => "Line",
                    _ => "Offset Fill"
                };

                var freqKhz = (ParseRoundedInt(freq) / 1000)
                    .ToString(CultureInfo.InvariantCulture);

                var displayName = $"{materialName} {noThickTitle} {desc}"
                    .Replace("\"", "\"\"", StringComparison.Ordinal);

                sb.Append('"')
                  .Append(displayName)
                  .Append("\",")
                  .Append(loop).Append(',')
                  .Append(speed).Append(',')
                  .Append(power).Append(',')
                  .Append(freqKhz).Append(',')
                  .Append(mode).Append(',')
                  .AppendLine(interval);
            }
        }

        return sb.ToString();
    }

    private static string GetAttr(XmlNode node, string attribute, string defaultValue)
        => node.Attributes?[attribute]?.Value ?? defaultValue;

    private static string GetChildValue(XmlNode node, string xpath, string defaultValue)
        => node.SelectSingleNode(xpath)?.Attributes?[0]?.Value ?? defaultValue;

    private static int ParseRoundedInt(string value)
    {
        var f = float.Parse(
            value,
            NumberStyles.Float | NumberStyles.AllowExponent,
            CultureInfo.InvariantCulture);
        return (int)Math.Round(f);
    }
}
