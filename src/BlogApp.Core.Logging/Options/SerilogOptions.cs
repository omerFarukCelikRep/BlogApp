namespace BlogApp.Core.Logging.Options;

/// <summary>
/// Represents the Serilog-related configuration settings.
/// These values are typically bound from appsettings.json.
/// </summary>
public class SerilogOptions
{
    public const string Section = "Serilog";

    /// <summary>
    /// The minimum log level (e.g., Debug, Information, Warning).
    /// </summary>
    public SerilogMinimumLevelOptions MinimumLevel { get; set; } = null!;

    /// <summary>
    /// Represents the Serilog-related file log configuration settings.
    /// </summary>
    public SerilogFileOptions? File { get; set; }

    /// <summary>
    /// Represents the Serilog-related elastic log configuration settings.
    /// </summary>
    public SerilogElasticOptions? Elastic { get; set; }
}