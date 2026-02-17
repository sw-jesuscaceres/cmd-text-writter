using System.Text;

namespace AsciiArt.Core;

/// <summary>
/// Converts plain text into ASCII art using a provided font.
/// </summary>
public sealed class AsciiRenderer
{
    /// <summary>
    /// Maximum supported output width.
    /// </summary>
    public const int MaxOutputWidth = 300;

    /// <summary>
    /// Maximum supported output height.
    /// </summary>
    public const int MaxOutputHeight = 24;

    /// <summary>
    /// Renders text into ASCII art lines.
    /// </summary>
    /// <param name="text">Text to render.</param>
    /// <param name="font">Font to use.</param>
    /// <param name="strict">
    /// When <see langword="true"/>, rendering fails if unsupported characters are found.
    /// </param>
    /// <returns>Render output with warnings or error details.</returns>
    public RenderResult Render(string text, IAsciiFont font, bool strict = false)
    {
        if (font is null)
        {
            throw new ArgumentNullException(nameof(font));
        }

        if (string.IsNullOrWhiteSpace(text))
        {
            return RenderResult.Failure("No text provided.");
        }

        if (font.Height <= 0)
        {
            return RenderResult.Failure("Font height must be greater than zero.");
        }

        if (font.Height > MaxOutputHeight)
        {
            return RenderResult.Failure($"Output exceeds {MaxOutputHeight} lines.");
        }

        var builders = new StringBuilder[font.Height];
        for (var index = 0; index < builders.Length; index++)
        {
            builders[index] = new StringBuilder();
        }

        var unsupportedCharacters = new HashSet<char>();

        foreach (var sourceCharacter in text)
        {
            var character = sourceCharacter;

            if (!font.Supports(character))
            {
                unsupportedCharacters.Add(sourceCharacter);
                if (strict)
                {
                    var ordered = unsupportedCharacters
                        .OrderBy(c => c)
                        .Select(c => $"'{c}'");

                    return RenderResult.Failure(
                        $"Unsupported characters found and --strict mode enabled: {string.Join(", ", ordered)}");
                }

                character = font.PlaceholderChar;
            }

            var glyph = font.GetGlyph(character);
            if (glyph.Length != font.Height)
            {
                return RenderResult.Failure("Font glyph height does not match the font definition.");
            }

            for (var lineIndex = 0; lineIndex < font.Height; lineIndex++)
            {
                builders[lineIndex].Append(glyph[lineIndex]);
            }
        }

        var outputLines = builders.Select(builder => builder.ToString()).ToArray();
        if (outputLines.Any(line => line.Length > MaxOutputWidth))
        {
            return RenderResult.Failure($"Output exceeds {MaxOutputWidth} characters.");
        }

        var warnings = unsupportedCharacters
            .OrderBy(c => c)
            .Select(c => $"Character '{c}' not supported, rendered as '{font.PlaceholderChar}'.")
            .ToArray();

        return RenderResult.SuccessResult(outputLines, warnings);
    }
}
