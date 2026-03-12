using Serilog;

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

    public string Path { get; set; } = "logs/app-.log";
    public string? Formatter { get; set; }
    public RollingInterval RollingInterval { get; set; } = RollingInterval.Day;
    public bool RollOnFileSizeLimit { get; set; } = true;
    public string OutputTemplate { get; set; } = "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}";
}