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
            "  --strict              Fail if unsupported characters are found",
            "  --list-fonts          List available fonts",
            string.Empty,
            "Constraints:",
            $"  Output width <= {AsciiRenderer.MaxOutputWidth} characters",
            $"  Output height <= {AsciiRenderer.MaxOutputHeight} lines",
            string.Empty,
            "Examples:",
            "  asciiart \"Hello\"",
            "  asciiart --help");
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
