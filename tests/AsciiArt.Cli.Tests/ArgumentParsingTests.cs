using AsciiArt.Cli;
using FluentAssertions;

namespace AsciiArt.Cli.Tests;

public sealed class ArgumentParsingTests
{
    private readonly CommandLineParser parser = new();

    [Fact]
    public void Parse_WithHelpFlag_SetsHelpFlag()
    {
        var options = parser.Parse(new[] { "--help" });

        options.IsValid.Should().BeTrue();
        options.ShowHelp.Should().BeTrue();
    }

    [Fact]
    public void Parse_WithText_ExtractsPositionalArgument()
    {
        var options = parser.Parse(new[] { "Hello" });

        options.IsValid.Should().BeTrue();
        options.Text.Should().Be("Hello");
    }

    [Fact]
    public void Parse_WithMultipleTextTokens_JoinsWithSpaces()
    {
        var options = parser.Parse(new[] { "Hello", "World" });

        options.IsValid.Should().BeTrue();
        options.Text.Should().Be("Hello World");
    }

    [Fact]
    public void Parse_WithUnknownOption_ReturnsUsageError()
    {
        var options = parser.Parse(new[] { "--badoption" });

        options.IsValid.Should().BeFalse();
        options.ErrorCode.Should().Be(2);
        options.ErrorMessage.Should().Contain("Unknown option");
    }

    [Fact]
    public void Parse_WithFontOptionWithoutValue_ReturnsUsageError()
    {
        var options = parser.Parse(new[] { "--font" });

        options.IsValid.Should().BeFalse();
        options.ErrorCode.Should().Be(2);
        options.ErrorMessage.Should().Contain("--font");
    }
}
