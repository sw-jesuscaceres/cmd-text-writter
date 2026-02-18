# CLI Interface Contract: Add Color Parameter

**Objective**: Define the command-line interface signature and behavior for color support  
**Date**: February 18, 2026  
**Based on**: [spec.md](spec.md) and [data-model.md](data-model.md)

---

## Command Signature

### Current Signature (Baseline)

```
asciiart [OPTIONS] <TEXT>

OPTIONS:
  -f, --font <FONTNAME>        Font style (default: BasicBlock)
  -h, --help                   Show help message
```

### Enhanced Signature (With Color Parameter)

```
asciiart [OPTIONS] <TEXT>

OPTIONS:
  -f, --font <FONTNAME>        Font style (default: BasicBlock)
  -c, --color <COLORNAME>      Text color for output; one of:
                               red, green, blue, yellow, magenta, cyan, 
                               white, black (default: no color)
  -h, --help                   Show help message

EXAMPLES:
  asciiart "Hello"                    # Default: no color
  asciiart --color red "Hello"        # Red text
  asciiart -c green "World"           # Green text (short form)
  asciiart --font Block --color cyan "Text"  # Combined options
```

---

## Parameter Specifications

### `--color` Parameter

| Attribute | Value |
|-----------|-------|
| Long form | `--color` |
| Short form | `-c` |
| Argument type | String (color name) |
| Value | One of: `red`, `green`, `blue`, `yellow`, `magenta`, `cyan`, `white`, `black` |
| Case sensitivity | **Case-insensitive**: `--color RED`, `--color red`, `--color Red` all valid |
| Required | No |
| Default | `null` (no color; use terminal default) |
| Positional order | Can appear before or after text argument |
| Multiple occurrences | Last specified value wins; previous values ignored |

### Input Format Examples

**Valid Inputs**:
```
asciiart --color red "Hello"
asciiart -c green "World"
asciiart --color BLUE "Text"
asciiart --color Cyan "Text"
asciiart "Text" --color yellow
asciiart "Text" -c white
asciiart --font Block --color red "Hello"
asciiart --color red --color green "Hello"    # green wins (last specified)
```

**Invalid Inputs**:
```
asciiart --color purple "Text"        # Unknown color
asciiart --color "Text"               # Missing color value
asciiart --color "" "Text"            # Empty color value
```

---

## Output Behavior

### Success Case (Valid Color)

**Input**: `asciiart --color red "Hi"`

**Terminal Output** (with ANSI codes):
```
[31mHi in big ASCII art font[39m
```

**Visual Appearance**: Red-colored ASCII art

**File Output** (when piped):
```bash
$ asciiart --color red "Hi" > output.txt
```
File contains: Plain ASCII art **without** ANSI codes (codes stripped automatically)

---

### Success Case (No Color Specified)

**Input**: `asciiart "Hi"`

