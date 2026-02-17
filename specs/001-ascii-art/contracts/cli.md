# CLI Contract: ASCII Text Art Command-Line Tool

**Phase**: Implementation-aligned contract  
**Date**: 2026-02-17  
**Feature**: [001-ascii-art](../spec.md)

## Command

```bash
asciiart [options] <text>
```

## Arguments

### Positional

- `text` (required)
  - must not be empty
  - max length: 40 chars

### Options

- `--help`, `-h`: print help, exit `0`
- `--font <name>`: select font by registry name (case-insensitive)
- `--strict`: fail on unsupported characters
- `--list-fonts`: list fonts, exit `0`

## Registered Fonts

- `big-money-ne` (default)
- `basicblock`

## Exit Codes

- `0`: success
- `1`: runtime/validation error
- `2`: usage/parsing error (unknown option, malformed option values)

## Output Streams

### Success

- ASCII art to `stdout`
- warnings (if any) to `stderr`

### Error

- error block to `stderr`
- nothing to `stdout`

Error format:

```text
Error: <message>

Usage: asciiart [options] <text>
Use --help for more information.
```

## Rendering Constraints

- Max output width: `300`
- Max output height: `24`

If limits are exceeded, rendering fails with an error.

## Behavioral Examples

### 1) Default banner

```bash
asciiart "Hello world"
```

Expected output style (first line):

```text
 /$$   /$$           /$$ /$$                                                   /$$       /$$
```

### 2) Help

```bash
asciiart --help
```

- prints usage/help text
- returns `0`

### 3) List fonts

```bash
asciiart --list-fonts
```

Example output:

```text
big-money-ne (11 lines)
basicblock (5 lines)
```

### 4) Unknown option

```bash
asciiart --badoption
```

- prints usage error to `stderr`
- returns `2`

### 5) Unsupported chars (non-strict)

```bash
asciiart "Hi<unicode>"
```

- renders with `?` placeholders
- prints warning to `stderr`
- returns `0`

### 6) Unsupported chars (strict)

```bash
asciiart --strict "Hi<unicode>"
```

- fails with error
- returns `1`

## Notes

- Multi-token positional text is joined with spaces (for example: `asciiart Hello world`).
- `--font` with unknown name returns error code `1` with available fonts listed.
