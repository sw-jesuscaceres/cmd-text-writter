namespace AsciiArt.Fonts;

internal static class FigletFontParser
{
    public static ParsedFont Parse(string fontText)
    {
        if (string.IsNullOrWhiteSpace(fontText))
        {
            throw new ArgumentException("Font text cannot be null or empty.", nameof(fontText));
        }

        var lines = fontText.Replace("\r", string.Empty, StringComparison.Ordinal).Split('\n');
        var header = lines[0];

        if (!header.StartsWith("flf2a", StringComparison.Ordinal))
        {
            throw new InvalidOperationException("Invalid FIGlet font header.");
        }

        var hardBlank = header[5];
        var headerParts = header.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (headerParts.Length < 6)
        {
            throw new InvalidOperationException("Invalid FIGlet header format.");
        }

        var height = int.Parse(headerParts[1]);
        var commentLineCount = int.Parse(headerParts[5]);
        var lineIndex = 1 + commentLineCount;
        if (lineIndex >= lines.Length)
        {
            throw new InvalidOperationException("FIGlet data section is missing.");
        }

        var endMark = lines[lineIndex].Length == 0 ? '@' : lines[lineIndex][^1];

        var glyphs = new Dictionary<char, string[]>();
        for (var charCode = 32; charCode <= 126; charCode++)
        {
            var glyph = new string[height];
            for (var row = 0; row < height; row++)
            {
                if (lineIndex >= lines.Length)
                {
                    throw new InvalidOperationException("Unexpected end of FIGlet glyph data.");
                }

                var raw = lines[lineIndex++];
                glyph[row] = DecodeLine(raw, endMark, hardBlank);
            }

            glyphs[(char)charCode] = glyph;
        }

        return new ParsedFont(height, glyphs);
    }

    private static string DecodeLine(string raw, char endMark, char hardBlank)
    {
        var line = raw;
        while (line.Length > 0 && line[^1] == endMark)
        {
            line = line[..^1];
        }

        return line.Replace(hardBlank, ' ');
    }
}

internal sealed record ParsedFont(int Height, IReadOnlyDictionary<char, string[]> Glyphs);
