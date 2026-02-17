using AsciiArt.Core;

namespace AsciiArt.Fonts;

/// <summary>
/// Provides lookup and listing of available fonts.
/// </summary>
public static class FontRegistry
{
    private static readonly Dictionary<string, IAsciiFont> Fonts = new(StringComparer.OrdinalIgnoreCase)
    {
        ["big-money-ne"] = new BigMoneyNeFont(),
        ["basicblock"] = new BasicBlockFont(),
    };

    /// <summary>
    /// Gets the default font.
    /// </summary>
    public static IAsciiFont DefaultFont => Fonts["big-money-ne"];

    /// <summary>
    /// Gets a font by name.
    /// </summary>
    /// <param name="name">Font name.</param>
    /// <returns>The font when found; otherwise, <see langword="null"/>.</returns>
    public static IAsciiFont? GetFont(string name)
    {
        return Fonts.TryGetValue(name, out var font) ? font : null;
    }

    /// <summary>
    /// Returns all registered fonts.
    /// </summary>
    /// <returns>Registered fonts.</returns>
    public static IReadOnlyCollection<IAsciiFont> ListFonts()
    {
        return Fonts.Values.ToArray();
    }
}
