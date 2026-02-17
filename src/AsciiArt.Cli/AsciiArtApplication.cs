using AsciiArt.Core;
using AsciiArt.Fonts;

namespace AsciiArt.Cli;

/// <summary>
/// Coordinates argument parsing, rendering, and output for the CLI.
/// </summary>
public sealed class AsciiArtApplication
{
    private readonly CommandLineParser parser;
    private readonly AsciiRenderer renderer;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsciiArtApplication"/> class.
    /// </summary>
    public AsciiArtApplication()
        : this(new CommandLineParser(), new AsciiRenderer())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsciiArtApplication"/> class.
    /// </summary>
    /// <param name="parser">Parser dependency.</param>
    /// <param name="renderer">Renderer dependency.</param>
    public AsciiArtApplication(CommandLineParser parser, AsciiRenderer renderer)
    {
        this.parser = parser ?? throw new ArgumentNullException(nameof(parser));
        this.renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
    }

    /// <summary>
    /// Executes the CLI command.
    /// </summary>
    /// <param name="args">Input arguments.</param>
    /// <param name="stdout">Output writer.</param>
    /// <param name="stderr">Error writer.</param>
    /// <returns>Process exit code.</returns>
    public int Run(string[] args, TextWriter? stdout = null, TextWriter? stderr = null)
    {
        var output = new ConsoleOutput(stdout ?? Console.Out, stderr ?? Console.Error);

        try
        {
            var options = parser.Parse(args ?? Array.Empty<string>());

            if (!options.IsValid)
            {
                output.WriteErrorLine(HelpFormatter.BuildError(options.ErrorMessage ?? "Invalid command."));
                return options.ErrorCode;
            }

            if (options.ShowHelp)
            {
                output.WriteLine(HelpFormatter.BuildHelp());
                return 0;
            }

            if (options.ListFonts)
            {
                foreach (var registeredFont in FontRegistry.ListFonts())
                {
                    output.WriteLine($"{registeredFont.Name} ({registeredFont.Height} lines)");
                }

                return 0;
            }

            if (string.IsNullOrWhiteSpace(options.Text))
            {
                output.WriteErrorLine(HelpFormatter.BuildError("No text provided."));
                return 1;
            }

            if (options.Text.Length > 40)
            {
                output.WriteErrorLine(HelpFormatter.BuildError(
                    $"Text too long ({options.Text.Length} chars > 40 char limit). Try shorter input."));
                return 1;
            }

            var font = string.IsNullOrWhiteSpace(options.FontName)
                ? FontRegistry.DefaultFont
                : FontRegistry.GetFont(options.FontName);

            if (font is null)
            {
                var availableFonts = string.Join(", ", FontRegistry.ListFonts().Select(f => f.Name));
                output.WriteErrorLine(HelpFormatter.BuildError(
                    $"Font '{options.FontName}' not found. Available fonts: {availableFonts}."));
                return 1;
            }

            var result = renderer.Render(options.Text, font, options.Strict);
            if (!result.Success)
            {
                output.WriteErrorLine(HelpFormatter.BuildError(result.Warnings.FirstOrDefault() ?? "Rendering failed."));
                return 1;
            }

            foreach (var line in result.Lines)
            {
                output.WriteLine(line);
            }

            foreach (var warning in result.Warnings)
            {
                output.WriteErrorLine($"Warning: {warning}");
            }

            return 0;
        }
        catch
        {
            output.WriteErrorLine(HelpFormatter.BuildError(
                "An internal error occurred during conversion. Please try again with valid input."));
            return 1;
        }
    }
}
