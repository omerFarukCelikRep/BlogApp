using Microsoft.Extensions.Logging;

namespace BlogApp.Core.Logging.Options;

public class SerilogMinimumLevelOptions
{
    public string Default { get; set; } = nameof(LogLevel.Information);
}