using AsciiArt.Core.Tests.Fixtures;
using AsciiArt.Fonts;
using FluentAssertions;

namespace AsciiArt.Core.Tests;

public sealed class AsciiRendererTests
{
    private readonly AsciiRenderer renderer = new();

    [Fact]
    public void Render_WithSingleCharacter_ReturnsCorrectHeight()
    {
        var font = new MockFont();

        var result = renderer.Render("A", font);

        result.Success.Should().BeTrue();
        result.Lines.Should().HaveCount(font.Height);
    }

    [Fact]
    public void Render_WithMultipleCharacters_ConcatenatesGlyphs()
    {
        var result = renderer.Render("AB", new MockFont());

        result.Success.Should().BeTrue();
        result.Lines.Should().Equal("A1B1", "A2B2", "A3B3");
    }

    [Fact]
    public void Render_WithSpaceCharacter_PreservesSpacing()
    {
        var result = renderer.Render("A B", new MockFont());

        result.Success.Should().BeTrue();
        result.Lines[0].Should().Be("A1  B1");
    }

    [Fact]
    public void Render_WithUnsupportedCharacter_UsesPlaceholderAndAddsWarning()
    {
        var result = renderer.Render("A@B", new MockFont());

        result.Success.Should().BeTrue();
        result.Lines.Should().Equal("A1?1B1", "A2?2B2", "A3?3B3");
        result.Warnings.Should().ContainSingle();
        result.Warnings[0].Should().Contain("'@'");
    }

    [Fact]
    public void Render_WithAllSupportedCharacters_HasNoWarnings()
    {
        var result = renderer.Render("A B", new MockFont());

        result.Success.Should().BeTrue();
        result.Warnings.Should().BeEmpty();
    }

    [Fact]
    public void Render_WithStrictModeAndUnsupportedCharacter_Fails()
    {
        var result = renderer.Render("A@B", new MockFont(), strict: true);

        result.Success.Should().BeFalse();
        result.Warnings.Should().ContainSingle();
        result.Warnings[0].Should().Contain("strict mode");
    }

    [Fact]
    public void Render_WithEmptyText_Fails()
    {
        var result = renderer.Render("   ", new MockFont());

        result.Success.Should().BeFalse();
        result.Warnings.Should().ContainSingle();
    }

    [Fact]
    public void Render_WithFortyCharacters_FitsWithinTerminalWidth()
    {
        var text = new string('A', 40);

        var result = renderer.Render(text, new BasicBlockFont());

        result.Success.Should().BeTrue();
        result.Lines.Should().OnlyContain(line => line.Length <= AsciiRenderer.MaxOutputWidth);
    }

    [Fact]
    public void Render_WithSeventyCharacters_ReturnsWidthError()
    {
        var text = new string('A', 70);

        var result = renderer.Render(text, new BasicBlockFont());

        result.Success.Should().BeFalse();
        result.Warnings.Should().ContainSingle();
        result.Warnings[0].Should().Contain("300");
    }

    [Fact]
    public void Render_WithTwentyCharacters_CompletesUnderTwoHundredMilliseconds()
    {
        var text = "HELLO12345WORLD67890";
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        var result = renderer.Render(text, new BasicBlockFont());

        stopwatch.Stop();
        result.Success.Should().BeTrue();
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(200);
    }
}
