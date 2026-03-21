namespace Philiprehberger.GlobMatcher;

/// <summary>
/// Provides static methods for matching file paths against glob patterns.
/// Supports <c>*</c> (any within segment), <c>**</c> (any depth), <c>?</c> (single char),
/// <c>[abc]</c> (character class), <c>[!abc]</c> (negated class), and <c>{a,b}</c> (brace expansion).
/// </summary>
public static class Glob
{
    /// <summary>
    /// Determines whether the specified path matches the glob pattern using default options.
    /// </summary>
    /// <param name="pattern">The glob pattern to match against.</param>
    /// <param name="path">The file path to test.</param>
    /// <returns><c>true</c> if the path matches the pattern; otherwise, <c>false</c>.</returns>
    public static bool IsMatch(string pattern, string path)
    {
        return IsMatch(pattern, path, new GlobOptions());
    }

    /// <summary>
    /// Determines whether the specified path matches the glob pattern using the given options.
    /// </summary>
    /// <param name="pattern">The glob pattern to match against.</param>
    /// <param name="path">The file path to test.</param>
    /// <param name="options">Options controlling match behavior.</param>
    /// <returns><c>true</c> if the path matches the pattern; otherwise, <c>false</c>.</returns>
    public static bool IsMatch(string pattern, string path, GlobOptions options)
    {
        var normalizedPath = NormalizePath(path, options.PathSeparator);
        var normalizedPattern = NormalizePath(pattern, options.PathSeparator);

        // Handle negation
        if (normalizedPattern.StartsWith('!'))
        {
            return !IsMatchInternal(normalizedPattern[1..], normalizedPath, options.CaseSensitive);
        }

        return IsMatchInternal(normalizedPattern, normalizedPath, options.CaseSensitive);
    }

    /// <summary>
    /// Returns all paths from the input collection that match the glob pattern.
    /// </summary>
    /// <param name="pattern">The glob pattern to match against.</param>
    /// <param name="paths">The collection of file paths to filter.</param>
    /// <returns>An enumerable of paths that match the pattern.</returns>
    public static IEnumerable<string> Match(string pattern, IEnumerable<string> paths)
    {
        return Match(pattern, paths, new GlobOptions());
    }

    /// <summary>
    /// Returns all paths from the input collection that match the glob pattern using the given options.
    /// </summary>
    /// <param name="pattern">The glob pattern to match against.</param>
    /// <param name="paths">The collection of file paths to filter.</param>
    /// <param name="options">Options controlling match behavior.</param>
    /// <returns>An enumerable of paths that match the pattern.</returns>
    public static IEnumerable<string> Match(string pattern, IEnumerable<string> paths, GlobOptions options)
    {
        return paths.Where(p => IsMatch(pattern, p, options));
    }

    /// <summary>
    /// Returns all paths from the input collection that match the glob pattern.
    /// This is an alias for <see cref="Match(string, IEnumerable{string})"/>.
    /// </summary>
    /// <param name="pattern">The glob pattern to match against.</param>
    /// <param name="paths">The collection of file paths to filter.</param>
    /// <returns>An enumerable of paths that match the pattern.</returns>
    public static IEnumerable<string> Filter(string pattern, IEnumerable<string> paths)
    {
        return Match(pattern, paths);
    }

    /// <summary>
    /// Returns all paths from the input collection that match the glob pattern using the given options.
    /// This is an alias for <see cref="Match(string, IEnumerable{string}, GlobOptions)"/>.
    /// </summary>
    /// <param name="pattern">The glob pattern to match against.</param>
    /// <param name="paths">The collection of file paths to filter.</param>
    /// <param name="options">Options controlling match behavior.</param>
    /// <returns>An enumerable of paths that match the pattern.</returns>
    public static IEnumerable<string> Filter(string pattern, IEnumerable<string> paths, GlobOptions options)
    {
        return Match(pattern, paths, options);
    }

    private static bool IsMatchInternal(string pattern, string path, bool caseSensitive)
    {
        var segments = GlobParser.Parse(pattern);
        var pathParts = path.Split('/');
        return GlobMatcherEngine.IsMatch(segments, pathParts, caseSensitive);
    }

    private static string NormalizePath(string path, char separator)
    {
        return path.Replace('\\', '/').Replace(separator, '/');
    }
}
