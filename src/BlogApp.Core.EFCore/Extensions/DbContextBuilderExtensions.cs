using BlogApp.Core.EFCore.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BlogApp.Core.EFCore.Extensions;

public static class DbContextBuilderExtensions
{
    public static DbContextOptionsBuilder AddInterceptors(this DbContextOptionsBuilder optionsBuilder,
        IServiceProvider serviceProvider)
    {
        //TODO:appsettings üzerinden belirtilen sınıfllar dahil edilecek
        serviceProvider.GetRequiredService<QueryTimingInterceptor>();
        optionsBuilder.AddInterceptors(serviceProvider.GetRequiredService<QueryTimingInterceptor>(),
            serviceProvider.GetRequiredService<SaveAuditableChangesInterceptor>(),
            serviceProvider.GetRequiredService<SqlLoggingInterceptor>());

        return optionsBuilder;
    }
}