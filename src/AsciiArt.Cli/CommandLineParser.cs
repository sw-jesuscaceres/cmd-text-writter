namespace AsciiArt.Cli;

/// <summary>
/// Parses command-line arguments for the ASCII art CLI.
/// </summary>
public sealed class CommandLineParser
{
    /// <summary>
    /// Parses command-line arguments.
    /// </summary>
    /// <param name="args">Raw argument list.</param>
    /// <returns>Parsed options and parse state.</returns>
    public CommandLineOptions Parse(string[] args)
    {
        var options = new CommandLineOptions();
        var textTokens = new List<string>();
        var safeArgs = args ?? Array.Empty<string>();

        for (var index = 0; index < safeArgs.Length; index++)
        {
            var token = safeArgs[index];

            switch (token)
            {
                case "--help":
                case "-h":
                    options.ShowHelp = true;
                    break;
                case "--strict":
                    options.Strict = true;
                    break;
                case "--list-fonts":
                    options.ListFonts = true;
                    break;
                case "--font":
                    if (index + 1 >= safeArgs.Length || safeArgs[index + 1].StartsWith("-", StringComparison.Ordinal))
                    {
                        options.IsValid = false;
                        options.ErrorMessage = "Missing value for --font option.";
                        return options;
                    }

                    index++;
                    options.FontName = safeArgs[index];
                    break;
                default:
                    if (token.StartsWith("-", StringComparison.Ordinal))
                    {
                        options.IsValid = false;
                        options.ErrorMessage = $"Unknown option '{token}'.";
                        return options;
                    }

                    textTokens.Add(token);
                    break;
            }
        }

        options.Text = textTokens.Count == 0 ? null : string.Join(" ", textTokens);
        return options;
    }
}
