using FluentAssertions;
using Reapit.Packages.Versioning.Exceptions;

namespace Reapit.Packages.Versioning.UnitTests.Exceptions;

public class VersionExceptionTests
{
    [Fact]
    public void MissingVersion_Initializes_WithDefaultValues()
    {
        VersionException.MissingVersion.Message.Should().Be(VersionException.MissingVersionMessage);
    }
    
    [Fact]
    public void InvalidVersion_Initializes_WithDefaultValues()
    {
        VersionException.InvalidVersion.Message.Should().Be(VersionException.InvalidVersionMessage);
    }
}