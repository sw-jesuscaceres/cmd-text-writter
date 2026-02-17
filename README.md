# ASCII Text Art CLI

`asciiart` is a cross-platform .NET command-line tool that renders text as multi-line ASCII art.

## Features

- Render text from CLI arguments: `asciiart "Hello"`
- Support for letters (`A-Z`, `a-z`), digits (`0-9`), and spaces
- Help output: `asciiart --help`
- Warning handling for unsupported characters (placeholder rendering)
- Strict mode: `asciiart --strict "Hi!"`
- Font listing: `asciiart --list-fonts`
- Terminal safety constraints: width `<= 80`, height `<= 24`

## Build

```bash
dotnet build AsciiArt.sln
```

## Run

```bash
dotnet run --project src/AsciiArt.Cli -- "Hello World"
dotnet run --project src/AsciiArt.Cli -- --help
```

## Test

```bash
dotnet test AsciiArt.sln
```

## Example

```bash
dotnet run --project src/AsciiArt.Cli -- "Hi!"
```

- ASCII art is written to `stdout`
- Warnings/errors are written to `stderr`
