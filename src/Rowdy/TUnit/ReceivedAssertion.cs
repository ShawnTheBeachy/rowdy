using Microsoft.AspNetCore.Http;
using TUnit.Assertions.Core;

namespace Rowdy.TUnit;

/// <summary>
/// A TUnit assertion to assert that a <c>RowdyServer</c> received a certain number of calls.
/// </summary>
public sealed partial class ReceivedAssertion : Assertion<RowdyServer>
{
    private readonly int _expected;
    private RequestMatcher _matcher = _ => true;

    internal ReceivedAssertion(AssertionContext<RowdyServer> context, int expected)
        : base(context)
    {
        _expected = expected;
    }

    /// <summary>
    /// Checks whether the assertion passes.
    /// </summary>
    /// <param name="metadata">Handled by TUnit.</param>
    protected override Task<AssertionResult> CheckAsync(EvaluationMetadata<RowdyServer> metadata)
    {
        var exception = metadata.Exception;

        if (exception != null)
            return Task.FromResult(AssertionResult.Failed($"threw {exception.GetType().Name}"));

        var server = metadata.Value;

        if (server is null)
            return Task.FromResult(AssertionResult.Failed("server was null"));

        var actual = GetCount(server);

        return Task.FromResult(
            AssertionResult.FailIf(actual != _expected, $"received {actual} matching requests")
        );
    }

    private int GetCount(RowdyServer? server) =>
        server?.Requests.Count(request => _matcher(request)) ?? 0;

    /// <summary>
    /// Gets the expectation message.
    /// </summary>
    protected override string GetExpectation() => $"to have received {_expected} matching requests";

    private delegate bool RequestMatcher((HttpRequest Request, HttpResponse Response) request);
}
