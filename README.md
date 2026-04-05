# Philiprehberger.GlobMatcher

[![CI](https://github.com/philiprehberger/dotnet-glob-matcher/actions/workflows/ci.yml/badge.svg)](https://github.com/philiprehberger/dotnet-glob-matcher/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/Philiprehberger.GlobMatcher.svg)](https://www.nuget.org/packages/Philiprehberger.GlobMatcher)
[![Last updated](https://img.shields.io/github/last-commit/philiprehberger/dotnet-glob-matcher)](https://github.com/philiprehberger/dotnet-glob-matcher/commits/main)

Match file paths against glob patterns — supports *, **, ?, character classes, brace expansion, and negation.

## Installation

```bash
dotnet add package Philiprehberger.GlobMatcher
```

## Usage

```csharp
using Philiprehberger.GlobMatcher;

var matched = Glob.IsMatch("src/**/*.cs", "src/Models/User.cs");
// true
```

### Match Single Path

```csharp
using Philiprehberger.GlobMatcher;

Glob.IsMatch("*.txt", "readme.txt");       // true
Glob.IsMatch("src/*.cs", "src/Program.cs"); // true
Glob.IsMatch("**/*.json", "a/b/c.json");   // true
Glob.IsMatch("file?.log", "file1.log");     // true
Glob.IsMatch("[abc].txt", "b.txt");         // true
```

### Filter Collections

```csharp
using Philiprehberger.GlobMatcher;

var files = new[] { "app.cs", "app.js", "lib.cs", "readme.md" };

var csFiles = Glob.Match("*.cs", files);
// ["app.cs", "lib.cs"]

var filtered = Glob.Filter("*.cs", files);
// ["app.cs", "lib.cs"]  (alias for Match)
```

### Compiled Patterns

Pre-compile a pattern for efficient reuse across multiple match operations:

```csharp
using Philiprehberger.GlobMatcher;

var compiled = Glob.Compile("src/**/*.cs");

compiled.IsMatch("src/Models/User.cs"); // true
compiled.IsMatch("test/foo.cs");        // false

var files = new[] { "src/App.cs", "src/readme.md", "src/Lib.cs" };
var csFiles = compiled.Filter(files);
// ["src/App.cs", "src/Lib.cs"]
```

With options:

```csharp
var compiled = Glob.Compile("*.CS", new GlobOptions(CaseSensitive: false));
compiled.IsMatch("App.cs"); // true
```

### Negation Patterns

```csharp
using Philiprehberger.GlobMatcher;

Glob.IsMatch("!*.log", "data.csv");  // true
Glob.IsMatch("!*.log", "error.log"); // false
```

## API

### `Glob`

| Method | Description |
|--------|-------------|
| `IsMatch(string pattern, string path)` | Test if a path matches a glob pattern |
| `IsMatch(string pattern, string path, GlobOptions options)` | Test with custom options |
| `Match(string pattern, IEnumerable<string> paths)` | Return all matching paths |
| `Filter(string pattern, IEnumerable<string> paths)` | Alias for `Match` |
| `Compile(string pattern, GlobOptions? options)` | Pre-compile a pattern for reuse |

### `CompiledGlob`

| Method | Description |
|--------|-------------|
| `IsMatch(string path)` | Test if a path matches the compiled pattern |
| `Filter(IEnumerable<string> paths)` | Return all matching paths |
| `Pattern` | The original pattern string |

### `GlobOptions`

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `CaseSensitive` | `bool` | `true` | Whether matching is case-sensitive |
| `PathSeparator` | `char` | `'/'` | Path separator character |

## Development

```bash
dotnet build src/Philiprehberger.GlobMatcher.csproj --configuration Release
```

## Support

If you find this project useful:

⭐ [Star the repo](https://github.com/philiprehberger/dotnet-glob-matcher)

🐛 [Report issues](https://github.com/philiprehberger/dotnet-glob-matcher/issues?q=is%3Aissue+is%3Aopen+label%3Abug)

💡 [Suggest features](https://github.com/philiprehberger/dotnet-glob-matcher/issues?q=is%3Aissue+is%3Aopen+label%3Aenhancement)

❤️ [Sponsor development](https://github.com/sponsors/philiprehberger)

🌐 [All Open Source Projects](https://philiprehberger.com/open-source-packages)

💻 [GitHub Profile](https://github.com/philiprehberger)

🔗 [LinkedIn Profile](https://www.linkedin.com/in/philiprehberger)

## License

[MIT](LICENSE)
