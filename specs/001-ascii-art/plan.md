# Implementation Plan: ASCII Text Art Command-Line Tool

**Branch**: `001-ascii-art`  
**Date**: 2026-02-17  
**Spec**: [spec.md](spec.md)  
**Status**: Implemented and validated against automated tests

## Summary

The application is implemented as a three-layer .NET solution:

- `AsciiArt.Cli`: command parsing, output streams, user-facing errors/help
- `AsciiArt.Core`: rendering engine and result model
- `AsciiArt.Fonts`: built-in fonts (`big-money-ne`, `basicblock`, `caligraphy`) and registry

Current behavior is optimized for deterministic CLI output and testability.

## Technical Context

- **Language/Version**: C# / .NET 6 (`net6.0` in current projects)
- **Dependencies**: .NET standard libraries only (no runtime third-party libraries)
- **Testing**: xUnit + FluentAssertions
- **Platform**: Windows/Linux/macOS
- **Output limits**: width <= 300, height <= 24
- **Input limit**: max 40 characters at CLI level
- **Default font**: `big-money-ne` (embedded FIGlet `.flf` resource)

## Current Architecture

```text
src/
+-- AsciiArt.Cli/
|   +-- Program.cs
|   +-- AsciiArtApplication.cs
|   +-- CommandLineOptions.cs
|   +-- CommandLineParser.cs
|   +-- ConsoleOutput.cs
|   +-- HelpFormatter.cs
+-- AsciiArt.Core/
|   +-- IAsciiFont.cs
|   +-- RenderResult.cs
|   +-- AsciiRenderer.cs
+-- AsciiArt.Fonts/
    +-- BasicBlockFont.cs
    +-- BigMoneyNeFont.cs
    +-- CaligraphyFont.cs
    +-- FigletFontParser.cs
    +-- FontRegistry.cs
    +-- Resources/BigMoney-ne.flf
    +-- Resources/Block.flf
    +-- Resources/Caligraphy.flf

tests/
+-- AsciiArt.Core.Tests/
|   +-- AsciiRendererTests.cs
|   +-- FontTests.cs
|   +-- Fixtures/MockFont.cs
+-- AsciiArt.Cli.Tests/
    +-- ArgumentParsingTests.cs
    +-- ApplicationExecutionTests.cs
```

## Design Decisions (Implemented)

1. **Three-layer separation (CLI/Core/Fonts)**
- Keeps rendering logic independent from console I/O
- Allows direct unit testing of core behavior

2. **`IAsciiFont` abstraction**
- Multiple font implementations with a single renderer
- New fonts can be added by registering in `FontRegistry`

3. **Embedded FIGlet resources for built-in styles**
- `Resources/*.flf` is bundled in assembly resources
- `FigletFontParser` translates FIGlet data into glyph maps

4. **Explicit CLI exit-code contract**
- `0`: success
- `1`: runtime/validation error
- `2`: usage/parsing error

## Constitution Check (Current)

- **Code Quality**: PASS (modular and documented public APIs)
- **Testing**: PASS (unit + integration tests implemented)
- **UX Consistency**: PASS (uniform help/error formatting)
- **Performance**: PASS (tests include <200ms renderer check for standard input)

## Notes

- Original planning docs targeted 80-column output and a basic block default font.
- Current implementation intentionally uses `big-money-ne` as default and width cap 300 to support the required `Hello world` banner style.
