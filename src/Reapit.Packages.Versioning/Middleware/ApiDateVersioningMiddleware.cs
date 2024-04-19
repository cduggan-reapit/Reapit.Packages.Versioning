using Microsoft.AspNetCore.Http;
using Reapit.Packages.Versioning.Attributes;
using Reapit.Packages.Versioning.Configuration;
using Reapit.Packages.Versioning.Exceptions;

namespace Reapit.Packages.Versioning.Middleware;

/// <summary>Middleware for testing the ApiVersionDate attribute</summary>
public class ApiDateVersioningMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ApiDateVersioningConfiguration _configuration;

    /// <summary>Initializes a new instance of the <see cref="ApiDateVersioningMiddleware"/> class.</summary>
    /// <param name="next">The request pipeline delegate</param>
    /// <param name="configuration">The versioning configuration</param>
    public ApiDateVersioningMiddleware(RequestDelegate next, ApiDateVersioningConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    /// <summary>Executes the API version middleware</summary>
    /// <param name="context">The request HttpContext</param>
    public async Task InvokeAsync(HttpContext context)
    {
        CompareProvidedVersion(context);
        await _next(context);
    }
    
    private void CompareProvidedVersion(HttpContext context)
    {
        // Return early if no version is required
        var required = GetRequiredVersion(context);
        if (required == null)
            return;
        
        // Throw exception if we expect a header and none was provided
        var provided = GetProvidedVersionHeader(context.Request.Headers);
        if(string.IsNullOrEmpty(provided))
            throw ApiDateVersioningException.MissingApiVersionDate(_configuration.Header);

        // Return early if "latest" is provided and allowLatest=true is configure
        if ("latest".Equals(provided) && _configuration.AllowLatest)
            return;
        
        // Throw exception if header format is incorrect
        if (!DateOnly.TryParseExact(provided, "yyyy-MM-dd", out var providedDate))
            throw ApiDateVersioningException.InvalidApiVersionDate(_configuration.Header);

        var isVersionMatch = _configuration.AllowRollback
            ? required.VersionDate <= providedDate
            : required.VersionDate == providedDate;
        
        if(!isVersionMatch)
            throw ApiDateVersioningException.UnmatchedApiVersionDate(provided);
    }

    private static ApiVersionDateAttribute? GetRequiredVersion(HttpContext context)
        => context.GetEndpoint()?
            .Metadata
            .GetMetadata<ApiVersionDateAttribute>();

    private string? GetProvidedVersionHeader(IHeaderDictionary headers)
        => headers.ContainsKey(_configuration.Header)
                ? headers[_configuration.Header]
                : null as string;
    
}