namespace Philiprehberger.GlobMatcher;

/// <summary>
/// Options for configuring glob pattern matching behavior.
/// </summary>
/// <param name="CaseSensitive">Whether matching is case-sensitive. Defaults to <c>true</c>.</param>
/// <param name="PathSeparator">The path separator character. Defaults to <c>'/'</c>.</param>
public record GlobOptions(bool CaseSensitive = true, char PathSeparator = '/');
