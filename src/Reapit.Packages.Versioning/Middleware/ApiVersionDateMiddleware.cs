using Microsoft.AspNetCore.Http;
using Reapit.Packages.Versioning.Attributes;
using Reapit.Packages.Versioning.Configuration;
using Reapit.Packages.Versioning.Exceptions;

namespace Reapit.Packages.Versioning.Middleware;

/// <summary>Middleware for testing the ApiVersionDate attribute</summary>
public class ApiVersionDateMiddleware
{
    private readonly RequestDelegate _next;
    private readonly VersioningConfiguration _configuration;

    /// <summary>Initializes a new instance of the <see cref="ApiVersionDateMiddleware"/> class.</summary>
    /// <param name="next">The request pipeline delegate</param>
    /// <param name="configuration">The versioning configuration</param>
    public ApiVersionDateMiddleware(RequestDelegate next, VersioningConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    /// <summary>Executes the API version middleware</summary>
    /// <param name="context">The request HttpContext</param>
    public async Task InvokeAsync(HttpContext context)
    {
        // Get the ApiVersionDateAttribute value from the matched route
        var endpointVersion = context.GetEndpoint()?
            .Metadata
            .GetMetadata<ApiVersionDateAttribute>()?
            .Version;

        
        if (endpointVersion != null)
        {
            var headerVersion = GetVersionHeaderValue(context);
           TestVersionHeader(headerVersion, endpointVersion);
        }

        await _next(context);
    }

    /// <summary>Retrieves the configured version header from a request context.</summary>
    /// <param name="context">The HttpContext.</param>
    /// <returns>The value stored in the configured header field.</returns>
    /// <exception cref="VersionException">version header is missing.</exception>
    private string GetVersionHeaderValue(HttpContext context)
    {
        // If a version is expected but none provided, throw MissingVersion
        if(!context.Request.Headers.TryGetValue(_configuration.Header, out var headerValues))
            throw VersionException.MissingVersion;

        // We only allow one header, so just take the first
        return headerValues.First();
    }

    /// <summary>
    /// Applies tests to confirm that the provided header version matches the version required by the endpoint.
    /// </summary>
    /// <param name="headerVersion">The version provided in the header.</param>
    /// <param name="endpointVersion">The version required by the endpoint.</param>
    /// <exception cref="VersionException">version header is incorrect or invalid.</exception>
    /// <remarks>
    /// Remember - we're not actually implementing versioning.  Each endpoint must be unique at the moment, so all
    /// we need to test is that the api version matches the endpoint ApiVersionDate value
    /// </remarks>
    private void TestVersionHeader(string headerVersion, string endpointVersion)
    {
        var isLatest = _configuration.AllowLatest && headerVersion.Equals("latest", StringComparison.OrdinalIgnoreCase);
            
        // If it's not "latest" (when that's allowed) or a valid date format, throw InvalidVersion
        if(!(isLatest || DateOnly.TryParseExact(headerVersion, "yyyy-MM-dd", out _)))
            throw VersionException.InvalidVersion;

        // If it's not "latest" (when that's allowed) and doesn't match the endpoint version, throw UnmatchedVersion
        if (!(isLatest || endpointVersion.Equals(headerVersion, StringComparison.OrdinalIgnoreCase)))
            throw VersionException.UnmatchedVersion;
    }
}