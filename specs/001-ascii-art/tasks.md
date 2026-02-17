---
description: "Implementation tasks and current status for ASCII Text Art Command-Line Tool"
---

# Tasks: ASCII Text Art Command-Line Tool

**Input**: `/specs/001-ascii-art/`  
**Branch**: `001-ascii-art`  
**Status**: Updated to reflect current implementation

## Phase 1 - Solution and Project Setup

- [x] Create solution `AsciiArt.sln`
- [x] Create projects: `AsciiArt.Cli`, `AsciiArt.Core`, `AsciiArt.Fonts`
- [x] Create test projects: `AsciiArt.Cli.Tests`, `AsciiArt.Core.Tests`
- [x] Add project references and test dependencies
- [x] Add root `.gitignore` and `README.md`

## Phase 2 - Core Rendering Model

- [x] Define `IAsciiFont`
- [x] Define `RenderResult`
- [x] Implement `AsciiRenderer.Render(text, font, strict)`
- [x] Implement unsupported-character fallback and strict-mode failure
- [x] Enforce renderer constraints (`MaxOutputWidth=300`, `MaxOutputHeight=24`)

## Phase 3 - Font Implementations

- [x] Implement `BasicBlockFont`
- [x] Implement `BigMoneyNeFont` (FIGlet-based)
- [x] Add `FigletFontParser`
- [x] Embed `BigMoney-ne.flf` as resource
- [x] Register fonts in `FontRegistry`
- [x] Set default font to `big-money-ne`

## Phase 4 - CLI Flow

- [x] Implement `CommandLineOptions`
- [x] Implement `CommandLineParser`
- [x] Implement `ConsoleOutput`
- [x] Implement `HelpFormatter`
- [x] Implement `AsciiArtApplication` orchestration
- [x] Implement `Program.Main`

## Phase 5 - CLI Contract Features

- [x] `--help`, `-h`
- [x] `--font <name>`
- [x] `--strict`
- [x] `--list-fonts`
- [x] Exit codes `0/1/2`
- [x] Error format with usage hint
- [x] Input validation: empty and >40 chars

## Phase 6 - Tests

- [x] Renderer unit tests (concatenation, spacing, strict mode, limits, perf)
- [x] Font tests (registry and fallback behavior)
- [x] CLI parser tests (valid/invalid options)
- [x] CLI execution tests (help, errors, strict, list-fonts)
- [x] Exact banner test for `Hello world`

## Phase 7 - Remaining Improvements (Optional)

- [ ] Add publish profile / release automation
- [ ] Add CI workflow for build + test
- [ ] Add snapshot tests for all built-in fonts across full charset
- [ ] Add benchmark project for startup + rendering

## Validation Snapshot

- Automated tests: passing (`dotnet test AsciiArt.sln --no-restore`)
- Manual smoke test: `dotnet run --project src/AsciiArt.Cli -- "Hello world"` matches expected banner style
