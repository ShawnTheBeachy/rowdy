using Microsoft.AspNetCore.Http.Extensions;
using Rowdy.Matching;

namespace Rowdy.TUnit;

sealed partial class ReceivedAssertion<T>
{
    /// <summary>
    /// Asserts that a request path matched this path.
    /// </summary>
    /// <param name="path">The path to compare.</param>
    /// <param name="comparison">The string comparison setting to use.</param>
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

    /// <summary>
    /// Asserts that a request URL matched this URL.
    /// </summary>
    /// <param name="url">The URL to compare.</param>
    /// <param name="comparison">The string comparison setting to use.</param>
    public ReceivedAssertion<T> AtUrl(
        string url,
        StringComparison comparison = StringComparison.OrdinalIgnoreCase
    )
    {
        _matcher += request => string.Equals(request.Request.GetDisplayUrl(), url, comparison);
        return this;
    }

    /// <summary>
    /// Asserts that a request method was equal to <c>CONNECT</c>.
    /// </summary>
    public ReceivedAssertion<T> UsingConnect() => UsingMethod(HttpMethod.Connect);

    /// <summary>
    /// Asserts that a request method was equal to <c>DELETE</c>.
    /// </summary>
    public ReceivedAssertion<T> UsingDelete() => UsingMethod(HttpMethod.Delete);

    /// <summary>
    /// Asserts that a request method was equal to <c>GET</c>.
    /// </summary>
    public ReceivedAssertion<T> UsingGet() => UsingMethod(HttpMethod.Get);

    /// <summary>
    /// Asserts that a request method was equal to <c>HEAD</c>.
    /// </summary>
    public ReceivedAssertion<T> UsingHead() => UsingMethod(HttpMethod.Head);

    /// <summary>
    /// Asserts that a request method was equal to this method.
    /// <param name="method">The method to compare.</param>
    /// </summary>
    public ReceivedAssertion<T> UsingMethod(HttpMethod method) => UsingMethod(method.Method);

    /// <summary>
    /// Asserts that a request method was equal to this method.
    /// <param name="method">The method to compare.</param>
    /// <param name="comparison">The string comparison setting to use.</param>
    /// </summary>
    public ReceivedAssertion<T> UsingMethod(
        string method,
        StringComparison comparison = StringComparison.OrdinalIgnoreCase
    )
    {
        _matcher += request => request.Request.Method.Equals(method, comparison);
        return this;
    }

    /// <summary>
    /// Asserts that a request method was equal to <c>OPTIONS</c>.
    /// </summary>
    public ReceivedAssertion<T> UsingOptions() => UsingMethod(HttpMethod.Options);

    /// <summary>
    /// Asserts that a request method was equal to <c>PATCH</c>.
    /// </summary>
    public ReceivedAssertion<T> UsingPatch() => UsingMethod(HttpMethod.Patch);

    /// <summary>
    /// Asserts that a request method was equal to <c>POST</c>.
    /// </summary>
    public ReceivedAssertion<T> UsingPost() => UsingMethod(HttpMethod.Post);

    /// <summary>
    /// Asserts that a request method was equal to <c>PUT</c>.
    /// </summary>
    public ReceivedAssertion<T> UsingPut() => UsingMethod(HttpMethod.Put);

    /// <summary>
    /// Asserts that a request method was equal to <c>TRACE</c>.
    /// </summary>
    public ReceivedAssertion<T> UsingTrace() => UsingMethod(HttpMethod.Trace);

    /// <summary>
    /// Asserts that a request body was equal to this body.
    /// <param name="body">The body to compare.</param>
    /// <param name="comparison">The string comparison setting to use.</param>
    /// </summary>
    public ReceivedAssertion<T> WithBody(
        string? body,
        StringComparison comparison = StringComparison.Ordinal
    )
    {
        _matcher += request =>
        {
            using var reader = new StreamReader(request.Request.Body);
            return reader.ReadToEnd().Equals(body, comparison);
        };
        return this;
    }

    /// <summary>
    /// Asserts that a request body is matched by this matcher.
    /// <param name="matcher">The string matcher to use..</param>
    /// </summary>
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
