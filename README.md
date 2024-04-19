# Reapit.Packages.Versioning
Rather than rely on versioning that doesn't work, this package just checks that the api-version header is set and is in a valid format.

## Usage

### Register the services

- Call the AddVersioning method when building your DI container.  This takes one parameter, the name of the header 
containing API version information. This is used by the middleware, allowing flexibility from project-to-project. 

```csharp
services.AddDateVersioning(
    // The header property which will contain the requested version date
    headerKey: "api-version",
    // Flag indicating whether "latest" should be accepted
    allowLatest: true|false,
    // Flag indicating a request with a later dated header should be accepted for an earier versioned endpoint
    allowRollback: true|false); 
```

- This package doesn't actually apply versioning yet - it is intended to replicate the behaviour of our current 
PlatformVersioning package and just make sure that the header has been provided.
- It does require that we move away from `UseMvc()` though, and the middleware should be added through the `UseVersioning()` 
method

```csharp
app.UseRouting();
app.UseDateVersioning();
app.UseEndpoints(endpoint => endpoint.MapControllers());
```

### Decorate the actions

Add an `ApiVersionDate` attribute to the endpoints that should be protected:

```csharp
[HttpGet("has-version")]
[ApiVersionDate("2020-01-31")]
public IActionResult HasVersion() => Ok();
```

# Dependencies
- xUnit
- FluentAssertions
- Microsoft.AspNetCore.TestHost