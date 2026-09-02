using System.Security.Cryptography;
using BlogApp.Api.BackgroundServices;
using BlogApp.Core.DataAccess.Enums;
using BlogApp.Domain.Entities;
using BlogApp.Infrastructure.EFCore.Contexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Role = BlogApp.Core.Security.Enums.Role;

namespace BlogApp.IntegrationTests.Fixtures;

public class BlogAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder("postgres:18.4-alpine")
        .WithDatabase("blogapp_test")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    private static async Task SeedSigningKeyAsync(BlogAppDbContext context)
    {
        var activeKey = await context.SigningKeys.FirstOrDefaultAsync(x => x.IsActive, TestContext.Current.CancellationToken);
        if (activeKey is null || activeKey.ExpireDate < DateTime.Now)
        {
            activeKey?.IsActive = false;

            using var rsa = RSA.Create(2048);
            var privateKey = Convert.ToBase64String(rsa.ExportRSAPrivateKey());
            var publicKey = Convert.ToBase64String(rsa.ExportRSAPublicKey());
            var newKeyId = Guid.CreateVersion7().ToString();
            var newKey = new SigningKey
            {
                KeyId = newKeyId,
                PrivateKey = privateKey,
                PublicKey = publicKey,
                IsActive = true,
                ExpireDate = DateTime.UtcNow.AddDays(7)
            };

            await context.AddAsync(newKey, TestContext.Current.CancellationToken);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        }
    }

    private static async Task SeedAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<BlogAppDbContext>();

        await SeedSigningKeyAsync(db);

        if (!db.Roles.Any())
        {
            db.Roles.AddRange(Enum.GetNames<Role>().Select((x, i) => new Domain.Entities.Role()
            {
                Id = i + 1,
                Name = x,
                CreatedBy = "test seed",
                CreatedDate = DateTimeOffset.UtcNow,
                Status = Status.Added
            }));

            await db.SaveChangesAsync(TestContext.Current.CancellationToken);
        }
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<BlogAppDbContext>));
            if (descriptor is not null)
                services.Remove(descriptor);

            services.AddDbContext<BlogAppDbContext>(options => options.UseNpgsql(_dbContainer.GetConnectionString()));
            
            var hostedServiceDescriptor = services.SingleOrDefault(d => d.ImplementationType == typeof(KeyRotationBackgroundService));
            if (hostedServiceDescriptor is not null)
                services.Remove(hostedServiceDescriptor);
        });
    }

    public async ValueTask InitializeAsync()
    {
        await _dbContainer.StartAsync(TestContext.Current.CancellationToken);

        using var scope =   Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BlogAppDbContext>();
        await db.Database.MigrateAsync(TestContext.Current.CancellationToken);

        await SeedAsync(scope.ServiceProvider);
    }

    public override async ValueTask DisposeAsync()
    {
        await _dbContainer.StopAsync(TestContext.Current.CancellationToken);
        await base.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}