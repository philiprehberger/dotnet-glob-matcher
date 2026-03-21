namespace Philiprehberger.GlobMatcher;

/// <summary>
/// Parses a glob pattern string into a list of <see cref="PatternSegment"/> instances.
/// </summary>
internal static class GlobParser
{
    /// <summary>
    /// Parses the given glob pattern into segments split by path separator.
    /// </summary>
    /// <param name="pattern">The glob pattern to parse.</param>
    /// <returns>A list of pattern segments.</returns>
    internal static List<PatternSegment> Parse(string pattern)
    {
        pattern = pattern.Replace('\\', '/');

        // Handle brace expansion at the top level
        var expanded = ExpandBraces(pattern);
        if (expanded.Count > 1)
        {
            var alternatives = expanded
                .Select(Parse)
                .Select(segments => segments)
                .ToList();
            return [new PatternGroup(alternatives)];
        }

        var parts = pattern.Split('/');
        var segments = new List<PatternSegment>();

        foreach (var part in parts)
        {
            segments.Add(ParseSegment(part));
        }

        return segments;
    }

    private static PatternSegment ParseSegment(string part)
    {
        if (part == "**")
        {
            return new GlobstarSegment();
        }

        if (part.StartsWith('[') && part.EndsWith(']') && part.Length >= 3)
        {
            var inner = part[1..^1];
            var negated = inner.StartsWith('!');
            var chars = negated ? inner[1..] : inner;
            return new CharClassSegment(chars, negated);
        }

        if (part.Contains('*') || part.Contains('?'))
        {
            return new WildcardSegment(part);
        }

        return new LiteralSegment(part);
    }

    private static List<string> ExpandBraces(string pattern)
    {
        var braceStart = -1;
        var depth = 0;

        for (var i = 0; i < pattern.Length; i++)
        {
            switch (pattern[i])
            {
                case '{' when depth == 0:
                    braceStart = i;
                    depth++;
                    break;
                case '{':
                    depth++;
                    break;
                case '}' when depth == 1:
                {
                    var before = pattern[..braceStart];
                    var after = pattern[(i + 1)..];
                    var content = pattern[(braceStart + 1)..i];
                    var alternatives = SplitBraceContent(content);

                    var results = new List<string>();
                    foreach (var alt in alternatives)
                    {
                        var expanded = ExpandBraces(before + alt + after);
                        results.AddRange(expanded);
                    }

                    return results;
                }
                case '}':
                    depth--;
                    break;
            }
        }

        return [pattern];
    }

    private static List<string> SplitBraceContent(string content)
    {
        var parts = new List<string>();
        var depth = 0;
        var start = 0;

        for (var i = 0; i < content.Length; i++)
        {
            switch (content[i])
            {
                case '{':
                    depth++;
                    break;
                case '}':
                    depth--;
                    break;
                case ',' when depth == 0:
                    parts.Add(content[start..i]);
                    start = i + 1;
                    break;
            }
        }

        parts.Add(content[start..]);
        return parts;
    }
}
