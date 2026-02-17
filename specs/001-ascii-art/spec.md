# Feature Specification: ASCII Text Art Command-Line Tool

**Feature Branch**: `001-ascii-art`  
**Created**: 2026-02-17  
**Last Updated**: 2026-02-17  
**Status**: Implemented (aligned with current codebase)

## User Scenarios & Testing

### User Story 1 - Generate ASCII Banner from Text (Priority: P1)

As a developer, I want to convert plain text into ASCII art so I can create visually styled banners for terminal output, documentation headers, and scripts.

**Independent Test**: run `asciiart "Hello world"` and verify the banner is rendered using the default style (`big-money-ne`).

**Acceptance Scenarios**:

1. **Given** the tool is installed, **When** the user runs `asciiart "Hello world"`, **Then** the command prints a multi-line ASCII banner to stdout
2. **Given** valid short input, **When** conversion runs, **Then** execution completes under 200ms for typical strings (<= 20 chars)
3. **Given** input with spaces, **When** conversion runs, **Then** spaces are preserved in the rendered output

---

### User Story 2 - CLI with Consistent Interface (Priority: P2)

As a terminal user, I want standard command-line options so I can use the tool interactively and in scripts.

**Independent Test**: verify `--help`, `--font`, `--strict`, and `--list-fonts` behavior with expected exit codes.

**Acceptance Scenarios**:

1. **Given** the user runs `asciiart --help`, **When** the command executes, **Then** usage and options are printed and exit code is 0
2. **Given** the user runs `asciiart --list-fonts`, **When** the command executes, **Then** all registered fonts are printed and exit code is 0
3. **Given** the user runs `asciiart --font basicblock "Hello"`, **When** the font exists, **Then** output is rendered using that font
4. **Given** the user runs `asciiart --font caligraphy "Hello"`, **When** the font exists, **Then** output is rendered using that font

---

### User Story 3 - Error Handling and User Guidance (Priority: P3)

As a new user, I want clear errors and warnings so I can quickly correct input problems.

**Independent Test**: run the tool with no input, invalid options, unsupported chars, and strict mode.

**Acceptance Scenarios**:

1. **Given** missing text input, **When** command executes, **Then** a formatted error is printed to stderr and exit code is 1
2. **Given** an unknown option, **When** command executes, **Then** a usage error is printed and exit code is 2
3. **Given** unsupported characters and `--strict`, **When** command executes, **Then** rendering fails with an actionable error and exit code is 1
4. **Given** unsupported characters without `--strict`, **When** command executes, **Then** output is rendered with placeholders and warnings are printed to stderr

---

### User Story 4 - Legible Output in Terminal Constraints (Priority: P2)

As a technical writer, I want output that is legible and bounded so terminal display remains practical.

**Independent Test**: verify renderer enforces width and height limits for generated output.

**Acceptance Scenarios**:

1. **Given** valid input, **When** rendering completes, **Then** output height does not exceed 24 lines
2. **Given** very long or wide output, **When** rendering exceeds max width, **Then** renderer returns a failure result
3. **Given** normal usage, **When** output is generated, **Then** lines stay within configured width constraints

---

### Edge Cases

- Empty or whitespace-only input
- Input longer than 40 characters
- Unsupported Unicode characters (for example emoji)
- Unknown command-line options
- Unknown font name passed through `--font`

## Requirements

### Functional Requirements

- **FR-001**: System MUST accept text via command line using `asciiart [options] <text>`
- **FR-002**: System MUST output rendered ASCII art to stdout
- **FR-003**: System MUST preserve spaces from input text
- **FR-004**: System MUST support `--help` and `-h`
- **FR-005**: System MUST support `--font <name>` with case-insensitive lookup
- **FR-006**: System MUST support `--list-fonts`
- **FR-007**: System MUST support `--strict` to fail on unsupported characters
- **FR-008**: System MUST return exit code 0 on success, 1 on runtime/validation errors, and 2 on usage errors
- **FR-009**: System MUST reject empty text input
- **FR-010**: System MUST reject text longer than 40 characters in CLI validation
- **FR-011**: System MUST enforce renderer limits of width <= 300 and height <= 24
- **FR-012**: System MUST print warnings for unsupported characters in non-strict mode

### Key Entities

- **Font**: glyph provider (`IAsciiFont`) with name, height, support check, and placeholder
- **RenderResult**: rendered lines, warnings, and success flag
- **CommandLineOptions**: parsed CLI state (flags, text, font, validity)
- **FontRegistry**: registered fonts and default font selection

## Success Criteria

- **SC-001**: `asciiart "Hello world"` prints the expected `big-money-ne` banner
- **SC-002**: `asciiart --help` returns 0 and prints usage/options text
- **SC-003**: `asciiart --list-fonts` returns 0 and includes `big-money-ne`, `basicblock`, and `caligraphy`
- **SC-004**: Missing text returns exit code 1 with actionable error format
- **SC-005**: Unknown option returns exit code 2 with actionable usage message
- **SC-006**: Automated tests pass for renderer behavior, parser behavior, and CLI integration

## Assumptions

- App targets current local SDK/runtime configuration (`net6.0` in this repository)
- Output is plain text and terminal-safe (no color control sequences)
- Default style is `big-money-ne` (FIGlet-based)
- Additional built-in styles are `basicblock` and `caligraphy` (FIGlet-based)

## Out of Scope

- GUI support
- Rich color output
- Interactive prompt mode
- Runtime font downloads from external services
