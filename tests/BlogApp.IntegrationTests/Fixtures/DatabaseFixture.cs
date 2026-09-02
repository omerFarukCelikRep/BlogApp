using BlogApp.Infrastructure.EFCore.Contexts;
using Microsoft.Extensions.DependencyInjection;

namespace BlogApp.IntegrationTests.Fixtures;

public class DatabaseFixture(BlogAppFactory factory) : IAsyncLifetime
{
    private IServiceScope _scope = null!;
    public BlogAppDbContext DbContext { get; private set; } = null!;

    private async Task CleanAsync()
    {
        DbContext.RefreshTokens.RemoveRange(DbContext.RefreshTokens);
        DbContext.UserRoles.RemoveRange(DbContext.UserRoles);
        DbContext.Users.RemoveRange(DbContext.Users);
        await DbContext.SaveChangesAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await CleanAsync();
        _scope.Dispose();
        GC.SuppressFinalize(this); 
    }

    public async ValueTask InitializeAsync()
    {
        _scope = factory.Services.CreateScope();
        DbContext = _scope.ServiceProvider.GetRequiredService<BlogAppDbContext>();
        
        await CleanAsync();
    }
}