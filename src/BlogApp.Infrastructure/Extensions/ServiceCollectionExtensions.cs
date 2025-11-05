using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BlogApp.Core.EFCore.Extensions;
using BlogApp.Core.Security.Abstractions;
using BlogApp.Domain.Abstractions.Repositories;
using BlogApp.Infrastructure.Contexts;
using BlogApp.Infrastructure.Providers;
using BlogApp.Infrastructure.Repositories;
using BlogApp.Infrastructure.Setups;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace BlogApp.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    private static IServiceCollection AddAuthorization(this IServiceCollection services)
    {
        services.ConfigureOptions<JwtOptionsSetup>();
        services.ConfigureOptions<JwtBearerOptionsSetup>();

        services.AddScoped<IDomainPrincipal, DomainPrincipal>();
        services.AddSingleton<IJwtProvider, JwtProvider>();
        services.AddScoped<IRoleManager, RoleManager>();
        services.AddSingleton<IAuthorizationManager, AuthorizationManager>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme);
        return services;
    }

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

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHttpContextAccessor()
            .AddDbContext(configuration)
            .AddRepositories()
            .AddAuthorization();

        return services;
    }
}