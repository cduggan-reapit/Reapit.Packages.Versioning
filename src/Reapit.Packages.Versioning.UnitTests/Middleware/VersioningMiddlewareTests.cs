using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Reapit.Packages.Versioning.Exceptions;

namespace Reapit.Packages.Versioning.UnitTests.Middleware;

public class VersioningMiddlewareTests
{
    private const string TestHeaderKey = "test-api-header";

    [Fact]
    public async Task Invoke_ShouldNotInterrupt_WhenEndpointNotMatched()
    {
        using var host = await CreateHost().StartAsync();
        var client = host.GetTestClient();
        var result = await client.GetAsync("/no-endpoint");
        result.Should().HaveStatusCode(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task Invoke_ShouldNotInterrupt_WhenNoHeaderExpected_AndNoneProvided()
    {
        using var host = await CreateHost().StartAsync();
        var client = host.GetTestClient();
        var result = await client.GetAsync("/no-version");
        result.Should().HaveStatusCode(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task Invoke_ShouldNotInterrupt_WhenNoHeaderExpected_AndOneProvided()
    {
        using var host = await CreateHost().StartAsync();
        var client = host.GetTestClient();
        client.DefaultRequestHeaders.Add(TestHeaderKey, "2020-01-31");
        var result = await client.GetAsync("/no-version");
        result.Should().HaveStatusCode(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task Invoke_ShouldThrowException_WhenHeaderExpected_AndNoneProvided()
    {
        using var host = await CreateHost().StartAsync();
        var client = host.GetTestClient();
        var action = () => client.GetAsync("/has-version");
        await action.Should().ThrowAsync<VersionException>()
            .WithMessage(VersionException.MissingVersionMessage);
    }
    
    [Fact]
    public async Task Invoke_ShouldThrowException_WhenHeaderExpected_AndWrongOneProvided()
    {
        using var host = await CreateHost().StartAsync();
        var client = host.GetTestClient();
        client.DefaultRequestHeaders.Add(TestHeaderKey, "1970-01-01");
        var action = () => client.GetAsync("/has-version");
        await action.Should().ThrowAsync<VersionException>()
            .WithMessage(VersionException.InvalidVersionMessage);
    }
    
    [Fact]
    public async Task Invoke_ShouldNotInterrupt_WhenHeaderExpected_AndCorrectHeaderProvided()
    {
        using var host = await CreateHost().StartAsync();
        var client = host.GetTestClient();
        client.DefaultRequestHeaders.Add(TestHeaderKey, "2020-01-31");
        var result = await client.GetAsync("/has-version");
        result.Should().HaveStatusCode(HttpStatusCode.OK);
    }
    
    // Private methods

    private static IHostBuilder CreateHost()
        => new HostBuilder()
            .ConfigureWebHost(webBuilder =>
            {
                webBuilder
                    .UseTestServer()
                    .ConfigureServices(services =>
                    {
                        services.AddVersioning(TestHeaderKey);
                        services.AddRouting();
                        services.AddControllers();
                    })
                    .Configure(app =>
                    {
                        app.UseRouting();
                        app.UseVersioning();
                        app.UseEndpoints(endpoint => endpoint.MapControllers());
                    });
            });
}