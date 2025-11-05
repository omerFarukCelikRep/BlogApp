using System.Data.Common;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace BlogApp.Core.EFCore.Interceptors;

public class QueryTimingInterceptor(ILogger<QueryTimingInterceptor> logger)
    : DbCommandInterceptor
{
    private readonly TimeSpan _threshold = TimeSpan.FromMilliseconds(500);

    public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData,
        DbDataReader result)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            return base.ReaderExecuted(command, eventData, result);
        }
        finally
        {
            stopwatch.Stop();
            if (stopwatch.Elapsed > _threshold)
            {
                logger.LogInformation(
                    "Slow Query Detected: {CommandCommandText} took {StopwatchElapsedMilliseconds} ms",
                    command.CommandText, stopwatch.ElapsedMilliseconds);
            }
        }
    }
}