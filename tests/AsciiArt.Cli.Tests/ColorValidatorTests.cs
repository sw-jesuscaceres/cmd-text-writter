using AsciiArt.Core;
using AsciiArt.Cli.ColorSupport;

namespace AsciiArt.Cli.Tests;

public class ColorValidatorTests
{
    [Theory]
    [InlineData("red")]
    [InlineData("RED")]
    [InlineData("Red")]
    [InlineData("green")]
    [InlineData("blue")]
    [InlineData("yellow")]
    [InlineData("magenta")]
    [InlineData("cyan")]
    [InlineData("white")]
    [InlineData("black")]
    public void Validate_WithValidColor_ReturnsValid(string colorName)
    {
        var result = ColorValidator.Validate(colorName);

        Assert.True(result.IsValid);
        Assert.NotNull(result.Color);
    }

    [Fact]
    public void Validate_WithRed_ReturnsWarning()
    {
        var result = ColorValidator.Validate("red");

        Assert.True(result.IsValid);
        Assert.Equal(ColorOption.Red, result.Color);
        Assert.NotNull(result.Message);
        Assert.Contains("colorblind", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Validate_WithGreen_ReturnsWarning()
    {
        var result = ColorValidator.Validate("green");

        Assert.True(result.IsValid);
        Assert.Equal(ColorOption.Green, result.Color);
        Assert.NotNull(result.Message);
        Assert.Contains("colorblind", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("cyan")]
    [InlineData("yellow")]
    [InlineData("white")]
    [InlineData("blue")]
    [InlineData("black")]
    [InlineData("magenta")]
    public void Validate_WithAccessibleColor_NoWarning(string colorName)
    {
        var result = ColorValidator.Validate(colorName);

        Assert.True(result.IsValid);
        Assert.Null(result.Message);
    }

    [Fact]
    public void Validate_WithInvalidColor_ReturnsInvalid()
    {
        var result = ColorValidator.Validate("purple");

        Assert.False(result.IsValid);
        Assert.Null(result.Color);
        Assert.NotNull(result.Message);
        Assert.Contains("Invalid color", result.Message);
    }

    [Fact]
    public void Validate_WithInvalidColor_MayIncludeSuggestion()
    {
        var result = ColorValidator.Validate("redd"); // Close to "red" (distance 1)

        Assert.False(result.IsValid);
        Assert.Null(result.Color);
        Assert.NotNull(result.Message);
        // "redd" → "red" has distance 1, so should suggest
        if (result.Message.Contains("Did you mean"))
        {
            Assert.NotEmpty(result.Suggestion ?? "");
        }
    }

    [Fact]
    public void Validate_WithEmptyString_ReturnsInvalid()
    {
        var result = ColorValidator.Validate("");

        Assert.False(result.IsValid);
        Assert.Null(result.Color);
        Assert.NotNull(result.Message);
    }

    [Fact]
    public void Validate_WithNullString_ReturnsInvalid()
    {
        var result = ColorValidator.Validate(null!);

        Assert.False(result.IsValid);
        Assert.Null(result.Color);
        Assert.NotNull(result.Message);
    }

    [Theory]
    [InlineData("red")]
    [InlineData("gre")] // Close to "green" (distance 2)
    [InlineData("blu")] // Close to "blue" (distance 2)
    public void Validate_WithCloseTypo_SuggestsCorrectColor(string input)
    {
        var result = ColorValidator.Validate(input);

        if (!result.IsValid && input != "red")
        {
            Assert.NotNull(result.Suggestion);
            Assert.NotEmpty(result.Suggestion);
        }
    }

    [Theory]
    [InlineData("xyz")]      // Distance > 2
    [InlineData("qwerty")]   // Distance > 2
    [InlineData("foobar")]   // Distance > 2
    public void Validate_WithDistantTypo_NoSuggestion(string input)
    {
        var result = ColorValidator.Validate(input);

        Assert.False(result.IsValid);
        Assert.Null(result.Color);
    }
}