**Terminal Output**:
```
Hi in big ASCII art font
```
(Uses terminal's default text color)

**File Output**:
```bash
$ asciiart "Hi" > output.txt
```
File contains: Plain ASCII art (identical to above)

---

### Error Cases

#### Case 1: Unknown Color

**Input**: `asciiart --color purple "Text"`

**Exit Code**: `1` (error)

**Error Output** (stderr):
```
Error: Color 'purple' is not supported.

Available colors: red, green, blue, yellow, magenta, cyan, white, black

Did you mean one of these?
  - red (1 character difference)
  - green (4 character difference)
```

#### Case 2: Empty Color Value

**Input**: `asciiart --color "" "Text"`

**Exit Code**: `1` (error)

**Error Output** (stderr):
```
Error: Color value cannot be empty.

Usage: asciiart --color [colorname] "text"

Available colors: red, green, blue, yellow, magenta, cyan, white, black
```

#### Case 3: Multiple Parameters (Last Wins)

**Input**: `asciiart --color red --color blue "Text"`

**Behavior**: Uses `blue` (last specified)

**Output**: Blue-colored ASCII art

**Note**: No warning is issued for overridden values (standard CLI behavior)

---

### Accessibility Warnings (Informational, Not Errors)

#### Case 1: Potentially Inaccessible Color for Red-Green Colorblind Users

**Input**: `asciiart --color red "Text"`

**Exit Code**: `0` (success; warning only)

**Output** (stdout):
```
⚠ Warning: Red may be difficult to see for users with red-green color blindness.

Accessible alternatives:
  - cyan: Bright and easily distinguishable
  - yellow: High contrast
  - white: Maximum contrast

Render with: asciiart --color cyan "Text"
```

**Actual output**: Red-colored ASCII art (user choice is honored)

#### Case 2: Safe Color Choice

**Input**: `asciiart --color cyan "Text"`

**Exit Code**: `0` (success)

**Output**: Cyan-colored ASCII art (no warnings)

---

### Help Output

**Input**: `asciiart --help`

**Output**:
```
AsciiArt - Transform text into ASCII banner art

Usage: asciiart [OPTIONS] "text"

OPTIONS:
  -f, --font <FONTNAME>     Font style for rendering
                            Choices: BasicBlock, Block, BigMoneyNe, Caligraphy
                            Default: BasicBlock

  -c, --color <COLORNAME>   Color for output text
                            Choices: red, green, blue, yellow, magenta, cyan, white, black
                            Default: None (terminal default color)

  -h, --help                Display this help message
  --color help              Display available colors with descriptions

EXAMPLES:
  asciiart "Hello World"
      Render "Hello World" in default font and color

  asciiart --color red "Hello"
      Render "Hello" in red color

  asciiart --font Block --color cyan "Status: OK"
      Render using Block font in cyan color

FEATURES:
  • Case-insensitive color names: --color RED, --color red, --color Red
  • Multiple invocations: Later --color value overrides earlier ones
  • Piped output: Colors are automatically removed when piped/redirected
  • Accessibility: Warnings provided for potentially inaccessible colors

See extensive documentation at:
  https://github.com/your-org/asciiart/wiki/Color-Support
```

---

### Special Help Command

**Input**: `asciiart --color help`

**Output**:
```
Available Colors for --color parameter:

Color Name         ANSI Code    Accessibility Notes
─────────────────────────────────────────────────────────────
red                31           ⚠ Low contrast for Protanopia/Deuteranopia
green              32           ⚠ Low contrast for Protanopia/Deuteranopia  
blue               34           ✓ Accessible for most color blindness types
yellow             33           ✓ Accessible (high contrast)
magenta            35           ⚠ Moderate contrast concerns (some terminals)
cyan               36           ✓ Accessible (high contrast)
white              37           ✓ Accessible (maximum contrast)
black              30           ✓ Accessible (depends on background)

Recommendations:
  • For maximum accessibility: Use cyan, yellow, or white
  • For red/green blindness: Avoid red and green combinations
  • Always test output on target terminals

Run: asciiart --color [colorname] "your text"
```

---

## Status Codes

| Exit Code | Scenario |
|-----------|----------|
| `0` | Success (with or without accessibility warnings) |
| `1` | Invalid color name |
| `1` | Missing color value |
| `1` | Empty color value |
| `1` | Invalid font name (existing behavior) |
| `1` | Missing required text argument |
| `2` | Other argument parsing errors |

---

## Backward Compatibility

✅ **Fully backward compatible**:
- All existing commands work unchanged
- `--color` is optional; omitting it uses terminal default (existing behavior)
- New parameter doesn't affect parsing of existing parameters
- Help output enhanced but doesn't break script parsing

**No Breaking Changes**:
- `asciiart "Hello"` → Still works, produces default-color output
- `asciiart --font Block "Hello"` → Still works unchanged
- Existing scripts/automation: No modifications needed

---

## Performance Contract

| Operation | Target | Notes |
|-----------|--------|-------|
| Color parsing | <1ms | Enum.TryParse is O(1) |
| ANSI code lookup | <1ms | Constant-time switch expression |
| Accessibility validation | <10ms | Single pass through 8 colors |
| Total color support overhead | <2ms | Adds ~1% to total execution |
| Piped output detection | <1ms | Single property check |

**Overall Application Performance**:
- Existing <200ms target maintained
- Color support adds negligible overhead (~1-2ms)

---

## Testing Contract

### Happy Path Tests

- [x] `--color red "text"` produces red output
- [x] `-c green "text"` produces green output  
- [x] All 8 colors work correctly
- [x] Case-insensitive: `--color RED`, `--color red`, `--color Red` all work
- [x] Multiple invocations: Last value wins
- [x] No color specified: Uses terminal default
- [x] Works with `--font` parameter together

### Error Cases

- [x] Invalid color name: Shows helpful error with suggestions
- [x] Empty color value: Shows error message
- [x] `--color help`: Shows available colors
- [x] Colorblind warnings: Appropriate warnings shown

### Piped Output

- [x] `... | cat`: ANSI codes stripped, plain output to pipe
- [x] `... > file.txt`: ANSI codes stripped, plain output to file
- [x] Interactive terminal: ANSI codes present, colors visible

### Edge Cases

- [x] Multiple `--color` flags: Last wins
- [x] `--color` before text: Parsed correctly
- [x] `--color` after text: Parsed correctly
- [x] Mixed with other options: Parsed correctly

---

## Related Specifications

- [Feature Specification](spec.md) - Requirements and user stories
- [Research](research.md) - Technical decisions and justification
- [Data Model](data-model.md) - Internal entities and structures
