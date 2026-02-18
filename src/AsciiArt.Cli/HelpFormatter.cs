using AsciiArt.Core;

namespace AsciiArt.Cli;

/// <summary>
/// Builds help and error messages for the CLI.
/// </summary>
public static class HelpFormatter
{
    /// <summary>
    /// Gets the short usage string.
    /// </summary>
    public static string Usage => "Usage: asciiart [options] <text>";

    /// <summary>
    /// Builds the full help message.
    /// </summary>
    /// <returns>Help text.</returns>
    public static string BuildHelp()
    {
        return string.Join(
            Environment.NewLine,
            Usage,
            string.Empty,
            "Description:",
            "  Transform text into ASCII art banners for terminal display.",
            string.Empty,
            "Arguments:",
            "  text                  The text to convert to ASCII art (max 40 characters)",
            string.Empty,
            "Options:",
            "  --help, -h            Show this help message and exit",
            "  --font <name>         Choose a built-in font",
            "  --color <name>        Apply a color to the output (use --color help for details)",
            "  --strict              Fail if unsupported characters are found",
            "  --list-fonts          List available fonts",
            string.Empty,
            "Constraints:",
            $"  Output width <= {AsciiRenderer.MaxOutputWidth} characters",
            $"  Output height <= {AsciiRenderer.MaxOutputHeight} lines",
            string.Empty,
            "Examples:",
            "  asciiart \"Hello\"",
            "  asciiart --color green \"Hello\"",
            "  asciiart --help");
    }

    /// <summary>
    /// Builds the color help message.
    /// </summary>
    /// <returns>Color help text.</returns>
    public static string BuildColorHelp()
    {
        return string.Join(
            Environment.NewLine,
            "Available Colors:",
            string.Empty,
            "  red      - Bright red (⚠️  accessibility note: difficult for some colorblind users)",
            "  green    - Bright green (⚠️  accessibility note: difficult for some colorblind users)",
            "  blue     - Bright blue",
            "  yellow   - Bright yellow",
            "  magenta  - Bright magenta",
            "  cyan     - Bright cyan (✓ recommended for colorblind accessibility)",
            "  white    - Bright white (✓ recommended for colorblind accessibility)",
            "  black    - Black text (on suitable backgrounds)",
            string.Empty,
            "Usage:",
            "  asciiart --color <name> \"Your text\"",
            "  asciiart -c <name> \"Your text\"",
            string.Empty,
            "Accessibility:",
            "  For colorblind-friendly output, prefer: cyan, yellow, white",
            "  The application will warn you if you choose red or green.",
            string.Empty,
            "Examples:",
            "  asciiart --color red \"Hello\"",
            "  asciiart -c cyan \"World\"",
            "  asciiart --color help");
    }

    /// <summary>
    /// Builds a formatted error message.
    /// </summary>
    /// <param name="message">Error detail.</param>
    /// <returns>Formatted error text.</returns>
    public static string BuildError(string message)
    {
        return string.Join(
            Environment.NewLine,
            $"Error: {message}",
            string.Empty,
            Usage,
            "Use --help for more information.");
    }
}
