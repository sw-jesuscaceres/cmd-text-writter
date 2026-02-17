using AsciiArt.Core;

namespace AsciiArt.Fonts;

/// <summary>
/// Built-in compact block font with support for A-Z, 0-9, space, and placeholder.
/// </summary>
public sealed class BasicBlockFont : IAsciiFont
{
    private static readonly IReadOnlyDictionary<char, string[]> Glyphs = BuildGlyphs();

    /// <inheritdoc />
    public string Name => "basicblock";

    /// <inheritdoc />
    public int Height => 5;

    /// <inheritdoc />
    public char PlaceholderChar => '?';

    /// <inheritdoc />
    public bool Supports(char character)
    {
        return Glyphs.ContainsKey(Normalize(character));
    }

    /// <inheritdoc />
    public string[] GetGlyph(char character)
    {
        var normalized = Normalize(character);
        if (Glyphs.TryGetValue(normalized, out var glyph))
        {
            return glyph;
        }

        return Glyphs[PlaceholderChar];
    }

    private static char Normalize(char character)
    {
        return char.IsLetter(character) ? char.ToUpperInvariant(character) : character;
    }

    private static IReadOnlyDictionary<char, string[]> BuildGlyphs()
    {
        var glyphs = new Dictionary<char, string[]>();
        for (var character = 'A'; character <= 'Z'; character++)
        {
            glyphs[character] = CreateGlyph(character);
        }

        for (var character = '0'; character <= '9'; character++)
        {
            glyphs[character] = CreateGlyph(character);
        }

        glyphs[' '] = new[] { "  ", "  ", "  ", "  ", "  " };
        glyphs['?'] = CreateGlyph('?');

        return glyphs;
    }

    private static string[] CreateGlyph(char token)
    {
        return new[]
        {
            "##",
            $"{token} ",
            $"{token} ",
            $"{token} ",
            "##",
        };
    }
}
