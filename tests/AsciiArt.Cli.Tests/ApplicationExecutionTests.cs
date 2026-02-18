using AsciiArt.Cli;
using AsciiArt.Core;
using AsciiArt.Fonts;
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
    public void Run_WithCaligraphyFont_PrintsAsciiArtAndReturnsZero()
    {
        var stdout = new StringWriter();
        var stderr = new StringWriter();

        var code = app.Run(new[] { "--font", "caligraphy", "Hi" }, stdout, stderr);

        var output = stdout.ToString().Replace("\r", string.Empty, StringComparison.Ordinal);
        var lines = output.EndsWith('\n')
            ? output[..^1].Split('\n', StringSplitOptions.None)
            : output.Split('\n', StringSplitOptions.None);

        code.Should().Be(0);
        lines.Should().HaveCount(new CaligraphyFont().Height);
        lines.Should().Contain(line => !string.IsNullOrWhiteSpace(line));
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

    [Fact]
    public void Run_WithValidColorParameter_OutputContainsAsciiArt()
    {
        var stdout = new StringWriter();
        var stderr = new StringWriter();

        // Note: Console.IsOutputRedirected will be true for StringWriter, so colors won't be applied
        // This test is for parsing; actual color output would require terminal interaction
        var code = app.Run(new[] { "--color", "blue", "Hi" }, stdout, stderr);

        code.Should().Be(0);
        stdout.ToString().Should().NotBeEmpty();
        // No warning for blue (accessible color)
        stderr.ToString().Should().BeEmpty();
    }

    [Fact]
    public void Run_WithInvalidColor_PrintsErrorAndReturnsTwo()
    {
        var stdout = new StringWriter();
        var stderr = new StringWriter();

        var code = app.Run(new[] { "--color", "purple", "Hi" }, stdout, stderr);

        code.Should().Be(2);
        stdout.ToString().Should().BeEmpty();
        stderr.ToString().Should().Contain("Error:");
        stderr.ToString().Should().Contain("Invalid color");
    }

    [Theory]
    [InlineData("red")]
    [InlineData("green")]
    [InlineData("blue")]
    [InlineData("yellow")]
    [InlineData("magenta")]
    [InlineData("cyan")]
    [InlineData("white")]
    [InlineData("black")]
    public void Run_WithAllValidColors_ReturnsZeroAndGeneratesOutput(string color)
    {
        var stdout = new StringWriter();
        var stderr = new StringWriter();

        var code = app.Run(new[] { "--color", color, "A" }, stdout, stderr);

        code.Should().Be(0);
        stdout.ToString().Should().NotBeEmpty();

        // Red and green may have accessibility warnings on stderr
        if (color != "red" && color != "green")
        {
            stderr.ToString().Should().BeEmpty();
        }
    }

    [Fact]
    public void Run_WithColorRed_DisplaysAccessibilityWarning()
    {
        var stdout = new StringWriter();
        var stderr = new StringWriter();

        var code = app.Run(new[] { "--color", "red", "A" }, stdout, stderr);

        code.Should().Be(0);
        stderr.ToString().Should().Contain("colorblind");
    }

    [Fact]
    public void Run_WithColorHelp_DisplaysColorList()
    {
        var stdout = new StringWriter();
        var stderr = new StringWriter();

        var code = app.Run(new[] { "--color", "help" }, stdout, stderr);

        code.Should().Be(0);
        stdout.ToString().Should().Contain("red");
        stdout.ToString().Should().Contain("green");
        stdout.ToString().Should().Contain("cyan");
        stdout.ToString().Should().Contain("Accessibility");
    }

    [Fact]
    public void Run_WithInvalidColorClose_SuggestsCorrectColor()
    {
        var stdout = new StringWriter();
        var stderr = new StringWriter();

        var code = app.Run(new[] { "--color", "redd", "A" }, stdout, stderr);

        code.Should().Be(2);
        stderr.ToString().Should().Contain("Error:");
        stderr.ToString().Should().Contain("Did you mean");
    }
}
