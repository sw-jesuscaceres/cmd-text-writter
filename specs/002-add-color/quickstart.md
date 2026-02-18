# Quickstart: Implementing Color Parameter

**For**: Developers implementing the "Add Color Parameter" feature  
**Time to Read**: 5 minutes  
**Complete Docs**: See [plan.md](plan.md), [data-model.md](data-model.md), [contracts/cli.md](contracts/cli.md)

---

## Implementation Overview

Add a `--color` parameter to the AsciiArt CLI allowing users to render ASCII art in one of 8 terminal colors.

**Scope**:
- ✅ P1: Core color output functionality (6-8 hours)
- ✅ P2: Color discovery & validation (4-6 hours)
- ✅ P2: Accessibility warnings (2-3 hours)

**Total Estimate**: 12-17 hours

---

## Architecture Summary

```
CommandLineParser
    ↓ (parses --color argument)
ColorValidator (validate input, suggest typos)
    ↓ (returns ColorOption enum)
CommandLineOptions.Color property
    ↓ (passed to application)
AsciiArtApplication
    ↓ (normal rendering, stores color in RenderResult)
RenderResult.Color (NEW property)
    ↓ (passed to output handler)
ConsoleOutput.WriteToConsole()
    ├─ Check Console.IsOutputRedirected
    ├─ If false (interactive): Apply ANSI codes
    └─ If true (piped): No color codes
```

---

## Key Decisions ✅

| Decision | Why | Impact |
|----------|-----|--------|
| **ANSI codes** (not Windows API) | Cross-platform, single implementation | Simple string concatenation |
| **Color as metadata in RenderResult** | Keeps rendering clean | One property addition to existing class |
| **Validate at parse time** | Fail fast with good error messages | ColorValidator utility class |
| **Strip codes when piped** | Explicit is better than implicit | Check Console.IsOutputRedirected |
| **Colorblind warnings** | Accessibility by default | ColorblindHelper utility class |
| **Levenshtein distance for typos** | No external dependencies | ~20-line utility method |

---

## Files to Create/Modify

### New Files

```
src/AsciiArt.Cli/ColorSupport/
├── ColorOption.cs              # Enum: red, green, blue, yellow, magenta, cyan, white, black
├── ColorValidator.cs           # Parse, validate, suggest typos
├── ColorCodeGenerator.cs       # Convert enum to ANSI codes
└── ColorblindHelper.cs         # Warn about inaccessible colors, suggest alternatives

tests/AsciiArt.Cli.Tests/
├── ColorOptionParsingTests.cs        # Unit tests for parsing
├── ColorOutputGenerationTests.cs     # Unit tests for ANSI code generation
├── ColorAccessibilityTests.cs        # Unit tests for colorblind warnings
└── ColorblindSuggestionTests.cs      # Unit tests for typo suggestions
```

### Files to Modify

```
src/AsciiArt.Cli/
├── CommandLineOptions.cs       # Add Color property
├── CommandLineParser.cs        # Parse --color argument
├── ConsoleOutput.cs            # Apply color codes; respect IsOutputRedirected
└── Program.cs                  # Wire everything together

src/AsciiArt.Core/
└── RenderResult.cs             # Add Color property

tests/AsciiArt.Cli.Tests/
├── ArgumentParsingTests.cs     # Extend: Add --color tests
├── ApplicationExecutionTests.cs # Extend: Add color output tests
```

---

## Implementation Checklist

### Phase 1: Core Functionality (P1)

- [ ] **Create ColorOption enum**
  ```csharp
  public enum ColorOption { Red, Green, Blue, Yellow, Magenta, Cyan, White, Black }
  ```

- [ ] **Create ColorCodeGenerator**
  ```csharp
  public static string GetAnsiCode(ColorOption color)
  // Returns: "\x1b[31m" for Red, "\x1b[32m" for Green, etc.
  ```

- [ ] **Extend RenderResult**
  ```csharp
  public ColorOption? Color { get; set; }
  ```

- [ ] **Extend CommandLineOptions**
  ```csharp
  public ColorOption? Color { get; set; }
  ```

- [ ] **Create ColorValidator**
  ```csharp
  public static bool TryParseColor(string input, out ColorOption color)
  // Use Enum.TryParse(input, ignoreCase: true)
  ```

- [ ] **Update CommandLineParser**
  - Parse `--color` or `-c` flag
  - Call ColorValidator.TryParseColor()
  - Store result in CommandLineOptions.Color

- [ ] **Update ConsoleOutput.WriteToConsole()**
  ```csharp
  if (renderResult.Color.HasValue && !Console.IsOutputRedirected)
  {
      var colorCode = ColorCodeGenerator.GetAnsiCode(renderResult.Color.Value);
      Console.Write(colorCode + renderResult.Text + "\x1b[39m");
  }
  else
  {
      Console.Write(renderResult.Text);
  }
  ```

- [ ] **Wire in AsciiArtApplication**
  - Pass CommandLineOptions.Color to RenderResult

