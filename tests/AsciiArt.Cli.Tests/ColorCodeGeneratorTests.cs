using AsciiArt.Core;
using AsciiArt.Cli.ColorSupport;

namespace AsciiArt.Cli.Tests;

public class ColorCodeGeneratorTests
{
    [Theory]
    [InlineData(ColorOption.Red, "\x1b[31m")]
    [InlineData(ColorOption.Green, "\x1b[32m")]
    [InlineData(ColorOption.Blue, "\x1b[34m")]
    [InlineData(ColorOption.Yellow, "\x1b[33m")]
    [InlineData(ColorOption.Magenta, "\x1b[35m")]
    [InlineData(ColorOption.Cyan, "\x1b[36m")]
    [InlineData(ColorOption.White, "\x1b[37m")]
    [InlineData(ColorOption.Black, "\x1b[30m")]
    public void GetAnsiCode_ReturnsCorrectCodeForColor(ColorOption color, string expectedCode)
    {
        // Act
        var code = ColorCodeGenerator.GetAnsiCode(color);

        // Assert
        Assert.Equal(expectedCode, code);
    }

    [Fact]
    public void GetAnsiCode_ThrowsForInvalidColor()
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => 
            ColorCodeGenerator.GetAnsiCode((ColorOption)999));
    }

    [Fact]
    public void Reset_IsCorrectValue()
    {
        // Assert
        Assert.Equal("\x1b[39m", ColorCodeGenerator.Reset);
    }

    [Theory]
    [InlineData(ColorOption.Red)]
    [InlineData(ColorOption.Green)]
    [InlineData(ColorOption.Blue)]
    [InlineData(ColorOption.Yellow)]
    [InlineData(ColorOption.Magenta)]
    [InlineData(ColorOption.Cyan)]
    [InlineData(ColorOption.White)]
    [InlineData(ColorOption.Black)]
    public void GetAnsiCode_AllColorsReturnNonEmptyString(ColorOption color)
    {
        // Act
        var code = ColorCodeGenerator.GetAnsiCode(color);

        // Assert
        Assert.NotNull(code);
        Assert.NotEmpty(code);
        Assert.StartsWith("\x1b[", code);
        Assert.EndsWith("m", code);
    }
}
