# Research Phase: Add Color Parameter

**Objective**: Resolve technical unknowns and establish best practices for color implementation  
**Date**: February 18, 2026  
**Source Spec**: [spec.md](spec.md)

---

## Research Topic 1: Windows Console Color API vs ANSI Codes

**Question**: How should colors be generated for cross-platform support in .NET 6?

### Findings

**ANSI Escape Codes (Recommended)**:

- Standard format: `\x1b[<code>m` (e.g., `\x1b[31m` for red)
- **Native support in .NET 6**: `System.Console` has built-in ANSI support via `Console.IsOutputRedirected`
- **Windows 10+ support**: Native ANSI support enabled by default in Windows Terminal and modern CMD
- **Cross-platform**: Works natively on Linux, macOS, Windows 10+
- **Performance**: Minimal overhead; just string concatenation
- **Fallback behavior**: Output is automatically stripped when piped (detected via `Console.IsOutputRedirected`)

**Windows Console API Alternative**:

- Uses `SetConsoleTextAttribute()` with color codes (FOREGROUND_RED = 0x0004, etc.)
- Requires P/Invoke and platform detection
- Only works on Windows; requires separate implementation for other platforms
- More complex code; mixes platform-specific logic with core rendering
- Unnecessary complexity for modern .NET

**Decision**: ✅ **Use ANSI escape codes**

- Rationale: Single implementation works across all platforms; .NET 6 handles automatic stripping
- Implementation: 8 basic colors use codes 30-37 (foreground) with reset code 39
- No external dependencies needed; use `System.Console` exclusively

### Code Pattern

```csharp
// Color codes for 8 basic colors
const string AnsiRed = "\x1b[31m";
const string AnsiGreen = "\x1b[32m";
const string AnsiReset = "\x1b[39m";

// Usage
string coloredText = AnsiRed + "Hello" + AnsiReset;
Console.Write(coloredText);  // ANSI codes auto-stripped if output is piped
```

---

## Research Topic 2: Colorblind-Friendly Color Combinations

**Question**: Which color pairs are safely distinguishable for users with color vision deficiency?

### Findings

**Common Types of Color Blindness**:

1. **Protanopia (Red-Green Blindness)**: ~0.6% of males
   - Cannot distinguish red from green
   - Can distinguish blue/purple/yellow

2. **Deuteranopia (Red-Green Blindness)**: ~0.4% of males
   - Cannot distinguish red/orange from brown/green
   - Can clearly see blue and yellow

3. **Tritanopia (Blue-Yellow Blindness)**: <0.001% (very rare)
   - Cannot distinguish blue from yellow
   - Can see red and green

**WCAG 2.1 Accessibility Guidelines**:

- Minimum contrast ratio of 4.5:1 for normal text
- Color should not be the only means of conveying information
- Accessible color palette strategies:
  - Use colors that differ in both hue and brightness
  - Pair complementary colors (blue + yellow, red + cyan)
  - Test with Ishihara color blindness tests

**Recommended Safe Color Combinations for Terminal**:

| Background | Safe Foreground Colors | Avoid |
|------------|------------------------|-------|
| Black (terminal default) | Cyan, Yellow, White, Green | Red, Magenta (low contrast) |
| White | Black, Red, Blue, Green | Yellow (insufficient contrast) |

**Cross-checking Tool Recommendation**:

- Use color distance formula (Delta E in LAB color space) or simple contrast ratio calculation
- For CLI with standard terminal colors, validate against: Does NOT use red/green together as only difference

**Decision**: ✅ **Implement colorblind validation at input level**

- Rationale: Warn users when their color choice may be problematic; suggest alternatives
- Implementation:
  - Create `ColorblindHelper.cs` with validation logic
  - For red selections: suggest cyan, yellow, or white as alternatives
  - For green selections: suggest cyan, yellow, or white as alternatives
  - For magenta selections: suggest white or clear alternatives
  - Always allow user choice but provide warnings

### Validation Logic

- **Red in red-green blindness**: Indistinguishable from green/black/cyan when viewed through protanopia filter
- **Solution**: If user picks red, warn and suggest cyan (bright and clearly different in protanopia view)
- **Accessible by default**: Blue, Yellow, Cyan, White, Black are mostly safe

---

## Research Topic 3: Terminal Detection for Piped Output

**Question**: How to determine if output is sent to a terminal vs piped/redirected?

### Findings

**.NET 6 Console Properties**:

```csharp
// Property available in System.Console
bool isRedirected = Console.IsOutputRedirected;  // true if piped/redirected
bool isTerminal = !Console.IsOutputRedirected;   // true if interactive terminal
```

**Behavior**:

- Returns `true` when output is piped: `asciiart --color red "text" > output.txt`
- Returns `true` when output is redirected: `asciiart --color red "text" | cat`
- Returns `false` in interactive terminal: `asciiart --color red "text"`

**ANSI Code Handling**:

