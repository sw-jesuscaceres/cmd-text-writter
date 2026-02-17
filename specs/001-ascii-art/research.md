# Research: ASCII Text Art Command-Line Tool

**Phase**: Post-implementation alignment  
**Date**: 2026-02-17  
**Status**: Completed

## Objective

Record the decisions reflected by the current implementation and replace pre-implementation assumptions that no longer match runtime behavior.

## Implemented Decisions

### 1. Runtime target

- **Decision**: target `net6.0`
- **Reason**: local environment and project scaffolding are currently .NET 6-based
- **Impact**: cross-platform CLI behavior remains unchanged

### 2. Default font style

- **Decision**: use FIGlet-based `big-money-ne` as default font
- **Reason**: required output style for `Hello world`
- **Implementation**:
  - Embedded resource: `Resources/BigMoney-ne.flf`
  - Parser: `FigletFontParser`
  - Font class: `BigMoneyNeFont`

### 3. Width constraint

- **Decision**: renderer max width set to 300
- **Reason**: default FIGlet banners can exceed 80 columns (for example, `Hello world` is wider than 80)
- **Impact**: app supports richer banner styles while still enforcing a hard upper bound

### 4. CLI options now active (not future)

- `--help`, `-h`
- `--font <name>`
- `--strict`
- `--list-fonts`

### 5. Error model

- **Exit code 0**: success
- **Exit code 1**: runtime/validation error
- **Exit code 2**: usage/parsing error
- Errors use a consistent message template from `HelpFormatter`

## Performance Notes

- Renderer remains `O(n * fontHeight)`
- Uses `StringBuilder` per output line
- Current tests include a baseline check: 20-char input renders under 200ms

## Risks and Mitigations

- **Risk**: FIGlet fonts vary in width significantly
  - **Mitigation**: keep a hard width cap (`MaxOutputWidth = 300`)
- **Risk**: unsupported Unicode input
  - **Mitigation**: placeholder fallback in default mode and hard fail in strict mode

## Conclusion

Current architecture is stable and consistent with implemented requirements. Documentation has been updated to match actual behavior and constraints.
