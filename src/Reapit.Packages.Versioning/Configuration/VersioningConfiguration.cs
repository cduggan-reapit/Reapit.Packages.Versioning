namespace Reapit.Packages.Versioning.Configuration;

public class VersioningConfiguration
{
    public bool AllowLatest { get; }
    
    public string Header { get; }

    public VersioningConfiguration(string header, bool allowLatest = false)
    {
        Header = header;
        AllowLatest = allowLatest;
    }
}