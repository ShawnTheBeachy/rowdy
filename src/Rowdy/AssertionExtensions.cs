using Rowdy.TUnit;
using TUnit.Assertions.Core;

namespace Rowdy;

public static class AssertionExtensions
{
    public static ReceivedAssertion<T> Received<T>(
        this IAssertionSource<T> assertionSource,
        int expected
    )
        where T : IRowdyServer => new(assertionSource.Context, expected);
}
