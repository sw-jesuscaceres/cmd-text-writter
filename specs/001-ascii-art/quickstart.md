# Quickstart: ASCII Text Art Command-Line Tool

**Feature**: [001-ascii-art](../spec.md)  
**Updated**: 2026-02-17

## Prerequisites

- .NET SDK 6.x or compatible
- Repository root at `cmd-text-writter`

## Build

```bash
dotnet restore
dotnet build AsciiArt.sln
```

## Run

```bash
dotnet run --project src/AsciiArt.Cli -- "Hello world"
```

This uses default font `big-money-ne` and should produce the multi-line `$` banner style.

## CLI Usage

```bash
asciiart [options] <text>
```

Supported options:

- `--help`, `-h`
- `--font <name>`
- `--strict`
- `--list-fonts`

Current built-in fonts:

- `big-money-ne` (11 lines, default)
- `basicblock` (8 lines)
- `caligraphy` (21 lines)

Examples:

```bash
dotnet run --project src/AsciiArt.Cli -- --help
dotnet run --project src/AsciiArt.Cli -- --list-fonts
dotnet run --project src/AsciiArt.Cli -- --font basicblock "Hello"
dotnet run --project src/AsciiArt.Cli -- --font caligraphy "Hello"
dotnet run --project src/AsciiArt.Cli -- --strict "Hi<unicode>"
```

## Constraints

- Max input length (CLI): 40 characters
- Max output width (renderer): 300 characters
- Max output height (renderer): 24 lines

## Tests

```bash
dotnet test AsciiArt.sln --no-restore
```

## Architecture Snapshot

```text
AsciiArt.Cli   -> parser, help/error formatting, stream output
AsciiArt.Core  -> renderer + result model
AsciiArt.Fonts -> font implementations + registry + FIGlet parser
```

## Key Files

- `src/AsciiArt.Cli/AsciiArtApplication.cs`
- `src/AsciiArt.Core/AsciiRenderer.cs`
- `src/AsciiArt.Fonts/FontRegistry.cs`
- `src/AsciiArt.Fonts/BigMoneyNeFont.cs`
- `src/AsciiArt.Fonts/FigletFontParser.cs`
