using Rowdy.TUnit;
using TUnit.Assertions.Core;

namespace Rowdy;

/// <summary>
/// Contains extension methods for test assertions.
/// </summary>
public static class AssertionExtensions
{
    /// <summary>
    /// Asserts that a Rowdy server received a certain number of calls.
    /// </summary>
    /// <param name="assertionSource">The Rowdy server.</param>
    /// <param name="expected">The number of calls expected to have been received.</param>
    /// <returns>A TUnit assertion.</returns>
    public static ReceivedAssertion<T> Received<T>(
        this IAssertionSource<T> assertionSource,
        int expected
    )
        where T : IRowdyServer => new(assertionSource.Context, expected);
}
