namespace Reapit.Packages.Versioning.Exceptions;

public class VersionException : Exception
{
    public static readonly VersionException MissingVersion = new("No version provided");

    public static readonly VersionException InvalidVersion = new("Invalid version provided");
    
    private VersionException(string message)
        : base(message)
    {
    }
    
}