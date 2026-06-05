/*
 * This file is part of LaserParamsConverter (cross-platform fork).
 * Copyright (c) 2022 David Christian (upstream).
 * Copyright (c) 2026 Charles Cassagnol (fork).
 *
 * Licensed under the GNU General Public License, version 3.
 * See LICENSE in the repository root, or <http://www.gnu.org/licenses/>.
 *
 * LightBurn .clb library reader / writer / converter. Ported from the upstream
 * LaserLibrary.LoadLightBurn / SaveLightBurn / ConvertLightBurn paths.
 */

using System.Globalization;
using System.Text;
using System.Xml;
using LPC.Core.Conversion;
using LPC.Core.Models;

namespace LPC.Core.Formats;

/// <summary>LightBurn (.clb) parameter library — load, convert, save.</summary>
public sealed class LightBurnLibrary
{
    private const string RootName = "LightBurnLibrary";

    /// <summary>The underlying XML document.</summary>
    public XmlDocument Document { get; private set; } = new();

    /// <summary>The <c>DisplayName</c> attribute on the root element. May be empty.</summary>
    public string DisplayName
    {
        get => Document.DocumentElement?.GetAttribute("DisplayName") ?? string.Empty;
        set => Document.DocumentElement?.SetAttribute("DisplayName", value);
    }

    /// <summary>Count of <c>Material</c> elements (for sanity checks / tests).</summary>
    public int MaterialCount
        => Document.SelectNodes($"//{RootName}/Material")?.Count ?? 0;

    /// <summary>Count of <c>Entry</c> elements (for sanity checks / tests).</summary>
    public int EntryCount
        => Document.SelectNodes($"//{RootName}/Material/Entry")?.Count ?? 0;

    /// <summary>Load a LightBurn .clb file from disk.</summary>
    public static LightBurnLibrary Load(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        var lib = new LightBurnLibrary();
        lib.Document.Load(path);
        if (lib.Document.DocumentElement?.Name != RootName)
        {
            throw new InvalidDataException(
                $"File '{path}' is not a LightBurn library (root element is "
                + $"'{lib.Document.DocumentElement?.Name}', expected '{RootName}').");
        }
        return lib;
    }

    /// <summary>Load from an in-memory XML string (used by tests).</summary>
    public static LightBurnLibrary Parse(string xml)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(xml);
        var lib = new LightBurnLibrary();
        lib.Document.LoadXml(xml);
        return lib;
    }

    /// <summary>Save the current document to disk as UTF-8 XML.</summary>
    public void Save(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        Document.Save(path);
    }

    /// <summary>
    /// Convert all entries in this library from <paramref name="input"/> to
    /// <paramref name="output"/> profile. Returns a new instance; the source
    /// library is left untouched.
    /// </summary>
    public LightBurnLibrary Convert(
        LaserProfile input,
        LaserProfile output,
        ConversionOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(output);
        options ??= ConversionOptions.Default;

        var clone = new LightBurnLibrary
        {
            Document = (XmlDocument)Document.Clone()
        };

        var entries = clone.Document.SelectNodes($"//{RootName}/Material/Entry");
        if (entries is null)
        {
            return clone;
        }

        foreach (XmlNode entry in entries)
        {
            ApplyAdvancedOverrides(entry, options);
            ScaleSpeedAndPower(entry, input, output);
        }

        return clone;
    }

    private static void ApplyAdvancedOverrides(XmlNode entry, ConversionOptions options)
    {
        if (!options.SetPulseWidth)
        {
            return;
        }

        // Upstream applied this against the first CutSetting child.
        var cutSetting = entry.FirstChild;
        if (cutSetting is null)
        {
            return;
        }

        SetOrCreateValuedChild(
            cutSetting,
            "QPulseWidth",
            options.PulseWidth.ToString(CultureInfo.InvariantCulture));
    }

    private static void ScaleSpeedAndPower(
        XmlNode entry,
        LaserProfile input,
        LaserProfile output)
    {
        var speedNode = entry.SelectSingleNode("./CutSetting/speed");
        var minPowerNode = entry.SelectSingleNode("./CutSetting/minPower");
        var maxPowerNode = entry.SelectSingleNode("./CutSetting/maxPower");

        if (speedNode is null || minPowerNode is null || maxPowerNode is null)
        {
            return;
        }

        var speed = ParseRoundedInt(GetValueAttribute(speedNode));
        var minPower = ParseRoundedInt(GetValueAttribute(minPowerNode));
        var maxPower = ParseRoundedInt(GetValueAttribute(maxPowerNode));

        var (_, scaledMinPower) = LaserParamScaler.Scale(speed, minPower, input, output);
        var (scaledSpeed, scaledMaxPower) =
            LaserParamScaler.Scale(speed, maxPower, input, output);

        SetValueAttribute(speedNode, scaledSpeed.ToString(CultureInfo.InvariantCulture));
        SetValueAttribute(minPowerNode, scaledMinPower.ToString(CultureInfo.InvariantCulture));
        SetValueAttribute(maxPowerNode, scaledMaxPower.ToString(CultureInfo.InvariantCulture));
    }

    private static string GetValueAttribute(XmlNode node)
        => node.Attributes?["Value"]?.Value
           ?? throw new InvalidDataException(
               $"Element <{node.Name}> is missing the required 'Value' attribute.");

    private static void SetValueAttribute(XmlNode node, string value)
    {
        var attr = node.Attributes?["Value"]
            ?? throw new InvalidDataException(
                $"Element <{node.Name}> is missing the required 'Value' attribute.");
        attr.Value = value;
    }

    private static void SetOrCreateValuedChild(XmlNode parent, string name, string value)
    {
        var existing = parent.SelectSingleNode($".//{name}");
        if (existing is XmlElement el)
        {
            el.SetAttribute("Value", value);
            return;
        }

        var doc = parent.OwnerDocument
            ?? throw new InvalidOperationException("Parent node is detached from any document.");
        var child = doc.CreateElement(name);
        child.SetAttribute("Value", value);
        parent.AppendChild(child);
    }

    private static int ParseRoundedInt(string value)
    {
        var f = float.Parse(
            value,
            NumberStyles.Float | NumberStyles.AllowExponent,
            CultureInfo.InvariantCulture);
        return (int)Math.Round(f);
    }

    /// <summary>Convenience: serialize current document to a string (test helper).</summary>
    public string ToXmlString()
    {
        var sb = new StringBuilder();
        using var writer = XmlWriter.Create(sb, new XmlWriterSettings
        {
            Indent = true,
            Encoding = Encoding.UTF8,
            OmitXmlDeclaration = false
        });
        Document.Save(writer);
        return sb.ToString();
    }
}
