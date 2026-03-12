using Microsoft.Extensions.Logging;

namespace BlogApp.Core.Logging.Options;

public class SerilogMinimumLevelOptions
{
    public LogLevel Default { get; set; } = LogLevel.Information;
    public Dictionary<string, LogLevel> Override { get; set; } = new();
}