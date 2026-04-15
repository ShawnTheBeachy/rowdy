using System.Diagnostics.Contracts;
using System.Net.NetworkInformation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Rowdy;

/// <summary>
/// A server which will listen on a TCP port for requests.
/// </summary>
public class RowdyServer : IEndpointRouteBuilder, IAsyncDisposable
{
    private readonly WebApplication _app;
    private bool _isDisposed,
        _isStarted;
    private readonly List<(HttpRequest, HttpResponse)> _requests = [];

    ICollection<EndpointDataSource> IEndpointRouteBuilder.DataSources =>
        ((IEndpointRouteBuilder)_app).DataSources;

    private const string HttpClientName = nameof(RowdyServer);

    IServiceProvider IEndpointRouteBuilder.ServiceProvider => _app.Services;

    /// <summary>
    /// The TCP port on which the server is listening.
    /// </summary>
    public int Port { get; }

    /// <summary>
    /// A log list of received requests and responses.
    /// </summary>
    public IReadOnlyList<(HttpRequest Request, HttpResponse Response)> Requests => _requests;

    /// <summary>
    /// The full URL at which the server is listening.
    /// </summary>
    public string Url { get; }

    /// <summary>
    /// Creates a new <c>RowdyServer</c> and starts listening on a TCP port.
    /// </summary>
    /// <param name="port">The TCP port on which to listen.</param>
    /// <param name="useHttps">Whether to use HTTPS. HTTPS redirection will not be enabled even if this is set to <c>true</c>.</param>
    /// <param name="interface">The network interface on which to listen.</param>
    public RowdyServer(int? port = null, bool useHttps = false, string @interface = "localhost")
    {
        var scheme = useHttps ? "https" : "http";
        Port = port ?? GetOpenTcpPort();
        Url = $"{scheme}://{@interface}:{Port}";
        var builder = CreateBuilder();

        if (useHttps)
            builder.WebHost.UseKestrelHttpsConfiguration();

        _app = builder.Build();
        ApplyMiddleware();
    }

    IApplicationBuilder IEndpointRouteBuilder.CreateApplicationBuilder() => null!;

    private void ApplyMiddleware()
    {
        _app.UseCors();
        _app.Use(
            async (context, next) =>
            {
                context.Request.EnableBuffering();
                await next(context);
                _requests.Add(
                    (
                        await CapturedHttpRequest.Create(context.Request),
                        new CapturedHttpResponse(context.Response)
                    )
                );
            }
        );
    }

    [Pure]
    private WebApplicationBuilder CreateBuilder()
    {
        var builder = WebApplication.CreateSlimBuilder(
            new WebApplicationOptions { ApplicationName = "Rowdy Server", EnvironmentName = "Test" }
        );
        builder.WebHost.UseUrls(Url);
        builder.Services.AddCors(opts =>
            opts.AddDefaultPolicy(policy =>
                policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()
            )
        );

        builder.Services.AddHttpClient(
            HttpClientName,
            client =>
            {
                client.BaseAddress = new Uri($"{Url}/");
            }
        );
        return builder;
    }

    /// <summary>
    /// Creates an <c>HttpClient</c> with its <c>BaseAddress</c> set to the URL of the <c>RowdyServer</c>.
    /// </summary>
    /// <returns></returns>
    public HttpClient CreateClient() =>
        _app.Services.GetRequiredService<IHttpClientFactory>().CreateClient(HttpClientName);

    /// <summary>
    /// Stops and disposes the server. Also clears all request logs.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (_isDisposed)
            return;

        _isDisposed = true;
        await _app.StopAsync();
        await _app.DisposeAsync();
        _requests.Clear();
        GC.SuppressFinalize(this);
    }

    [Pure]
    private static int GetOpenTcpPort()
    {
        var ipProps = IPGlobalProperties.GetIPGlobalProperties();
        int port;

        do
        {
            port = Random.Shared.Next(5_000, 9_999);
        } while (ipProps.GetActiveTcpListeners().Any(x => x.Port == port));

        return port;
    }

    /// <summary>
    /// Removes all route mappings and received requests.
    /// </summary>
    public void Clear()
    {
        ClearMappings();
        ClearRequests();
    }

    /// <summary>
    /// Removes all route mappings from the server.
    /// </summary>
    public void ClearMappings() => ((IEndpointRouteBuilder)_app).DataSources.Clear();

    /// <summary>
    /// Removes all received requests.
    /// </summary>
    public void ClearRequests() => _requests.Clear();

    /// <summary>
    /// Starts the server.
    /// </summary>
    public void Start()
    {
        if (_isStarted)
            return;

        _isStarted = true;
        ThreadPool.QueueUserWorkItem(_ => _app.Run());
    }
}
