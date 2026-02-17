using AsciiArt.Core;

namespace AsciiArt.Core.Tests.Fixtures;

internal sealed class MockFont : IAsciiFont
{
    private static readonly IReadOnlyDictionary<char, string[]> Glyphs = new Dictionary<char, string[]>
    {
        ['A'] = new[] { "A1", "A2", "A3" },
        ['B'] = new[] { "B1", "B2", "B3" },
        ['?'] = new[] { "?1", "?2", "?3" },
        [' '] = new[] { "  ", "  ", "  " },
    };

    public string Name => "mock";

    public int Height => 3;

    public char PlaceholderChar => '?';

    public bool Supports(char character)
    {
        var normalized = char.IsLetter(character) ? char.ToUpperInvariant(character) : character;
        return Glyphs.ContainsKey(normalized);
    }

    public string[] GetGlyph(char character)
    {
        var normalized = char.IsLetter(character) ? char.ToUpperInvariant(character) : character;
        return Glyphs.TryGetValue(normalized, out var glyph) ? glyph : Glyphs[PlaceholderChar];
    }
}
