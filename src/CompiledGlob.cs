namespace Philiprehberger.GlobMatcher;

/// <summary>
/// A pre-compiled glob pattern that can be reused for multiple match operations
/// without re-parsing the pattern each time.
/// </summary>
public sealed class CompiledGlob
{
    private readonly string _normalizedPattern;
    private readonly List<PatternSegment> _segments;
    private readonly bool _negated;
    private readonly GlobOptions _options;

    internal CompiledGlob(string pattern, GlobOptions options)
    {
        _options = options;

        var normalized = pattern.Replace('\\', '/').Replace(options.PathSeparator, '/');

        if (normalized.StartsWith('!'))
        {
            _negated = true;
            normalized = normalized[1..];
        }

        _normalizedPattern = normalized;
        _segments = GlobParser.Parse(normalized);
    }

    /// <summary>
    /// The original pattern this compiled glob was created from.
    /// </summary>
    public string Pattern => _negated ? $"!{_normalizedPattern}" : _normalizedPattern;

    /// <summary>
    /// Determines whether the specified path matches this compiled glob pattern.
    /// </summary>
    /// <param name="path">The file path to test.</param>
    /// <returns><c>true</c> if the path matches the pattern; otherwise, <c>false</c>.</returns>
    public bool IsMatch(string path)
    {
        var normalizedPath = path.Replace('\\', '/').Replace(_options.PathSeparator, '/');
        var pathParts = normalizedPath.Split('/');
        var result = GlobMatcherEngine.IsMatch(_segments, pathParts, _options.CaseSensitive);
        return _negated ? !result : result;
    }

    /// <summary>
    /// Returns all paths from the input collection that match this compiled glob pattern.
    /// </summary>
    /// <param name="paths">The collection of file paths to filter.</param>
    /// <returns>An enumerable of paths that match the pattern.</returns>
    public IEnumerable<string> Filter(IEnumerable<string> paths)
    {
        return paths.Where(IsMatch);
    }
}
