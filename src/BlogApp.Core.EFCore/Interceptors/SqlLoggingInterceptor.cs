using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace BlogApp.Core.EFCore.Interceptors;

public class SqlLoggingInterceptor(ILogger<SqlLoggingInterceptor> logger) : DbCommandInterceptor
{
    public override DbCommand CommandCreated(CommandEndEventData eventData, DbCommand result)
    {
        logger.LogInformation("SQL Command Created: {ResultCommandText}", result.CommandText);
        return base.CommandCreated(eventData, result);
    }

    public override ValueTask<DbDataReader> ReaderExecutedAsync(DbCommand command, CommandExecutedEventData eventData,
        DbDataReader result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        logger.LogInformation("Executing SQL Command: {CommandCommandText}", command.CommandText);
        return base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
    }

    public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData,
        DbDataReader result)
    {
        logger.LogInformation("Executing SQL Command: {CommandCommandText}", command.CommandText);
        return base.ReaderExecuted(command, eventData, result);
    }
}