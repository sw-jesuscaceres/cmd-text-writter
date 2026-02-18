using AsciiArt.Core;

namespace AsciiArt.Cli.ColorSupport;

/// <summary>
/// Generates ANSI escape codes for terminal colors.
/// </summary>
public static class ColorCodeGenerator
{
    /// <summary>
    /// ANSI reset code (returns to default terminal color).
    /// </summary>
    public const string Reset = "\x1b[39m";

    /// <summary>
    /// Gets the ANSI escape code for the specified color.
    /// </summary>
    /// <param name="color">The color to generate a code for.</param>
    /// <returns>The ANSI escape code string.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if color is not a valid ColorOption value.</exception>
    public static string GetAnsiCode(ColorOption color)
    {
        return color switch
        {
            ColorOption.Red => "\x1b[31m",
            ColorOption.Green => "\x1b[32m",
            ColorOption.Blue => "\x1b[34m",
            ColorOption.Yellow => "\x1b[33m",
            ColorOption.Magenta => "\x1b[35m",
            ColorOption.Cyan => "\x1b[36m",
            ColorOption.White => "\x1b[37m",
            ColorOption.Black => "\x1b[30m",
            _ => throw new ArgumentOutOfRangeException(nameof(color), color, "Invalid color option.")
        };
    }
}
