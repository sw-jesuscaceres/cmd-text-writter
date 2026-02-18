using AsciiArt.Core;
using AsciiArt.Cli.ColorSupport;

namespace AsciiArt.Cli;

/// <summary>
/// Provides consistent writes to stdout and stderr.
/// </summary>
public sealed class ConsoleOutput
{
    private readonly TextWriter stdout;
    private readonly TextWriter stderr;
    private readonly bool shouldApplyColor;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleOutput"/> class.
    /// </summary>
    /// <param name="stdoutWriter">Output writer.</param>
    /// <param name="stderrWriter">Error writer.</param>
    public ConsoleOutput(TextWriter stdoutWriter, TextWriter stderrWriter)
    {
        stdout = stdoutWriter ?? throw new ArgumentNullException(nameof(stdoutWriter));
        stderr = stderrWriter ?? throw new ArgumentNullException(nameof(stderrWriter));
        
        // Only apply colors when output is not redirected (i.e., when writing to actual terminal)
        shouldApplyColor = !Console.IsOutputRedirected;
    }

    /// <summary>
    /// Writes a line to stdout.
    /// </summary>
    /// <param name="line">Line to write.</param>
    public void WriteLine(string line)
    {
        stdout.WriteLine(line);
    }

    /// <summary>
    /// Writes a colored line to stdout.
    /// </summary>
    /// <param name="line">Line to write.</param>
    /// <param name="color">Color to apply (null = default).</param>
    public void WriteLineColored(string line, ColorOption? color)
    {
        if (color == null || !shouldApplyColor)
        {
            stdout.WriteLine(line);
            return;
        }

        var ansiCode = ColorCodeGenerator.GetAnsiCode(color.Value);
        var coloredLine = $"{ansiCode}{line}{ColorCodeGenerator.Reset}";
        stdout.WriteLine(coloredLine);
    }

    /// <summary>
    /// Writes a line to stderr.
    /// </summary>
    /// <param name="line">Line to write.</param>
    public void WriteErrorLine(string line)
    {
        stderr.WriteLine(line);
    }
}
