namespace Reapit.Packages.Versioning.Exceptions;

public class VersionException : Exception
{
    internal const string MissingVersionMessage = "No version provided";
    public static readonly VersionException MissingVersion = new(MissingVersionMessage);

    internal const string InvalidVersionMessage = "Invalid version provided";
    public static readonly VersionException InvalidVersion = new(InvalidVersionMessage);
    
    private VersionException(string message)
        : base(message)
    {
    }
    
}