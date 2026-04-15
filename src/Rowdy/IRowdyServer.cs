using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rowdy;

/// <summary>
/// A server which will listen on a TCP port for requests.
/// </summary>
public interface IRowdyServer : IEndpointRouteBuilder
{
    /// <summary>
    /// A log list of received requests and responses.
    /// </summary>
    IReadOnlyList<(HttpRequest Request, HttpResponse Response)> Requests { get; }
}
