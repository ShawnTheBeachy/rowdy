using System.Diagnostics.Contracts;
using System.Net.NetworkInformation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Rowdy;

public sealed class RowdyServer : IRowdyServer, IEndpointRouteBuilder, IAsyncDisposable
{
    private readonly WebApplication _app;
    private bool _isDisposed;
    private readonly List<(HttpRequest, HttpResponse)> _requests = [];

    ICollection<EndpointDataSource> IEndpointRouteBuilder.DataSources =>
        ((IEndpointRouteBuilder)_app).DataSources;

    private const string HttpClientName = nameof(RowdyServer);

    IServiceProvider IEndpointRouteBuilder.ServiceProvider => _app.Services;

    public int Port { get; }
    public IReadOnlyList<(HttpRequest Request, HttpResponse Response)> Requests => _requests;
    public string Url { get; }

    public RowdyServer(int? port = null, bool useHttps = false, string @interface = "localhost")
        : this(port ?? GetOpenTcpPort(), useHttps, @interface) { }

    public RowdyServer(int port, bool useHttps, string @interface)
    {
        var scheme = useHttps ? "https" : "http";
        Port = port;
        Url = $"{scheme}://{@interface}:{Port}";
        var builder = CreateBuilder();

        if (useHttps)
            builder.WebHost.UseKestrelHttpsConfiguration();

        _app = builder.Build();
        ApplyMiddleware();
        Start();
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

    public async ValueTask DisposeAsync()
    {
        if (_isDisposed)
            return;

        _isDisposed = true;
        await _app.StopAsync();
        await _app.DisposeAsync();
        _requests.Clear();
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

    private void Start() => ThreadPool.QueueUserWorkItem(_ => _app.Run());
}
