using AsciiArt.Core;

namespace AsciiArt.Fonts;

/// <summary>
/// FIGlet font based on Caligraphy.
/// </summary>
public sealed class CaligraphyFont : IAsciiFont
{
    private static readonly ParsedFont Parsed = FigletFontParser.Parse(LoadFontText());

    /// <inheritdoc />
    public string Name => "caligraphy";

    /// <inheritdoc />
    public int Height => Parsed.Height;

    /// <inheritdoc />
    public char PlaceholderChar => '?';

    /// <inheritdoc />
    public bool Supports(char character)
    {
        return Parsed.Glyphs.ContainsKey(character);
    }

    /// <inheritdoc />
    public string[] GetGlyph(char character)
    {
        if (Parsed.Glyphs.TryGetValue(character, out var glyph))
        {
            return glyph;
        }

        return Parsed.Glyphs[PlaceholderChar];
    }

    private static string LoadFontText()
    {
        const string resourceName = "AsciiArt.Fonts.Resources.Caligraphy.flf";
        using var stream = typeof(CaligraphyFont).Assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Embedded font resource '{resourceName}' was not found.");

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
