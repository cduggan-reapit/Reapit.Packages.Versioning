using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Reapit.Packages.Versioning.Configuration;
using Reapit.Packages.Versioning.Middleware;

namespace Reapit.Packages.Versioning;

public static class Startup
{
    /// <summary>Adds date versioning services to the DI container</summary>
    /// <param name="services">The IServiceCollection to add the service to.</param>
    /// <param name="headerKey">The header property to contain the api version.</param>
    /// <param name="allowLatest">Flag indicating whether the legacy "latest" value should be accepted.</param>
    /// <param name="allowRollback">Flag indicating whether later dates should resolve to earlier versioned endpoints.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    /// <remarks>We want to get away from needing allowLatest, but our legacy strategy precludes us doing so.</remarks>
    public static IServiceCollection AddDateVersioning(this IServiceCollection services, string headerKey, bool allowLatest, bool allowRollback)
        => services.AddSingleton(new ApiDateVersioningConfiguration(headerKey, allowLatest, allowRollback));

    /// <summary>Adds date versioning middleware to the request pipeline</summary>
    /// <param name="app">The IApplicationBuilder instance.</param>
    /// <returns>The IApplicationBuilder instance.</returns>
    public static IApplicationBuilder UseDateVersioning(this IApplicationBuilder app)
        => app.UseMiddleware<ApiDateVersioningMiddleware>();
}