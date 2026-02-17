namespace AsciiArt.Cli;

/// <summary>
/// Provides consistent writes to stdout and stderr.
/// </summary>
public sealed class ConsoleOutput
{
    private readonly TextWriter stdout;
    private readonly TextWriter stderr;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleOutput"/> class.
    /// </summary>
    /// <param name="stdoutWriter">Output writer.</param>
    /// <param name="stderrWriter">Error writer.</param>
    public ConsoleOutput(TextWriter stdoutWriter, TextWriter stderrWriter)
    {
        stdout = stdoutWriter ?? throw new ArgumentNullException(nameof(stdoutWriter));
        stderr = stderrWriter ?? throw new ArgumentNullException(nameof(stderrWriter));
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
    /// Writes a line to stderr.
    /// </summary>
    /// <param name="line">Line to write.</param>
    public void WriteErrorLine(string line)
    {
        stderr.WriteLine(line);
    }
}
