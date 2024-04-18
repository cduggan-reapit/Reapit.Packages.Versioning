using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Reapit.Packages.Versioning.Configuration;
using Reapit.Packages.Versioning.Middleware;

namespace Reapit.Packages.Versioning;

public static class Startup
{
    public static IServiceCollection AddVersioning(this IServiceCollection services, string headerKey)
        => services.AddSingleton(new VersioningConfiguration { Header = headerKey });

    public static IApplicationBuilder UseVersioning(this IApplicationBuilder app)
        => app.UseMiddleware<VersioningMiddleware>();
}