- When `Console.IsOutputRedirected == true`: .NET 6 automatically disables ANSI codes
- Actual behavior: ANSI escape sequences are written to output as literal text (8 characters like `[31m`)
- User problem: Files and pipes contain visible ANSI code artifacts

**Decision**: ✅ **Explicitly strip ANSI codes when piped**

- Rationale: Prevents malformed output in files; matches Unix philosophy (no invisible codes in stored output)
- Implementation in `ConsoleOutput.cs`:

  ```csharp
  if (Console.IsOutputRedirected)
  {
      // Write plain text without color codes
      Console.Write(renderResult.Text);
  }
  else
  {
      // Write with ANSI color codes
      Console.Write(GenerateColoredOutput(renderResult));
  }
  ```

---

## Research Topic 4: Closest Color Matching for Typo Suggestions

**Question**: How to find the closest matching valid color when a user typos?

### Findings

**Options Evaluated**:

1. **Levenshtein Distance (String Edit Distance)** ✅ **RECOMMENDED**
   - Calculate minimum edits (insert/delete/replace) needed to transform input to valid color
   - Example: "redx" → "red" = 1 edit; "reedy" → "red" = 2 edits
   - Pros: Simple, no dependencies, deterministic, intuitive
   - Cons: Doesn't account for phonetic similarity (but not needed for color names)

2. **Jaro-Winkler Distance**
   - Used in fuzzy matching; weights prefixes more heavily
   - More complex; arguably overkill for 8 color names
   - Pros: Better for real-world typos; industry standard
   - Cons: Additional complexity

3. **NuGet Fuzzy Search Libraries**
   - Example: `FuzzySharp` package
   - Pros: Battle-tested, comprehensive
   - Cons: Adds external dependency; contradicts "no external dependencies" principle

**Decision**: ✅ **Implement simple Levenshtein distance**

- Rationale: Sufficient for 8 color names; no external dependencies; easy to test
- Implementation:

  ```csharp
  public static int LevenshteinDistance(string s1, string s2)
  {
      int[,] dp = new int[s1.Length + 1, s2.Length + 1];
      for (int i = 0; i <= s1.Length; i++) dp[i, 0] = i;
      for (int j = 0; j <= s2.Length; j++) dp[0, j] = j;
      
      for (int i = 1; i <= s1.Length; i++)
          for (int j = 1; j <= s2.Length; j++)
              dp[i, j] = Math.Min(
                  Math.Min(dp[i-1, j] + 1, dp[i, j-1] + 1),
                  dp[i-1, j-1] + (s1[i-1] == s2[j-1] ? 0 : 1)
              );
      return dp[s1.Length, s2.Length];
  }
  ```

- **Suggestion logic**:
  - Calculate distance to all 8 valid colors
  - Return colors with distance <= 2 (allows up to 2 typos)
  - If only 1 match: "Did you mean 'red'?"
  - If 2+ matches: "Did you mean one of: red, cyan, green?"

---

## Research Topic 5: .NET 6 String Comparison & Color Parameter Handling

**Question**: How to implement case-insensitive color parameter parsing?

### Findings

**.NET 6 String Comparison**:

```csharp
// Case-insensitive comparison (Ordinal = fastest, correct for color names)
if ("Red".Equals(userInput, StringComparison.OrdinalIgnoreCase))
{
    // Match found
}

// Parse to enum
if (Enum.TryParse(userInput, ignoreCase: true, out ColorOption color))
{
    // Successfully parsed
}
```

**Recommended Approach**:

1. Define `ColorOption` as enum with lowercase names: `Red`, `Green`, `Blue`, etc.
2. Use `Enum.TryParse()` with `ignoreCase: true`
3. Fallback to Levenshtein distance if no exact match

**Decision**: ✅ **Use Enum.TryParse with ignoreCase**

- Rationale: Built-in, zero-dependency, idiomatic C#
- Implementation: Define enum with standard capitalization; let TryParse handle case-insensitivity

---

## Summary of Research Decisions

| Topic | Decision | Rationale |
|-------|----------|-----------|
| Color codes | ANSI (cross-platform) | Single implementation; no P/Invoke; works all platforms |
| Colorblind support | Validate & warn + suggest alternatives | Accessible by design; helps users avoid poor choices |
| Piped output | Explicitly strip ANSI codes if redirected | Prevents artifact output in files/pipes |
| Typo matching | Levenshtein distance (DIY) | No dependencies; sufficient for 8 colors; easy to test |
| Case-insensitive parsing | Enum.TryParse(ignoreCase: true) | Built-in; idiomatic; zero overhead |

---

## Next Steps (Phase 1: Design & Contracts)

1. ✅ Decisions made → Ready for `data-model.md`
2. Implement `ColorOption` enum
3. Create `ColorValidator`, `ColorCodeGenerator`, `ColorblindHelper` utilities
4. Extend `RenderResult` with `Color?` property
5. Extend `CommandLineOptions` and `ConsoleOutput`
6. Create comprehensive test suite covering all edge cases
