# Data Model: ASCII Text Art Command-Line Tool

**Phase**: Implementation-aligned model  
**Date**: 2026-02-17  
**Feature**: [001-ascii-art](spec.md)

## Entity 1: Font (`IAsciiFont`)

Represents a renderable ASCII font.

### Attributes

- `name` (string): unique font identifier (case-insensitive in registry)
- `height` (int): number of output lines per glyph
- `placeholderChar` (char): fallback glyph when input char is unsupported
- `glyphs` (map<char, string[]>): character map to line arrays

### Rules

- Every returned glyph must have exactly `height` lines
- `placeholderChar` must resolve to a valid glyph

### Implementations

- `BigMoneyNeFont` (default)
- `BasicBlockFont`

---

## Entity 2: Parsed FIGlet Font (`ParsedFont`)

Internal representation used by `BigMoneyNeFont`.

### Attributes

- `height` (int)
- `glyphs` (map<char, string[]>)

### Source

- Built from embedded `.flf` content through `FigletFontParser`

---

## Entity 3: RenderResult

Represents renderer output.

### Attributes

- `lines` (`IReadOnlyList<string>`): rendered lines
- `warnings` (`IReadOnlyList<string>`): warning/error details
- `success` (bool): operation status

### Behavior

- `success = true`: lines are render-ready
- `success = false`: `warnings` includes failure reason

---

## Entity 4: CommandLineOptions

Parsed CLI state.

### Attributes

- `isValid` (bool)
- `errorMessage` (string?)
- `errorCode` (int, default 2)
- `showHelp` (bool)
- `listFonts` (bool)
- `strict` (bool)
- `fontName` (string?)
- `text` (string?)

---

## Entity 5: FontRegistry

In-memory registry of available fonts.

### Attributes

- `fonts` (map<string, IAsciiFont>)
- `defaultFont` (`IAsciiFont`, currently `big-money-ne`)

### Operations

- `GetFont(name)`
- `ListFonts()`

---

## Validation Rules (Implemented)

### Input validation

- Text must be non-empty
- Text length must be <= 40 (CLI layer)
- Unknown options fail parsing with usage error

### Rendering validation

- `font.Height` must be > 0 and <= 24
- output width must be <= 300
- unsupported chars:
  - strict mode: fail
  - non-strict mode: placeholder + warning

---

## State Flow

1. Parse args into `CommandLineOptions`
2. Resolve font from `FontRegistry`
3. Render via `AsciiRenderer`
4. Emit stdout/stderr and exit code in `AsciiArtApplication`
