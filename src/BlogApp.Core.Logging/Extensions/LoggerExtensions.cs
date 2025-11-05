using BlogApp.Core.Logging.Models;
using Serilog;

namespace BlogApp.Core.Logging.Extensions;

/// <summary>
/// Extension methods for logging
/// </summary>
public static class LoggerExtensions
{
    /// <summary>
    /// Logs a full HTTP request and response in a structured format.
    /// </summary>
    /// <param name="logger">The Serilog logger.</param>
    /// <param name="log">The complete log entry for the transaction.</param>
    public static void LogHttpTransaction(this ILogger logger, HttpLog log)
    {
        logger.Information("HTTP Transaction {@HttpLog}", log);
    }
}