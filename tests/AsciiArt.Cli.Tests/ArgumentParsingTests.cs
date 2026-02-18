using AsciiArt.Cli;
using AsciiArt.Core;
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

    [Theory]
    [InlineData("--color", "red")]
    [InlineData("-c", "green")]
    [InlineData("--color", "BLUE")]
    [InlineData("-c", "YeLLow")]
    public void Parse_WithColorOption_ParsesColorValue(string colorFlag, string colorName)
    {
        var options = parser.Parse(new[] { colorFlag, colorName, "Hello" });

        options.IsValid.Should().BeTrue();
        options.Color.Should().NotBeNull();
        options.Text.Should().Be("Hello");
    }

    [Fact]
    public void Parse_WithColorRed_SetsColorToRed()
    {
        var options = parser.Parse(new[] { "--color", "red", "Test" });

        options.Color.Should().Be(ColorOption.Red);
    }

    [Fact]
    public void Parse_WithColorGreen_SetsColorToGreen()
    {
        var options = parser.Parse(new[] { "--color", "green", "Test" });

        options.Color.Should().Be(ColorOption.Green);
    }

    [Fact]
    public void Parse_WithInvalidColor_ReturnsError()
    {
        var options = parser.Parse(new[] { "--color", "purple", "Hello" });

        options.IsValid.Should().BeFalse();
        options.ErrorCode.Should().Be(2);
        options.ErrorMessage.Should().Contain("Invalid color");
    }

    [Fact]
    public void Parse_WithColorOptionWithoutValue_ReturnsUsageError()
    {
        var options = parser.Parse(new[] { "--color" });

        options.IsValid.Should().BeFalse();
        options.ErrorCode.Should().Be(2);
        options.ErrorMessage.Should().Contain("--color");
    }

    [Fact]
    public void Parse_WithMultipleColorOptions_UsesLastColor()
    {
        var options = parser.Parse(new[] { "--color", "red", "--color", "blue", "Hello" });

        options.Color.Should().Be(ColorOption.Blue);
    }
}
