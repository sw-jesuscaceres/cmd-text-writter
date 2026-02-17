using AsciiArt.Fonts;
using FluentAssertions;

namespace AsciiArt.Core.Tests;

public sealed class FontTests
{
    [Fact]
    public void BasicBlockFont_SupportsLettersDigitsAndSpace()
    {
        var font = new BasicBlockFont();

        font.Supports('A').Should().BeTrue();
        font.Supports('z').Should().BeTrue();
        font.Supports('0').Should().BeTrue();
        font.Supports('9').Should().BeTrue();
        font.Supports(' ').Should().BeTrue();
    }

    [Fact]
    public void BasicBlockFont_UnsupportedCharacter_FallsBackToPlaceholderGlyph()
    {
        var font = new BasicBlockFont();

        var unsupportedGlyph = font.GetGlyph('@');
        var placeholderGlyph = font.GetGlyph('?');

        unsupportedGlyph.Should().Equal(placeholderGlyph);
    }

    [Fact]
    public void BasicBlockFont_GlyphHeight_MatchesFontHeight()
    {
        var font = new BasicBlockFont();

        var glyph = font.GetGlyph('A');

        glyph.Should().HaveCount(font.Height);
    }

    [Fact]
    public void FontRegistry_ReturnsDefaultFontAndList()
    {
        FontRegistry.DefaultFont.Name.Should().Be("big-money-ne");
        FontRegistry.GetFont("big-money-ne").Should().NotBeNull();
        FontRegistry.GetFont("basicblock").Should().NotBeNull();
        FontRegistry.ListFonts().Should().NotBeEmpty();
    }
}
