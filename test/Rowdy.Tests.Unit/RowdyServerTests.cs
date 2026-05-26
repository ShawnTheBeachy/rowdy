using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Rowdy.Tests.Unit;

public sealed class RowdyServerTests
{
    [Test]
    public async Task Endpoint_ShouldBeAbleToBeAdded_WhenServerIsStarted()
    {
        // Arrange.
        var sut = new RowdyServer();
        sut.Start();

        // Act.
        sut.MapGet("/", TypedResults.Ok);
        HttpClient client = sut.CreateClient();
        HttpResponseMessage response = await client.GetAsync("/");

        // Assert.
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
    }

    [Test]
    public async Task Request_ShouldReturnResponse_WhenEndpointIsMapped()
    {
        // Arrange.
        var sut = new RowdyServer();
        sut.MapGet("/", TypedResults.Ok);
        sut.Start();

        // Act.
        HttpClient client = sut.CreateClient();
        HttpResponseMessage response = await client.GetAsync("/");

        // Assert.
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
    }

    [Test]
    public async Task Request_ShouldReturnResponse_WhenUsingManuallyCreatedClient()
    {
        // Arrange.
        var sut = new RowdyServer();
        sut.MapGet("/", TypedResults.Ok);
        sut.Start();
        var client = new HttpClient { BaseAddress = new Uri(sut.Url) };

        // Act.
        HttpResponseMessage response = await client.GetAsync("/");

        // Assert.
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
    }
}
