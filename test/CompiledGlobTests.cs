using Philiprehberger.GlobMatcher;
using Xunit;

namespace Philiprehberger.GlobMatcher.Tests;

public class CompiledGlobTests
{
    [Fact]
    public void Compile_ReturnsCompiledGlob()
    {
        var compiled = Glob.Compile("**/*.cs");
        Assert.NotNull(compiled);
    }

    [Fact]
    public void IsMatch_MatchesSimpleWildcard()
    {
        var compiled = Glob.Compile("*.txt");
        Assert.True(compiled.IsMatch("readme.txt"));
        Assert.False(compiled.IsMatch("readme.md"));
    }

    [Fact]
    public void IsMatch_MatchesGlobstar()
    {
        var compiled = Glob.Compile("src/**/*.cs");
        Assert.True(compiled.IsMatch("src/Models/User.cs"));
        Assert.True(compiled.IsMatch("src/a/b/c.cs"));
        Assert.False(compiled.IsMatch("test/foo.cs"));
    }

    [Fact]
    public void IsMatch_MatchesSingleCharWildcard()
    {
        var compiled = Glob.Compile("file?.log");
        Assert.True(compiled.IsMatch("file1.log"));
        Assert.True(compiled.IsMatch("fileA.log"));
        Assert.False(compiled.IsMatch("file12.log"));
    }

    [Fact]
    public void IsMatch_MatchesCharacterClass()
    {
        var compiled = Glob.Compile("[abc].txt");
        Assert.True(compiled.IsMatch("a.txt"));
        Assert.True(compiled.IsMatch("b.txt"));
        Assert.False(compiled.IsMatch("d.txt"));
    }

    [Fact]
    public void IsMatch_HandlesNegation()
    {
        var compiled = Glob.Compile("!*.log");
        Assert.True(compiled.IsMatch("data.csv"));
        Assert.False(compiled.IsMatch("error.log"));
    }

    [Fact]
    public void IsMatch_HandlesBraceExpansion()
    {
        var compiled = Glob.Compile("*.{cs,js}");
        Assert.True(compiled.IsMatch("app.cs"));
        Assert.True(compiled.IsMatch("app.js"));
        Assert.False(compiled.IsMatch("app.py"));
    }

    [Fact]
    public void IsMatch_CaseInsensitiveOption()
    {
        var compiled = Glob.Compile("*.CS", new GlobOptions(CaseSensitive: false));
        Assert.True(compiled.IsMatch("App.cs"));
        Assert.True(compiled.IsMatch("App.CS"));
    }

    [Fact]
    public void Filter_ReturnsOnlyMatchingPaths()
    {
        var compiled = Glob.Compile("*.cs");
        var files = new[] { "app.cs", "app.js", "lib.cs", "readme.md" };
        var result = compiled.Filter(files).ToList();
        Assert.Equal(new[] { "app.cs", "lib.cs" }, result);
    }

    [Fact]
    public void Filter_EmptyCollectionReturnsEmpty()
    {
        var compiled = Glob.Compile("*.cs");
        var result = compiled.Filter(Array.Empty<string>()).ToList();
        Assert.Empty(result);
    }

    [Fact]
    public void Pattern_ReturnsOriginalPattern()
    {
        var compiled = Glob.Compile("src/**/*.cs");
        Assert.Equal("src/**/*.cs", compiled.Pattern);
    }

    [Fact]
    public void Pattern_ReturnsNegatedPattern()
    {
        var compiled = Glob.Compile("!*.log");
        Assert.Equal("!*.log", compiled.Pattern);
    }

    [Fact]
    public void CompiledGlob_ProducesSameResultsAsStaticMethods()
    {
        var pattern = "src/**/*.cs";
        var paths = new[]
        {
            "src/Program.cs", "src/Models/User.cs", "test/Test.cs",
            "src/readme.md", "src/a/b/c.cs"
        };

        var compiled = Glob.Compile(pattern);
        foreach (var path in paths)
        {
            Assert.Equal(Glob.IsMatch(pattern, path), compiled.IsMatch(path));
        }

        var staticResult = Glob.Match(pattern, paths).ToList();
        var compiledResult = compiled.Filter(paths).ToList();
        Assert.Equal(staticResult, compiledResult);
    }
}
