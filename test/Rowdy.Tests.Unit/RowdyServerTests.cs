using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Rowdy.Tests.Unit;

public sealed class RowdyServerTests
{
    [Test]
    public async Task Request_ShouldReturnResponse_WhenEndpointIsMapped()
    {
        // Arrange.
        var sut = new RowdyServer();
        sut.MapGet("/", TypedResults.Ok);

        // Act.
        var client = sut.CreateClient();
        var response = await client.GetAsync("/");

        // Assert.
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
    }

    [Test]
    public async Task Request_ShouldReturnResponse_WhenUsingManuallyCreatedClient()
    {
        // Arrange.
        var sut = new RowdyServer();
        sut.MapGet("/", TypedResults.Ok);
        var client = new HttpClient { BaseAddress = new Uri(sut.Url) };

        // Act.
        var response = await client.GetAsync("/");

        // Assert.
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
    }
}
