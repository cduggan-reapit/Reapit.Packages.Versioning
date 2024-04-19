using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Reapit.Packages.Versioning.Exceptions;

namespace Reapit.Packages.Versioning.UnitTests.Middleware;

public class ApiVersionDateMiddlewareTests
{
    private const string TestHeaderKey = "test-api-header";

    /*
     * No Version Expected:
     */
    
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
    
    /*
     * Latest not permitted
     */
    
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
    public async Task Invoke_ShouldThrowException_WhenInvalidDateProvided()
    {
        using var host = await CreateHost().StartAsync();
        var client = host.GetTestClient();
        client.DefaultRequestHeaders.Add(TestHeaderKey, "26/01/1990");
        var action = () => client.GetAsync("/has-version");
        await action.Should().ThrowAsync<VersionException>()
            .WithMessage(VersionException.InvalidVersionMessage);
    }
    
    [Fact]
    public async Task Invoke_ShouldThrowException_WhenLatestProvided_AndNotPermitted()
    {
        using var host = await CreateHost().StartAsync();
        var client = host.GetTestClient();
        client.DefaultRequestHeaders.Add(TestHeaderKey, "latest");
        var action = () => client.GetAsync("/has-version");
        await action.Should().ThrowAsync<VersionException>()
            .WithMessage(VersionException.InvalidVersionMessage);
    }
    
    [Fact]
    public async Task Invoke_ShouldThrowException_WhenHeaderExpected_AndWrongOneProvided()
    {
        using var host = await CreateHost().StartAsync();
        var client = host.GetTestClient();
        client.DefaultRequestHeaders.Add(TestHeaderKey, "1970-01-01");
        var action = () => client.GetAsync("/has-version");
        await action.Should().ThrowAsync<VersionException>()
            .WithMessage(VersionException.UnmatchedVersionMessage);
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
    
    /*
     * Latest permitted
     */
    
    [Fact]
    public async Task Invoke_ShouldNotInterrupt_WhenLatestAllowed_AndLatestProvided()
    {
        using var host = await CreateHost(true).StartAsync();
        var client = host.GetTestClient();
        client.DefaultRequestHeaders.Add(TestHeaderKey, "latest");
        var result = await client.GetAsync("/has-version");
        result.Should().HaveStatusCode(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task Invoke_ShouldNotInterrupt_WhenLatestAllowed_AndDateProvided()
    {
        using var host = await CreateHost(true).StartAsync();
        var client = host.GetTestClient();
        client.DefaultRequestHeaders.Add(TestHeaderKey, "2020-01-31");
        var result = await client.GetAsync("/has-version");
        result.Should().HaveStatusCode(HttpStatusCode.OK);
    }
    
    // Private methods

    private static IHostBuilder CreateHost(bool allowLatest = false)
        => new HostBuilder()
            .ConfigureWebHost(webBuilder =>
            {
                webBuilder
                    .UseTestServer()
                    .ConfigureServices(services =>
                    {
                        services.AddVersioning(TestHeaderKey, allowLatest);
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