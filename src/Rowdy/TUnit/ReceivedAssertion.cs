using Microsoft.AspNetCore.Http;
using TUnit.Assertions.Core;

namespace Rowdy.TUnit;

public sealed partial class ReceivedAssertion<T> : Assertion<T>
    where T : IRowdyServer
{
    private int _expected;
    private RequestMatcher _matcher = _ => true;

    internal ReceivedAssertion(AssertionContext<T> context, int expected)
        : base(context)
    {
        _expected = expected;
    }

    protected override Task<AssertionResult> CheckAsync(EvaluationMetadata<T> metadata)
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

    private int GetCount(IRowdyServer? server) =>
        server?.Requests.Count(request => _matcher(request)) ?? 0;

    protected override string GetExpectation() => $"to have received {_expected} matching requests";

    private delegate bool RequestMatcher((HttpRequest Request, HttpResponse Response) request);
}
