namespace BlogApp.Core.Telemetry.Options;

public class TelemetryOptions
{
    public const string Section = "Telemetry";

    public bool Enabled { get; set; } = true;

    public string ServiceName { get; set; } = "BlogApp";
    public string ServiceVersion { get; set; } = "1.0.0";
    public string Environment { get; set; } = "Development";

    public JaegerOptions Jaeger { get; set; } = new();
    public PrometheusOptions Prometheus { get; set; } = new();
}