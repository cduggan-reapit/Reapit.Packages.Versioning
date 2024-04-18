using Microsoft.AspNetCore.Http;
using Reapit.Packages.Versioning.Attributes;
using Reapit.Packages.Versioning.Configuration;
using Reapit.Packages.Versioning.Exceptions;

namespace Reapit.Packages.Versioning.Middleware;

public class VersioningMiddleware
{
    private readonly RequestDelegate _next;
    private readonly VersioningConfiguration _configuration;

    public VersioningMiddleware(RequestDelegate next, VersioningConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Get the ApiVersionDateAttribute value from the matched route
        var version = context.GetEndpoint()?
            .Metadata
            .GetMetadata<ApiVersionDateAttribute>()?
            .Version;

        // Remember - we're not actually implementing versioning.  Each endpoint must be unique at the moment, so all
        // we need to test is that the api version matches the endpoint ApiVersionDate value
        if (version != null)
        {
            if(!context.Request.Headers.TryGetValue(_configuration.Header, out var header))
                throw VersionException.MissingVersion;

            if (!version.Equals(header, StringComparison.OrdinalIgnoreCase))
                throw VersionException.InvalidVersion;
        }

        await _next(context);
    }
}