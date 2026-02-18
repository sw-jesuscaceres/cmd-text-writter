using AsciiArt.Core;

namespace AsciiArt.Cli;

/// <summary>
/// Represents parsed CLI arguments.
/// </summary>
public sealed class CommandLineOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether parsing was successful.
    /// </summary>
    public bool IsValid { get; set; } = true;

    /// <summary>
    /// Gets or sets parsing error message.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets parse error exit code.
    /// </summary>
    public int ErrorCode { get; set; } = 2;

    /// <summary>
    /// Gets or sets a value indicating whether help was requested.
    /// </summary>
    public bool ShowHelp { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether color Help was requested.
    /// </summary>
    public bool ShowColorHelp { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the list-fonts mode was requested.
    /// </summary>
    public bool ListFonts { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether strict mode is enabled.
    /// </summary>
    public bool Strict { get; set; }

    /// <summary>
    /// Gets or sets the selected font name.
    /// </summary>
    public string? FontName { get; set; }

    /// <summary>
    /// Gets or sets the text to render.
    /// </summary>
    public string? Text { get; set; }

    /// <summary>
    /// Gets or sets the optional color for text rendering.
    /// </summary>
    public ColorOption? Color { get; set; }

    /// <summary>
    /// Gets or sets an optional accessibility warning message.
    /// </summary>
    public string? AccessibilityWarning { get; set; }
}
