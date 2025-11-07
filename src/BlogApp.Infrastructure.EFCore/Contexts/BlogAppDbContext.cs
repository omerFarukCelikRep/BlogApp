using System.Reflection;

namespace BlogApp.Infrastructure.EFCore.Contexts;

public class BlogAppDbContext(DbContextOptions<BlogAppDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}