using BlogApp.Core.EFCore.Interceptors;
using BlogApp.Domain.Abstractions.Repositories;
using BlogApp.Infrastructure.EFCore.Contexts;
using BlogApp.Infrastructure.EFCore.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BlogApp.Core.EFCore.Extensions;

namespace BlogApp.Infrastructure.EFCore.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        private IServiceCollection AddDbContext(IConfiguration configuration)
        {
            services.AddSingleton<QueryTimingInterceptor>();
            services.AddSingleton<SaveAuditableChangesInterceptor>();
            services.AddSingleton<SqlLoggingInterceptor>();

            services.AddDbContext<BlogAppDbContext>((serviceProvider, options) =>
            {
                options.UseSqlServer(configuration.GetConnectionString("Default"),
                        builder =>
                        {
                            builder.MigrationsAssembly(typeof(BlogAppDbContext).Assembly.FullName);
                            builder.EnableRetryOnFailure();
                        })
                    .AddInterceptors(serviceProvider)
                    .UseLazyLoadingProxies();
                // options.UseNpgsql(configuration.GetConnectionString("Default")) //TODO:PostgreSql paketi güncellenince düzeltilecek
                //     .AddInterceptors(serviceProvider);
            });

            return services;
        }

        private IServiceCollection AddRepositories()
        {
            services.AddScoped<IBlogRepository, BlogRepository>()
                .AddScoped<ICategoryRepository, CategoryRepository>()
                .AddScoped<ICommentRepository, CommentRepository>()
                .AddScoped<ILikeRepository, LikeRepository>()
                .AddScoped<IRefreshTokenRepository, RefreshTokenRepository>()
                .AddScoped<IRoleRepository, RoleRepository>()
                .AddScoped<ITagRepository, TagRepository>()
                .AddScoped<IUserRepository, UserRepository>()
                .AddScoped<ISigningKeyRepository, SigningKeyRepository>();

            return services;
        }

        public IServiceCollection AddEFCoreServices(IConfiguration configuration)
        {
            services.AddDbContext(configuration)
                .AddRepositories();

            return services;
        }
    }
}