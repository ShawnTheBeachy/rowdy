using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rowdy;

internal sealed class CapturedHttpRequest : HttpRequest
{
    public override Stream Body { get; set; }
    public override long? ContentLength { get; set; }
    public override string? ContentType { get; set; }
    public override IRequestCookieCollection Cookies { get; set; }
    public override IFormCollection Form { get; set; }
    public override bool HasFormContentType { get; }
    public override IHeaderDictionary Headers { get; }
    public override HostString Host { get; set; }
    public override HttpContext HttpContext => throw new NotSupportedException();
    public override bool IsHttps { get; set; }
    public override string Method { get; set; }
    public override PathString Path { get; set; }
    public override PathString PathBase { get; set; }
    public override string Protocol { get; set; }
    public override IQueryCollection Query { get; set; }
    public override QueryString QueryString { get; set; }
    public override string Scheme { get; set; }

    private CapturedHttpRequest(HttpRequest request, Stream body)
    {
        Body = body;
        ContentLength = request.ContentLength;
        ContentType = request.ContentType;
        Cookies = request.Cookies;
        Form = request.HasFormContentType ? request.Form : null!;
        HasFormContentType = request.HasFormContentType;
        Headers = new HeaderDictionary(request.Headers.ToDictionary(x => x.Key, x => x.Value));
        Host = request.Host;
        IsHttps = request.IsHttps;
        Method = request.Method;
        Path = request.Path;
        PathBase = request.PathBase;
        Protocol = request.Protocol;
        Query = request.Query;
        QueryString = request.QueryString;
        RouteValues = new RouteValueDictionary(request.RouteValues);
        Scheme = request.Scheme;
    }

    public static async ValueTask<CapturedHttpRequest> Create(HttpRequest request)
    {
        var body = new MemoryStream((int)request.Body.Length);

        if (request.Body.CanRead)
            await request.Body.CopyToAsync(body);

        body.Seek(0, SeekOrigin.Begin);
        return new CapturedHttpRequest(request, body);
    }

    public override Task<IFormCollection> ReadFormAsync(
        CancellationToken cancellationToken = new()
    ) => Task.FromResult(Form);
}
