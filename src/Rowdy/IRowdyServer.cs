using Microsoft.AspNetCore.Http;

namespace Rowdy;

public interface IRowdyServer
{
    IReadOnlyList<(HttpRequest Request, HttpResponse Response)> Requests { get; }
}
