using FluentAssertions;
using Reapit.Packages.Versioning.Attributes;

namespace Reapit.Packages.Versioning.UnitTests.Attributes;

public class ApiVersionDateAttributeTests
{
    [Fact]
    public void Ctor_InitializesObject_FromParameters()
    {
        const string version = "test-version";
        var attribute = new ApiVersionDateAttribute(version);
        attribute.Version.Should().Be(version);
    }
}