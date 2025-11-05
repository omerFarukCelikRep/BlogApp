namespace BlogApp.Core.Logging.Options;

/// <summary>
/// Represents the Serilog-related elastic log configuration settings.
/// These values are typically bound from appsettings.json.
/// </summary>
public class SerilogElasticOptions
{
    /// <summary>
    /// Whether to enable logging.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// The URI of the Elasticsearch sink.
    /// </summary>
    public string[] Urls { get; set; } = [];
}