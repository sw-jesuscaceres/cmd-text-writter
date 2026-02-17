using AsciiArt.Core;

namespace AsciiArt.Fonts;

/// <summary>
/// FIGlet font based on Big Money-ne.
/// </summary>
public sealed class BigMoneyNeFont : IAsciiFont
{
    private static readonly ParsedFont Parsed = FigletFontParser.Parse(LoadFontText());

    /// <inheritdoc />
    public string Name => "big-money-ne";

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
        const string resourceName = "AsciiArt.Fonts.Resources.BigMoney-ne.flf";
        using var stream = typeof(BigMoneyNeFont).Assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Embedded font resource '{resourceName}' was not found.");

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
