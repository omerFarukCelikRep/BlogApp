using BlogApp.Core.EFCore.Extensions;
using BlogApp.Domain.Abstractions.Repositories;
using BlogApp.Infrastructure.EFCore.Contexts;
using BlogApp.Infrastructure.EFCore.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlogApp.Infrastructure.EFCore.Extensions;

public static class ServiceCollectionExtensions
{
    private static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<BlogAppDbContext>((serviceProvider, options) =>
        {
            options.UseNpgsql(configuration.GetConnectionString("Default"))
                .AddInterceptors(serviceProvider);
            options.UseLazyLoadingProxies();
        });

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IBlogRepository, BlogRepository>()
            .AddScoped<ICategoryRepository, CategoryRepository>()
            .AddScoped<ICommentRepository, CommentRepository>()
            .AddScoped<ILikeRepository, LikeRepository>()
            .AddScoped<IRefreshTokenRepository, RefreshTokenRepository>()
            .AddScoped<IRoleRepository, RoleRepository>()
            .AddScoped<ITagRepository, TagRepository>()
            .AddScoped<IUserRepository, UserRepository>();

        return services;
    }

    public static IServiceCollection AddEFCoreServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext(configuration)
            .AddRepositories();

        return services;
    }
}