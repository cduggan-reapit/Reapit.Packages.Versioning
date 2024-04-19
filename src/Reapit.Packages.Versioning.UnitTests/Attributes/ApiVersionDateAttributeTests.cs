using FluentAssertions;
using Reapit.Packages.Versioning.Attributes;

namespace Reapit.Packages.Versioning.UnitTests.Attributes;

public class ApiVersionDateAttributeTests
{
    [Fact]
    public void Ctor_InitializesObject_FromParameters()
    {
        const string version = "1970-01-01";
        var attribute = new ApiVersionDateAttribute(version);
        attribute.Version.Should().Be(version);
        attribute.VersionDate.Should().Be(new DateOnly(1970, 1, 1));
    }
    
    [Fact]
    public void Ctor_ThrowsFormatException_WhenVersionNotValid()
    {
        const string version = "test-version";
        var action = () => new ApiVersionDateAttribute(version);
        action.Should().Throw<FormatException>();
    }
}