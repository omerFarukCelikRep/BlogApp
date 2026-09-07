namespace BlogApp.Core.Telemetry.Options;

public class JaegerOptions
{
    public bool Enabled { get; set; } = true;
    public string Endpoint { get; set; } = "http://localhost:4317";
}