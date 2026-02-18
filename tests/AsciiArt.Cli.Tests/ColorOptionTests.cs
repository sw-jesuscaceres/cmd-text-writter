using AsciiArt.Core;

namespace AsciiArt.Cli.Tests;

public class ColorOptionTests
{
    [Fact]
    public void ColorOption_HasRedValue()
    {
        // Arrange & Act
        var color = ColorOption.Red;

        // Assert
        Assert.Equal(ColorOption.Red, color);
    }

    [Fact]
    public void ColorOption_HasGreenValue()
    {
        // Arrange & Act
        var color = ColorOption.Green;

        // Assert
        Assert.Equal(ColorOption.Green, color);
    }

    [Fact]
    public void ColorOption_HasBlueValue()
    {
        // Arrange & Act
        var color = ColorOption.Blue;

        // Assert
        Assert.Equal(ColorOption.Blue, color);
    }

    [Fact]
    public void ColorOption_HasYellowValue()
    {
        // Arrange & Act
        var color = ColorOption.Yellow;

        // Assert
        Assert.Equal(ColorOption.Yellow, color);
    }

    [Fact]
    public void ColorOption_HasMagentaValue()
    {
        // Arrange & Act
        var color = ColorOption.Magenta;

        // Assert
        Assert.Equal(ColorOption.Magenta, color);
    }

    [Fact]
    public void ColorOption_HasCyanValue()
    {
        // Arrange & Act
        var color = ColorOption.Cyan;

        // Assert
        Assert.Equal(ColorOption.Cyan, color);
    }

    [Fact]
    public void ColorOption_HasWhiteValue()
    {
        // Arrange & Act
        var color = ColorOption.White;

        // Assert
        Assert.Equal(ColorOption.White, color);
    }

    [Fact]
    public void ColorOption_HasBlackValue()
    {
        // Arrange & Act
        var color = ColorOption.Black;

        // Assert
        Assert.Equal(ColorOption.Black, color);
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
    public void ColorOption_AllValuesAreDefined(ColorOption color)
    {
        // Assert
        Assert.True(Enum.IsDefined(typeof(ColorOption), color));
    }
}
