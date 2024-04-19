using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Reapit.Packages.Versioning.Configuration;
using Reapit.Packages.Versioning.Exceptions;

namespace Reapit.Packages.Versioning.UnitTests.Middleware;

public class ApiDateVersioningMiddlewareTests
{
    private readonly ApiDateVersioningConfiguration _configuration = new("test-api-header");
    
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
        client.DefaultRequestHeaders.Add(_configuration.Header, "2020-01-31");
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
        await action.Should().ThrowAsync<ApiDateVersioningException>()
            .WithMessage(ApiDateVersioningException.GetInvalidVersionMessage(_configuration.Header));
    }
    
    [Fact]
    public async Task Invoke_ShouldThrowException_WhenInvalidDateProvided()
    {
        using var host = await CreateHost().StartAsync();
        var client = host.GetTestClient();
        client.DefaultRequestHeaders.Add(_configuration.Header, "26/01/1990");
        var action = () => client.GetAsync("/has-version");
        await action.Should().ThrowAsync<ApiDateVersioningException>()
            .WithMessage(ApiDateVersioningException.GetInvalidVersionMessage(_configuration.Header));
    }
    
    [Fact]
    public async Task Invoke_ShouldThrowException_WhenLatestProvided_AndNotPermitted()
    {
        using var host = await CreateHost().StartAsync();
        var client = host.GetTestClient();
        client.DefaultRequestHeaders.Add(_configuration.Header, "latest");
        var action = () => client.GetAsync("/has-version");
        await action.Should().ThrowAsync<ApiDateVersioningException>()
            .WithMessage(ApiDateVersioningException.GetInvalidVersionMessage(_configuration.Header));
    }
    
    [Fact]
    public async Task Invoke_ShouldThrowException_WhenHeaderExpected_AndWrongOneProvided()
    {
        using var host = await CreateHost().StartAsync();
        var client = host.GetTestClient();
        client.DefaultRequestHeaders.Add(_configuration.Header, "1970-01-01");
        var action = () => client.GetAsync("/has-version");
        await action.Should().ThrowAsync<ApiDateVersioningException>()
            .WithMessage(ApiDateVersioningException.GetUnmatchedVersionMessage("1970-01-01"));
    }
    
    [Fact]
    public async Task Invoke_ShouldThrowException_WhenHeaderExpected_AndRollbackDisabled_AndLaterDateProvided()
    {
        using var host = await CreateHost().StartAsync();
        var client = host.GetTestClient();
        client.DefaultRequestHeaders.Add(_configuration.Header, "2999-01-01");
        var action = () => client.GetAsync("/has-version");
        await action.Should().ThrowAsync<ApiDateVersioningException>()
            .WithMessage(ApiDateVersioningException.GetUnmatchedVersionMessage("2999-01-01"));
    }
    
    [Fact]
    public async Task Invoke_ShouldNotInterrupt_WhenHeaderExpected_AndRollbackEnabled_AndLaterDateProvided()
    {
        _configuration.AllowRollback = true;
        using var host = await CreateHost().StartAsync();
        var client = host.GetTestClient();
        client.DefaultRequestHeaders.Add(_configuration.Header, "2999-01-01");
        var result = await client.GetAsync("/has-version");
        result.Should().HaveStatusCode(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Invoke_ShouldNotInterrupt_WhenHeaderExpected_AndCorrectHeaderProvided()
    {
        using var host = await CreateHost().StartAsync();
        var client = host.GetTestClient();
        client.DefaultRequestHeaders.Add(_configuration.Header, "2020-01-31");
        var result = await client.GetAsync("/has-version");
        result.Should().HaveStatusCode(HttpStatusCode.OK);
    }
    
    /*
     * Latest permitted
     */
    
    [Fact]
    public async Task Invoke_ShouldNotInterrupt_WhenLatestAllowed_AndLatestProvided()
    {
        _configuration.AllowLatest = true;
        using var host = await CreateHost().StartAsync();
        var client = host.GetTestClient();
        client.DefaultRequestHeaders.Add(_configuration.Header, "latest");
        var result = await client.GetAsync("/has-version");
        result.Should().HaveStatusCode(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task Invoke_ShouldNotInterrupt_WhenLatestAllowed_AndDateProvided()
    {
        _configuration.AllowLatest = true;
        using var host = await CreateHost().StartAsync();
        var client = host.GetTestClient();
        client.DefaultRequestHeaders.Add(_configuration.Header, "2020-01-31");
        var result = await client.GetAsync("/has-version");
        result.Should().HaveStatusCode(HttpStatusCode.OK);
    }
    
    // Private methods

    private IHostBuilder CreateHost()
        => new HostBuilder()
            .ConfigureWebHost(webBuilder =>
            {
                webBuilder
                    .UseTestServer()
                    .ConfigureServices(services =>
                    {
                        services.AddDateVersioning(
                                _configuration.Header,
                                _configuration.AllowLatest,
                                _configuration.AllowRollback
                            );
                        
                        services.AddRouting();
                        services.AddControllers();
                    })
                    .Configure(app =>
                    {
                        app.UseRouting();
                        app.UseDateVersioning();
                        app.UseEndpoints(endpoint => endpoint.MapControllers());
                    });
            });
}