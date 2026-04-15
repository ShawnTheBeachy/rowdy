namespace Rowdy.Matching;

/// <summary>
/// Matches a string.
/// </summary>
public interface IStringMatcher
{
    /// <summary>
    /// Checks whether a string matches.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns>Whether the string is a match.</returns>
    bool IsMatch(string? value);
}
