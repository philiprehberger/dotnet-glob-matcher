namespace Philiprehberger.GlobMatcher;

/// <summary>
/// Matches a normalized file path against a list of parsed glob pattern segments.
/// </summary>
internal static class GlobMatcherEngine
{
    /// <summary>
    /// Determines whether the given path segments match the pattern segments.
    /// </summary>
    /// <param name="patternSegments">The parsed pattern segments.</param>
    /// <param name="pathParts">The path split into segments.</param>
    /// <param name="caseSensitive">Whether matching is case-sensitive.</param>
    /// <returns><c>true</c> if the path matches the pattern; otherwise, <c>false</c>.</returns>
    internal static bool IsMatch(List<PatternSegment> patternSegments, string[] pathParts, bool caseSensitive)
    {
        // Handle PatternGroup (brace expansion) at top level
        if (patternSegments.Count == 1 && patternSegments[0] is PatternGroup group)
        {
            return group.Alternatives.Any(alt => IsMatch(alt, pathParts, caseSensitive));
        }

        return MatchSegments(patternSegments, 0, pathParts, 0, caseSensitive);
    }

    private static bool MatchSegments(
        List<PatternSegment> pattern,
        int patternIndex,
        string[] path,
        int pathIndex,
        bool caseSensitive)
    {
        while (patternIndex < pattern.Count && pathIndex < path.Length)
        {
            var segment = pattern[patternIndex];

            switch (segment)
            {
                case GlobstarSegment:
                {
                    // ** matches zero or more path segments
                    // Try matching zero segments (skip the **)
                    if (MatchSegments(pattern, patternIndex + 1, path, pathIndex, caseSensitive))
                    {
                        return true;
                    }

                    // Try matching one or more segments (consume current path segment)
                    return MatchSegments(pattern, patternIndex, path, pathIndex + 1, caseSensitive);
                }
                case PatternGroup group:
                {
                    return group.Alternatives.Any(alt =>
                    {
                        var combined = new List<PatternSegment>(alt);
                        combined.AddRange(pattern.Skip(patternIndex + 1));
                        return MatchSegments(combined, 0, path, pathIndex, caseSensitive);
                    });
                }
                default:
                {
                    if (!MatchSingleSegment(segment, path[pathIndex], caseSensitive))
                    {
                        return false;
                    }

                    patternIndex++;
                    pathIndex++;
                    break;
                }
            }
        }

        // Skip trailing globstars
        while (patternIndex < pattern.Count && pattern[patternIndex] is GlobstarSegment)
        {
            patternIndex++;
        }

        return patternIndex == pattern.Count && pathIndex == path.Length;
    }

    private static bool MatchSingleSegment(PatternSegment segment, string pathPart, bool caseSensitive)
    {
        return segment switch
        {
            LiteralSegment literal => StringEquals(literal.Text, pathPart, caseSensitive),
            WildcardSegment wildcard => MatchWildcard(wildcard.Pattern, pathPart, caseSensitive),
            CharClassSegment charClass => pathPart.Length == 1 && MatchCharClass(charClass, pathPart[0], caseSensitive),
            _ => false
        };
    }

    private static bool MatchWildcard(string pattern, string text, bool caseSensitive)
    {
        return MatchWildcardRecursive(pattern, 0, text, 0, caseSensitive);
    }

    private static bool MatchWildcardRecursive(string pattern, int pi, string text, int ti, bool caseSensitive)
    {
        while (pi < pattern.Length && ti < text.Length)
        {
            var pc = pattern[pi];

            switch (pc)
            {
                case '*':
                {
                    // Try matching zero characters or one character
                    if (MatchWildcardRecursive(pattern, pi + 1, text, ti, caseSensitive))
                    {
                        return true;
                    }

                    return MatchWildcardRecursive(pattern, pi, text, ti + 1, caseSensitive);
                }
                case '?':
                    pi++;
                    ti++;
                    break;
                case '[':
                {
                    var closeBracket = pattern.IndexOf(']', pi + 1);
                    if (closeBracket < 0)
                    {
                        return false;
                    }

                    var inner = pattern[(pi + 1)..closeBracket];
                    var negated = inner.StartsWith('!');
                    var chars = negated ? inner[1..] : inner;
                    var charClass = new CharClassSegment(chars, negated);

                    if (!MatchCharClass(charClass, text[ti], caseSensitive))
                    {
                        return false;
                    }

                    pi = closeBracket + 1;
                    ti++;
                    break;
                }
                default:
                {
                    if (!CharEquals(pc, text[ti], caseSensitive))
                    {
                        return false;
                    }

                    pi++;
                    ti++;
                    break;
                }
            }
        }

        // Consume remaining * in pattern
        while (pi < pattern.Length && pattern[pi] == '*')
        {
            pi++;
        }

        return pi == pattern.Length && ti == text.Length;
    }

    private static bool MatchCharClass(CharClassSegment charClass, char c, bool caseSensitive)
    {
        var matches = charClass.Characters.Any(cc => CharEquals(cc, c, caseSensitive));
        return charClass.Negated ? !matches : matches;
    }

    private static bool CharEquals(char a, char b, bool caseSensitive)
    {
        return caseSensitive ? a == b : char.ToLowerInvariant(a) == char.ToLowerInvariant(b);
    }

    private static bool StringEquals(string a, string b, bool caseSensitive)
    {
        return caseSensitive
            ? string.Equals(a, b, StringComparison.Ordinal)
            : string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
    }
}
