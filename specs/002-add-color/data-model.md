# Data Model: Add Color Parameter

**Objective**: Define domain entities and data structures for color support  
**Date**: February 18, 2026  
**Based on**: [research.md](research.md)

---

## Domain Entities

### ColorOption Enum

**Purpose**: Represents the 8 standard terminal colors available for text rendering.

**Definition**:
```csharp
namespace AsciiArt.Cli.ColorSupport;

/// <summary>
/// Enumeration of supported terminal text colors.
/// </summary>
public enum ColorOption
{
    /// <summary>Red text color.</summary>
    Red,
    
    /// <summary>Green text color.</summary>
    Green,
    
    /// <summary>Blue text color.</summary>
    Blue,
    
    /// <summary>Yellow text color.</summary>
    Yellow,
    
    /// <summary>Magenta text color.</summary>
    Magenta,
    
    /// <summary>Cyan text color.</summary>
    Cyan,
    
    /// <summary>White text color.</summary>
    White,
    
    /// <summary>Black text color.</summary>
    Black
}
```

**Relationships**: 
- Referenced by `CommandLineOptions.Color` (nullable, default null)
- Referenced by `RenderResult.Color` (stored as optional metadata)

**Validation Rules**:
- Must be one of the 8 defined values
- Case-insensitive input accepted (e.g., "RED" and "red" both map to Red)
- Invalid values rejected with helpful error message

**State Transitions**: None (enum values are immutable)

---

### RenderResult Enhancement

**Current Definition** (in AsciiArt.Core):
```csharp
public class RenderResult
{
    public string Text { get; set; }
    public bool IsSuccess { get; set; }
    public string? Error { get; set; }
}
```

**Enhanced Definition**:
```csharp
public class RenderResult
{
    public string Text { get; set; }
    public bool IsSuccess { get; set; }
    public string? Error { get; set; }
    
    /// <summary>
    /// Optional color to apply when rendering this result to console.
    /// Null indicates colors should not be applied (terminal default).
    /// </summary>
    public ColorOption? Color { get; set; }
}
```

**Rationale**: 
- Stores color as separate metadata property (not embedded in Text)
- Keeps rendering logic clean; color applied only at output stage
- Allows future extensibility (styling, formatting flags)

---

### CommandLineOptions Enhancement

**Current Definition** (in AsciiArt.Cli):
```csharp
public class CommandLineOptions
{
    public string? Text { get; set; }
    public string FontName { get; set; } = "BasicBlock";
}
```

**Enhanced Definition**:
```csharp
public class CommandLineOptions
{
    /// <summary>The text to render as ASCII art.</summary>
    public string? Text { get; set; }
    
    /// <summary>The font style to use for rendering (e.g., "Block", "BasicBlock").</summary>
    public string FontName { get; set; } = "BasicBlock";
    
    /// <summary>
    /// Optional color for the ASCII art output. null means use terminal default color.
    /// Specified via --color parameter (e.g., --color red).
    /// </summary>
    public ColorOption? Color { get; set; }
}
```

**Relationships**:
- Set by `CommandLineParser` based on `--color` command-line argument
- Passed to `AsciiArtApplication` for rendering
- Transferred to `RenderResult.Color` for output

---

## Domain Value Objects

### ColorCode (Internal Utility)

**Purpose**: Encapsulates ANSI color code generation and constants.

```csharp
namespace AsciiArt.Cli.ColorSupport;

/// <summary>
/// Utility class containing ANSI/platform-specific color codes.
/// </summary>
public static class ColorCode
{
    // ANSI Foreground color codes (30-37)
    public const string Red = "\x1b[31m";
    public const string Green = "\x1b[32m";
    public const string Blue = "\x1b[34m";
    public const string Yellow = "\x1b[33m";
    public const string Magenta = "\x1b[35m";
    public const string Cyan = "\x1b[36m";
    public const string White = "\x1b[37m";
    public const string Black = "\x1b[30m";
    
    // Reset code
    public const string Reset = "\x1b[39m";
    
    /// <summary>Get the ANSI code for the specified color.</summary>
    public static string GetAnsiCode(ColorOption color) => color switch
    {
        ColorOption.Red => Red,
        ColorOption.Green => Green,
        ColorOption.Blue => Blue,
        ColorOption.Yellow => Yellow,
        ColorOption.Magenta => Magenta,
        ColorOption.Cyan => Cyan,
        ColorOption.White => White,
        ColorOption.Black => Black,
        _ => throw new ArgumentOutOfRangeException(nameof(color))
    };
}
```

