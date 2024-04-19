namespace Reapit.Packages.Versioning.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ApiVersionDateAttribute : Attribute
{
    public string Version { get; }
    
    public DateOnly VersionDate { get; }

    public ApiVersionDateAttribute(string version)
    {
        Version = version;
        VersionDate = DateOnly.ParseExact(Version, "yyyy-MM-dd");
    }
}