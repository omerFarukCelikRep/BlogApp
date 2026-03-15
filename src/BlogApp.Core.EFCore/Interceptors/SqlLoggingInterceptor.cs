using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace BlogApp.Core.EFCore.Interceptors;

public class SqlLoggingInterceptor(ILogger<SqlLoggingInterceptor> logger) : DbCommandInterceptor
{
    public override ValueTask<DbDataReader> ReaderExecutedAsync(DbCommand command, CommandExecutedEventData eventData,
        DbDataReader result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        logger.LogDebug("Executing SQL Command: {CommandText}", command.CommandText);
        return base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
    }

    public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData,
        DbDataReader result)
    {
        logger.LogDebug("Executing SQL Command: {CommandText}", command.CommandText);
        return base.ReaderExecuted(command, eventData, result);
    }
    
    public override int NonQueryExecuted(DbCommand command, CommandExecutedEventData eventData, int result)
    {
        logger.LogDebug("Executing SQL Command: {CommandText}", command.CommandText);
        return base.NonQueryExecuted(command, eventData, result);
    }

    public override ValueTask<int> NonQueryExecutedAsync(DbCommand command, CommandExecutedEventData eventData,
        int result, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Executing SQL Command: {CommandText}", command.CommandText);
        return base.NonQueryExecutedAsync(command, eventData, result, cancellationToken);
    }
}