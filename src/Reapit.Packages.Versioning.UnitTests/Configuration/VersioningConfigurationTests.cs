using FluentAssertions;
using Reapit.Packages.Versioning.Configuration;

namespace Reapit.Packages.Versioning.UnitTests.Configuration;

public class VersioningConfigurationTests
{
    [Fact]
    public void Ctor_InitializesObject_FromParameters()
    {
        const string header = "test-header";
        var attribute = new VersioningConfiguration(header, true);
        attribute.Header.Should().Be(header);
        attribute.AllowLatest.Should().BeTrue();
    }
}