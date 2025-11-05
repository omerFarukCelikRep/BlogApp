namespace BlogApp.Core.Logging.Options;

/// <summary>
/// Represents the Serilog-related file log configuration settings.
/// These values are typically bound from appsettings.json.
/// </summary>
public class SerilogFileOptions
{
    /// <summary>
    /// Whether to enable logging.
    /// </summary>
    public bool Enabled { get; set; }

    public string Path { get; set; } = null!;
    public string Formatter { get; set; } = null!;
    public string RollingInterval { get; set; } = null!;
    public bool RollOnFileSizeLimit { get; set; }
    public string OutputTemplate { get; set; } = null!;
}