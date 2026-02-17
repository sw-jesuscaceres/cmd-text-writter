# ASCII Text Art CLI

`asciiart` is a cross-platform .NET command-line tool that renders text as multi-line ASCII art.

## Features

- Render text from CLI arguments: `asciiart "Hello"`
- Built-in fonts: `big-money-ne` (default), `basicblock`, `caligraphy`
- Supports printable ASCII characters from the selected font
- Help output: `asciiart --help`
- Warning handling for unsupported characters (placeholder rendering)
- Strict mode: `asciiart --strict "Hi!"`
- Font listing: `asciiart --list-fonts`
- Font selection: `asciiart --font caligraphy "Hello"`
- Terminal safety constraints: width `<= 300`, height `<= 24`

## Build

```bash
dotnet build AsciiArt.sln
```

## Run

```bash
dotnet run --project src/AsciiArt.Cli -- "Hello World"
dotnet run --project src/AsciiArt.Cli -- --help
dotnet run --project src/AsciiArt.Cli -- --list-fonts
dotnet run --project src/AsciiArt.Cli -- --font caligraphy "Hello"
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
