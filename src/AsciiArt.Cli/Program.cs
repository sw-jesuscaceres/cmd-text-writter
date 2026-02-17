namespace AsciiArt.Cli;

/// <summary>
/// Entry point for the ASCII art command-line tool.
/// </summary>
public static class Program
{
    /// <summary>
    /// Runs the CLI application.
    /// </summary>
    /// <param name="args">Command-line arguments.</param>
    /// <returns>Exit code.</returns>
    public static int Main(string[] args)
    {
        var app = new AsciiArtApplication();
        return app.Run(args);
    }
}
