namespace AsciiArt.Core;

/// <summary>
/// Defines a font that can render characters as fixed-height ASCII glyphs.
/// </summary>
public interface IAsciiFont
{
    /// <summary>
    /// Gets the display name of the font.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the fixed number of lines per glyph.
    /// </summary>
    int Height { get; }

    /// <summary>
    /// Gets the placeholder character used when input is not supported.
    /// </summary>
    char PlaceholderChar { get; }

    /// <summary>
    /// Determines whether the font supports a character.
    /// </summary>
    /// <param name="character">Character to test.</param>
    /// <returns><see langword="true"/> when supported.</returns>
    bool Supports(char character);

    /// <summary>
    /// Gets the glyph lines for a character. The returned array must match <see cref="Height"/>.
    /// </summary>
    /// <param name="character">Character to render.</param>
    /// <returns>Glyph lines.</returns>
    string[] GetGlyph(char character);
}
