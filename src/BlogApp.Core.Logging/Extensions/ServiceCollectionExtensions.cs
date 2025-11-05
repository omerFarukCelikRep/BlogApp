using BlogApp.Core.Logging.Options;
using Microsoft.Extensions.DependencyInjection;

namespace BlogApp.Core.Logging.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSerilogOptions(this IServiceCollection services)
    {
        services.AddOptions<SerilogOptions>().BindConfiguration(SerilogOptions.Section);
        return services;
    }
}