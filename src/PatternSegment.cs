namespace Philiprehberger.GlobMatcher;

/// <summary>
/// Base type for all glob pattern segments.
/// </summary>
internal abstract record PatternSegment;

/// <summary>
/// Matches an exact literal string within a path segment.
/// </summary>
/// <param name="Text">The literal text to match.</param>
internal sealed record LiteralSegment(string Text) : PatternSegment;

/// <summary>
/// Matches any characters within a single path segment (the <c>*</c> pattern).
/// Also handles the <c>?</c> single-character wildcard via a composed pattern string.
/// </summary>
/// <param name="Pattern">The wildcard pattern (e.g. <c>*.txt</c>, <c>file?.cs</c>).</param>
internal sealed record WildcardSegment(string Pattern) : PatternSegment;

/// <summary>
/// Matches zero or more path segments (the <c>**</c> pattern).
/// </summary>
internal sealed record GlobstarSegment : PatternSegment;

/// <summary>
/// Matches a single character from a set or range (e.g. <c>[abc]</c> or <c>[!abc]</c>).
/// </summary>
/// <param name="Characters">The characters in the class.</param>
/// <param name="Negated">Whether the class is negated (<c>[!...]</c>).</param>
internal sealed record CharClassSegment(string Characters, bool Negated) : PatternSegment;

/// <summary>
/// Represents a brace expansion group (e.g. <c>{a,b,c}</c>) that expands into multiple alternative segment lists.
/// </summary>
/// <param name="Alternatives">The list of alternative segment lists.</param>
internal sealed record PatternGroup(List<List<PatternSegment>> Alternatives) : PatternSegment;
