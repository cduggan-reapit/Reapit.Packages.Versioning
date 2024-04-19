namespace Reapit.Packages.Versioning.Configuration;

public class ApiDateVersioningConfiguration
{
    public bool AllowLatest { get; internal set; }
    
    public bool AllowRollback { get; internal set; }
    
    public string Header { get; }

    public ApiDateVersioningConfiguration(string header, bool allowLatest = false, bool allowRollback = false)
    {
        Header = header;
        AllowLatest = allowLatest;
        AllowRollback = allowRollback;
    }
}