---

### Phase 2: Validation & Discovery (P2)

- [ ] **Create ColorValidator (enhanced)**
  - Handle typo suggestions (Levenshtein distance)
  - Return helpful error messages
  - List all available colors in error output

- [ ] **Create ColorblindHelper**
  - Warn for red (not accessible for protanopia/deuteranopia)
  - Warn for green (same)
  - Suggest alternatives (cyan, yellow, white)

- [ ] **Update help output**
  - Document `--color` parameter
  - Show all 8 color options
  - Add examples

- [ ] **Implement `--color help` special case**
  - Show available colors with descriptions
  - Show accessibility notes for each color

---

### Testing Checklist

- [ ] **Unit tests for ColorValidator**
  - Valid colors (all 8)
  - Case-insensitivity
  - Typo suggestions
  - Error messages

- [ ] **Unit tests for ColorCodeGenerator**
  - Each color produces correct ANSI code
  - Reset code is correct

- [ ] **Unit tests for ColorblindHelper**
  - Red triggers warning
  - Green triggers warning
  - Cyan/yellow/white don't
  - Suggestions are reasonable

- [ ] **Integration tests**
  - `--color red "text"` produces colored output
  - No color specified → default output
  - Piped output → no ANSI codes
  - Multiple colors → last wins
  - Invalid color → helpful error
  - `--color help` → shows available colors

---

## Code Examples

### Example 1: Parse --color Argument

```csharp
public class CommandLineParser
{
    public static CommandLineOptions Parse(string[] args)
    {
        var options = new CommandLineOptions();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "--color" || args[i] == "-c")
            {
                if (i + 1 >= args.Length)
                {
                    Console.Error.WriteLine("Error: --color requires a value");
                    return null;
                }
                
                if (!ColorValidator.TryParseColor(args[++i], out var color))
                {
                    // Error message handled by ColorValidator
                    return null;
                }
                
                options.Color = color;
            }
            // ... handle other arguments
        }
        return options;
    }
}
```

### Example 2: Generate Colored Output

```csharp
public class ConsoleOutput
{
    public static void WriteToConsole(RenderResult result)
    {
        string output = result.Text;
        
        // Apply color only if:
        // 1. Color is specified
        // 2. Output is NOT redirected (going to terminal)
        if (result.Color.HasValue && !Console.IsOutputRedirected)
        {
            var ansiCode = ColorCodeGenerator.GetAnsiCode(result.Color.Value);
            output = ansiCode + output + "\x1b[39m";  // Reset code
        }
        
        Console.Write(output);
    }
}
```

### Example 3: Colorblind Warning

```csharp
public class ColorblindHelper
{
    public static void WarnIfInaccessible(ColorOption color)
    {
        string? warning = color switch
        {
            ColorOption.Red => "Red may be difficult for red-green colorblind users. Consider: cyan, yellow, white",
            ColorOption.Green => "Green may be difficult for red-green colorblind users. Consider: cyan, yellow, white",
            _ => null  // Blue, Yellow, Cyan, White, Black are safe
        };
        
        if (warning != null)
        {
            Console.WriteLine($"⚠ Warning: {warning}");
        }
    }
}
```

---

## Constitution Compliance

✅ **Code Quality**: All public methods will have XML doc comments  
✅ **Testing**: Comprehensive test suite required before completion  
✅ **UX**: Maintains consistent CLI pattern, clear error messages  
✅ **Performance**: <2ms overhead, well within 200ms budget

---

## Related Documentation

| Document | Purpose |
|----------|---------|
| [spec.md](spec.md) | Feature requirements (user-facing) |
| [research.md](research.md) | Technical decisions and justification |
| [data-model.md](data-model.md) | Domain entities and data structures |
| [contracts/cli.md](contracts/cli.md) | Complete CLI interface specification |
| [plan.md](plan.md) | Full implementation plan and phases |

---

## Quick Links

- **ANSI Color Codes**: [ANSI Escape Code Reference](https://en.wikipedia.org/wiki/ANSI_escape_code#8-bit)
  - Red: `\x1b[31m`
  - Green: `\x1b[32m`
  - etc.
  - Reset: `\x1b[39m`

- **.NET Documentation**:
  - `Enum.TryParse`: https://docs.microsoft.com/en-us/dotnet/api/system.enum.tryparse
  - `Console.IsOutputRedirected`: https://docs.microsoft.com/en-us/dotnet/api/system.console.isoutputredirected

- **Accessibility**:
  - Colorblind Vision Simulator: https://www.color-blindness.com/
  - WCAG Color Contrast: https://www.w3.org/WAI/WCAG21/Understanding/contrast-minimum.html

---

## Support

- Questions about requirements? See [spec.md](spec.md)
- Questions about design? See [data-model.md](data-model.md)
- Questions about CLI behavior? See [contracts/cli.md](contracts/cli.md)
- Questions about why decisions were made? See [research.md](research.md)