---

## Validation Rules & Constraints

### Color Validation Rules

1. **Input Parsing**:
   - Accept any case variation (RED, Red, red all valid)
   - Use `Enum.TryParse(input, ignoreCase: true)` for parsing
   
2. **Typo Detection & Suggestions**:
   - If input doesn't match, calculate Levenshtein distance
   - Suggest colors with distance ≤ 2
   - Provide helpful error message with suggestions

3. **Colorblind Validation** (Warning, not Error):
   - When color is parsed successfully, check accessibility
   - Warn if color is red (risky for protanopia/deuteranopia)
   - Warn if color is green (risky for red-green blindness)
   - Warn if color is magenta (poor contrast on some terminals)
   - Suggest alternatives for each problematic color

4. **Piped Output Handling**:
   - At output time: Check `Console.IsOutputRedirected`
   - If true: Write plain text without ANSI codes
   - If false: Write text with ANSI codes embedded

### Performance Constraints

- Color parsing: must complete in <1ms (simple enum lookup)
- Color code generation: must complete in <1ms (string constant lookup)
- accessibility warnings: single pass, <10ms for 8-color validation
- Overall impact: negligible (<2ms per invocation), well within 200ms total budget

---

## State Diagram

```
User Input (--color red)
    ↓
CommandLineParser.Parse()
    ↓ (parsed as string "red")
ColorValidator.ValidateAndParse()
    ↓ 
Enum.TryParse("red", ignoreCase: true, out ColorOption color)
    ↓ (successfully parsed as ColorOption.Red)
ColorblindValidator.CheckAndWarn()
    ↓ (no warning for red on most terminals)
CommandLineOptions.Color = ColorOption.Red
    ↓
AsciiArtApplication.GenerateArt()
    ↓
RenderResult.Color = ColorOption.Red
    ↓
ConsoleOutput.WriteToConsole()
    ├─ if (Console.IsOutputRedirected)
    │   └─ Write plain text (no color codes)
    └─ else
        └─ Write with ANSI codes: "\x1b[31m" + Text + "\x1b[39m"
           ↓
           Terminal renders in red
```

---

## Extension Points (Future Enhancements)

1. **Bright/Bold Colors**: Add 8 more colors (bright red, bright green, etc.) by extending enum
2. **Background Colors**: Add `BackgroundColor?` property to `RenderResult` for background coloring
3. **256-Color / True Color Support**: Extend `ColorCode` to support more colors without breaking current interface
4. **Color Profiles**: Store user preferences for default color, colorblind mode, etc.
5. **Theming**: Composition of multiple properties (foreground color, background, bold, etc.)

---

## Database/Persistence

**Not applicable** - This is a stateless CLI application. No persistence layer needed.

---

## APIs Using These Entities

### System.Console API (Microsoft.NET.Sdk)
- `Console.IsOutputRedirected` - detect piped output
- `Console.Write(string)` - output text with embedded ANSI codes
- All color logic uses standard library; zero external dependencies

### Enum Utilities (.NET Framework Library)
- `Enum.TryParse<T>(string, bool, out T)` - case-insensitive color parsing
- Standard library; no external dependencies

---

## Summary

| Entity | Type | Scope | Purpose |
|--------|------|-------|---------|
| ColorOption | Enum | CLI namespace | Represent valid color choices |
| RenderResult.Color | Property | Core namespace | Store color metadata with render output |
| CommandLineOptions.Color | Property | CLI namespace | Store parsed color choice from arguments |
| ColorCode | Static utility | CLI namespace | Provide ANSI codes and code generation |
| ColorValidator | Class | CLI namespace | Parse and validate color input; suggest alternatives |
| ColorblindValidator | Class | CLI namespace | Check accessibility; warn & suggest safe alternatives |
| ColorCodeGenerator | Class | CLI namespace | Convert ColorOption to platform-specific codes |

All entities follow existing project patterns (immutable enums, property-based configuration objects, static utilities for stateless logic).
