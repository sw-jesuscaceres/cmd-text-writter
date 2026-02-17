using AsciiArt.Cli;
using FluentAssertions;

namespace AsciiArt.Cli.Tests;

public sealed class ApplicationExecutionTests
{
    private readonly AsciiArtApplication app = new();

    [Fact]
    public void Run_WithHelp_PrintsHelpAndReturnsZero()
    {
        var stdout = new StringWriter();
        var stderr = new StringWriter();

        var code = app.Run(new[] { "--help" }, stdout, stderr);

        code.Should().Be(0);
        stdout.ToString().Should().Contain("Usage: asciiart");
        stderr.ToString().Should().BeEmpty();
    }

    [Fact]
    public void Run_WithValidText_PrintsAsciiArtAndReturnsZero()
    {
        var stdout = new StringWriter();
        var stderr = new StringWriter();

        var code = app.Run(new[] { "Hello", "world" }, stdout, stderr);
        var actualLines = stdout.ToString()
            .Replace("\r", string.Empty, StringComparison.Ordinal)
            .Split('\n', StringSplitOptions.None)
            .Take(11)
            .Select(line => line.TrimEnd())
            .ToArray();

        var expectedLines = new[]
        {
            " /$$   /$$           /$$ /$$                                                   /$$       /$$",
            "| $$  | $$          | $$| $$                                                  | $$      | $$",
            "| $$  | $$  /$$$$$$ | $$| $$  /$$$$$$        /$$  /$$  /$$  /$$$$$$   /$$$$$$ | $$  /$$$$$$$",
            "| $$$$$$$$ /$$__  $$| $$| $$ /$$__  $$      | $$ | $$ | $$ /$$__  $$ /$$__  $$| $$ /$$__  $$",
            "| $$__  $$| $$$$$$$$| $$| $$| $$  \\ $$      | $$ | $$ | $$| $$  \\ $$| $$  \\__/| $$| $$  | $$",
            "| $$  | $$| $$_____/| $$| $$| $$  | $$      | $$ | $$ | $$| $$  | $$| $$      | $$| $$  | $$",
            "| $$  | $$|  $$$$$$$| $$| $$|  $$$$$$/      |  $$$$$/$$$$/|  $$$$$$/| $$      | $$|  $$$$$$$",
            "|__/  |__/ \\_______/|__/|__/ \\______/        \\_____/\\___/  \\______/ |__/      |__/ \\_______/",
            string.Empty,
            string.Empty,
            string.Empty,
        };

        code.Should().Be(0);
        actualLines.Should().Equal(expectedLines);
        stderr.ToString().Should().BeEmpty();
    }

    [Fact]
    public void Run_WithNoArguments_PrintsErrorAndReturnsOne()
    {
        var stdout = new StringWriter();
        var stderr = new StringWriter();

        var code = app.Run(Array.Empty<string>(), stdout, stderr);

        code.Should().Be(1);
        stdout.ToString().Should().BeEmpty();
        stderr.ToString().Should().Contain("Error:");
        stderr.ToString().Should().Contain("Usage: asciiart");
    }

    [Fact]
    public void Run_WithUnknownOption_PrintsErrorAndReturnsTwo()
    {
        var stdout = new StringWriter();
        var stderr = new StringWriter();

        var code = app.Run(new[] { "--badoption" }, stdout, stderr);

        code.Should().Be(2);
        stdout.ToString().Should().BeEmpty();
        stderr.ToString().Should().Contain("Unknown option");
    }

    [Fact]
    public void Run_WithUnsupportedCharacter_PrintsWarningAndReturnsZero()
    {
        var stdout = new StringWriter();
        var stderr = new StringWriter();

        var code = app.Run(new[] { "Hi🙂" }, stdout, stderr);

        code.Should().Be(0);
        stdout.ToString().Should().NotBeNullOrWhiteSpace();
        stderr.ToString().Should().Contain("Warning:");
        stderr.ToString().Should().Contain("?");
    }

    [Fact]
    public void Run_WithStrictModeAndUnsupportedCharacter_ReturnsOne()
    {
        var stdout = new StringWriter();
        var stderr = new StringWriter();

        var code = app.Run(new[] { "--strict", "Hi🙂" }, stdout, stderr);

        code.Should().Be(1);
        stdout.ToString().Should().BeEmpty();
        stderr.ToString().Should().Contain("strict mode");
    }

    [Fact]
    public void Run_WithListFonts_PrintsFontsAndReturnsZero()
    {
        var stdout = new StringWriter();
        var stderr = new StringWriter();

        var code = app.Run(new[] { "--list-fonts" }, stdout, stderr);

        code.Should().Be(0);
        stdout.ToString().Should().Contain("big-money-ne");
        stdout.ToString().Should().Contain("basicblock");
        stdout.ToString().Should().Contain("caligraphy");
        stderr.ToString().Should().BeEmpty();
    }

    [Fact]
    public void Run_WithcaligraphyFont_PrintsAsciiArtAndReturnsZero()
    {
        var stdout = new StringWriter();
        var stderr = new StringWriter();

        var code = app.Run(new[] { "--font", "caligraphy", "Hi" }, stdout, stderr);

        code.Should().Be(0);
        stdout.ToString().Should().Contain("|H||I|");
        stderr.ToString().Should().BeEmpty();
    }

    [Fact]
    public void Run_WithTooLongText_PrintsErrorAndReturnsOne()
    {
        var stdout = new StringWriter();
        var stderr = new StringWriter();

        var code = app.Run(new[] { new string('A', 41) }, stdout, stderr);

        code.Should().Be(1);
        stdout.ToString().Should().BeEmpty();
        stderr.ToString().Should().Contain("Text too long");
    }
}
