using Microsoft.AspNetCore.Mvc;
using Reapit.Packages.Versioning.Attributes;

namespace Reapit.Packages.Versioning.UnitTests;

[ApiController]
[Route("/")]
public class TestController : ControllerBase
{
    [HttpGet("no-version")]
    public IActionResult NoVersion()
    {
        return Ok();
    }
    
    [HttpGet("has-version")]
    [ApiVersionDate("2020-01-31")]
    public IActionResult HasVersion()
    {
        return Ok();
    }
}