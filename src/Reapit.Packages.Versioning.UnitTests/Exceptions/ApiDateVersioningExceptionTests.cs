using FluentAssertions;
using Reapit.Packages.Versioning.Exceptions;

namespace Reapit.Packages.Versioning.UnitTests.Exceptions;

public class ApiDateVersioningExceptionTests
{
    [Fact]
    public void MissingVersion_Initializes_WithDefaultValues()
    {
        const string headerKey = "header-key";
        var expected = ApiDateVersioningException.GetInvalidVersionMessage(headerKey);
        var sut = ApiDateVersioningException.MissingApiVersionDate(headerKey);
        sut.Message.Should().Be(expected);
    }


    [Fact]
    public void InvalidVersion_Initializes_WithDefaultValues()
    {
        const string headerKey = "header-key";
        var expected = ApiDateVersioningException.GetInvalidVersionMessage(headerKey);
        var sut = ApiDateVersioningException.InvalidApiVersionDate(headerKey);
        sut.Message.Should().Be(expected);
    }

    [Fact]
    public void UnmatchedVersion_Initializes_WithDefaultValues()
    {
        const string provided = "provided-version";
        var expected = ApiDateVersioningException.GetUnmatchedVersionMessage(provided);
        var sut = ApiDateVersioningException.UnmatchedApiVersionDate(provided);
        sut.Message.Should().Be(expected);
    }
}