using Microsoft.AspNetCore.Http.Extensions;
using Rowdy.Matching;

namespace Rowdy.TUnit;

sealed partial class ReceivedAssertion<T>
{
    public ReceivedAssertion<T> AtPath(
        string path,
        StringComparison comparison = StringComparison.OrdinalIgnoreCase
    )
    {
        _matcher += request =>
            string.Equals(
                request.Request.Path.Value?.TrimStart('/'),
                path.TrimStart('/'),
                comparison
            );
        return this;
    }

    public ReceivedAssertion<T> AtUrl(
        string url,
        StringComparison comparison = StringComparison.OrdinalIgnoreCase
    )
    {
        _matcher += request => string.Equals(request.Request.GetDisplayUrl(), url, comparison);
        return this;
    }

    public ReceivedAssertion<T> UsingConnect() => UsingMethod(HttpMethod.Connect);

    public ReceivedAssertion<T> UsingDelete() => UsingMethod(HttpMethod.Delete);

    public ReceivedAssertion<T> UsingGet() => UsingMethod(HttpMethod.Get);

    public ReceivedAssertion<T> UsingHead() => UsingMethod(HttpMethod.Head);

    public ReceivedAssertion<T> UsingMethod(HttpMethod method) => UsingMethod(method.Method);

    public ReceivedAssertion<T> UsingMethod(string method)
    {
        _matcher += request => request.Request.Method == method;
        return this;
    }

    public ReceivedAssertion<T> UsingOptions() => UsingMethod(HttpMethod.Options);

    public ReceivedAssertion<T> UsingPatch() => UsingMethod(HttpMethod.Patch);

    public ReceivedAssertion<T> UsingPost() => UsingMethod(HttpMethod.Post);

    public ReceivedAssertion<T> UsingPut() => UsingMethod(HttpMethod.Put);

    public ReceivedAssertion<T> UsingTrace() => UsingMethod(HttpMethod.Trace);

    public ReceivedAssertion<T> WithBody(string? body)
    {
        _matcher += request =>
        {
            using var reader = new StreamReader(request.Request.Body);
            return reader.ReadToEnd() == body;
        };
        return this;
    }

    public ReceivedAssertion<T> WithBody(IStringMatcher matcher)
    {
        _matcher += request =>
        {
            using var reader = new StreamReader(request.Request.Body);
            return matcher.IsMatch(reader.ReadToEnd());
        };
        return this;
    }
}
