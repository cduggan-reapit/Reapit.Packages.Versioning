using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Reapit.Packages.Versioning.Configuration;

namespace Reapit.Packages.Versioning.UnitTests;

public class StartupTests
{
    [Fact]
    public void AddVersioning_AddsVersioningConfiguration_ToContainer()
    {
        const string header = "test-key";
        var services = new ServiceCollection();
        services.AddVersioning(header);

        using var provider = services.BuildServiceProvider();
        var config = provider.GetRequiredService<VersioningConfiguration>();
        config.Header.Should().Be(header);
    }
}