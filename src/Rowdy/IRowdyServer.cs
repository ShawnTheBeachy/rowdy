using Microsoft.AspNetCore.Http;

namespace Rowdy;

/// <summary>
/// A server which will listen on a TCP port for requests.
/// </summary>
public interface IRowdyServer
{
    /// <summary>
    /// A log list of received requests and responses.
    /// </summary>
    IReadOnlyList<(HttpRequest Request, HttpResponse Response)> Requests { get; }
}
