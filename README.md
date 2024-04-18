# Reapit.Packages.Versioning
Rather than rely on versioning that doesn't work, this package just checks that the api-version header is set and is in a valid format.

## Usage

### Register the services

- Call the AddVersioning method when building your DI container.  This takes one parameter, the name of the header 
containing API version information. This is used by the middleware, allowing flexibility from project-to-project. 

```csharp
services.AddVersioning("api-version");
```

- This package doesn't actually apply versioning yet - it is intended to replicate the behaviour of our current 
PlatformVersioning package and just make sure that the header has been provided.
- It does require that we move away from `UseMvc()` though, and the middleware should be added through the UseVersioning() 
method

```csharp
app.UseRouting();
app.UseVersioning();
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