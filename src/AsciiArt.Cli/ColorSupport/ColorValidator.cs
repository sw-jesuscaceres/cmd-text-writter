using AsciiArt.Core;

namespace AsciiArt.Cli.ColorSupport;

/// <summary>
/// Validates color choices and provides accessibility recommendations.
/// </summary>
public static class ColorValidator
{
    private static readonly string[] AllColorNames =
    {
        "red", "green", "blue", "yellow", "magenta", "cyan", "white", "black"
    };

    /// <summary>
    /// Validates if a color name is valid and returns a validation result.
    /// </summary>
    /// <param name="colorName">The color name to validate.</param>
    /// <returns>A validation result with the parsed color or error message.</returns>
    public static ColorValidationResult Validate(string colorName)
    {
        if (string.IsNullOrWhiteSpace(colorName))
        {
            return ColorValidationResult.Invalid("Color name cannot be empty.");
        }

        // Try direct parse
        if (Enum.TryParse<ColorOption>(colorName, ignoreCase: true, out var parsedColor))
        {
            // Check for colorblind accessibility issues
            var accessibility = CheckColorblindAccessibility(parsedColor);
            if (accessibility != null)
            {
                return ColorValidationResult.Valid(parsedColor, accessibility);
            }

            return ColorValidationResult.Valid(parsedColor);
        }

        // Find closest match for suggestion
        var closest = FindClosestColorName(colorName);
        if (closest != null)
        {
            return ColorValidationResult.InvalidWithSuggestion(
                $"Invalid color '{colorName}'. Did you mean '{closest}'?",
                closest);
        }

        var availableColors = string.Join(", ", AllColorNames);
        return ColorValidationResult.Invalid(
            $"Invalid color '{colorName}'. Valid colors: {availableColors}");
    }

    /// <summary>
    /// Checks if a color has colorblind accessibility concerns.
    /// </summary>
    /// <param name="color">The color to check.</param>
    /// <returns>Accessibility warning message or null if no concerns.</returns>
    private static string? CheckColorblindAccessibility(ColorOption color)
    {
        return color switch
        {
            ColorOption.Red => "Warning: Red may be difficult for colorblind users. Consider cyan, yellow, or white for better accessibility.",
            ColorOption.Green => "Warning: Green may be difficult for colorblind users. Consider cyan, yellow, or white for better accessibility.",
            _ => null
        };
    }

    /// <summary>
    /// Finds the closest matching color name using Levenshtein distance.
    /// </summary>
    /// <param name="input">The user input.</param>
    /// <returns>The closest color name or null if no close match found.</returns>
    private static string? FindClosestColorName(string input)
    {
        const int maxDistance = 2;
        var closestName = "";
        var closestDistance = int.MaxValue;

        foreach (var colorName in AllColorNames)
        {
            var distance = LevenshteinDistance(input.ToLowerInvariant(), colorName);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestName = colorName;
            }
        }

        return closestDistance <= maxDistance ? closestName : null;
    }

    /// <summary>
    /// Calculates the Levenshtein distance between two strings.
    /// </summary>
    /// <param name="source">The source string.</param>
    /// <param name="target">The target string.</param>
    /// <returns>The edit distance.</returns>
    private static int LevenshteinDistance(string source, string target)
    {
        if (source.Length == 0) return target.Length;
        if (target.Length == 0) return source.Length;

        var matrix = new int[source.Length + 1, target.Length + 1];

        for (var i = 0; i <= source.Length; i++) matrix[i, 0] = i;
        for (var j = 0; j <= target.Length; j++) matrix[0, j] = j;

        for (var i = 1; i <= source.Length; i++)
        {
            for (var j = 1; j <= target.Length; j++)
            {
                var cost = source[i - 1] == target[j - 1] ? 0 : 1;
                var deletion = matrix[i - 1, j] + 1;
                var insertion = matrix[i, j - 1] + 1;
                var substitution = matrix[i - 1, j - 1] + cost;

                matrix[i, j] = Math.Min(deletion, Math.Min(insertion, substitution));
            }
        }

        return matrix[source.Length, target.Length];
    }
}

/// <summary>
/// Represents the result of a color validation.
/// </summary>
public sealed class ColorValidationResult
{
    private ColorValidationResult(bool isValid, ColorOption? color, string? message, string? suggestion)
    {
        IsValid = isValid;
        Color = color;
        Message = message;
        Suggestion = suggestion;
    }

    /// <summary>
    /// Gets a value indicating whether the color is valid.
    /// </summary>
    public bool IsValid { get; }

    /// <summary>
    /// Gets the parsed color (null if invalid).
    /// </summary>
    public ColorOption? Color { get; }

    /// <summary>
    /// Gets the message (error or advisory).
    /// </summary>
    public string? Message { get; }

    /// <summary>
    /// Gets the suggested color name (if available).
    /// </summary>
    public string? Suggestion { get; }

    /// <summary>
    /// Creates a valid result.
    /// </summary>
    /// <param name="color">The parsed color.</param>
    /// <param name="message">Optional advisory message (e.g., accessibility warning).</param>
    /// <returns>A valid validation result.</returns>
    public static ColorValidationResult Valid(ColorOption color, string? message = null)
        => new(true, color, message, null);

    /// <summary>
    /// Creates an invalid result with suggestion.
    /// </summary>
    /// <param name="message">Error message.</param>
    /// <param name="suggestion">Suggested color name.</param>
    /// <returns>An invalid validation result with suggestion.</returns>
    public static ColorValidationResult InvalidWithSuggestion(string message, string suggestion)
        => new(false, null, message, suggestion);

    /// <summary>
    /// Creates an invalid result without suggestion.
    /// </summary>
    /// <param name="message">Error message.</param>
    /// <returns>An invalid validation result.</returns>
    public static ColorValidationResult Invalid(string message)
        => new(false, null, message, null);
}
