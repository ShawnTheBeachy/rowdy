using Microsoft.AspNetCore.Http;

namespace Rowdy;

internal sealed class CapturedHttpResponse : HttpResponse
{
    public override Stream Body
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }

    public override long? ContentLength { get; set; }
    public override string? ContentType { get; set; }
    public override IResponseCookies Cookies { get; }
    public override bool HasStarted { get; }
    public override IHeaderDictionary Headers { get; }
    public override HttpContext HttpContext => throw new NotSupportedException();
    public override int StatusCode { get; set; }

    public CapturedHttpResponse(HttpResponse response)
    {
        ContentLength = response.ContentLength;
        ContentType = response.ContentType;
        Cookies = response.Cookies;
        HasStarted = response.HasStarted;
        Headers = new HeaderDictionary(response.Headers.ToDictionary(x => x.Key, x => x.Value));
        StatusCode = response.StatusCode;
    }

    public override void OnCompleted(Func<object, Task> callback, object state) =>
        throw new NotSupportedException();

    public override void OnStarting(Func<object, Task> callback, object state) =>
        throw new NotSupportedException();

    public override void Redirect(string location, bool permanent) =>
        throw new NotSupportedException();
}
