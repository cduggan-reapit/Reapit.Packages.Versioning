namespace Reapit.Packages.Versioning.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ApiVersionDateAttribute : Attribute
{
    public string Version { get; }
    
    public ApiVersionDateAttribute(string version)
    {
        Version = version;
    }
}