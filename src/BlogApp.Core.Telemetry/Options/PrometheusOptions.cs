namespace BlogApp.Core.Telemetry.Options;

public class PrometheusOptions
{
    public bool Enabled { get; set; } = true;
    public string Path { get; set; } = "/metrics";
}