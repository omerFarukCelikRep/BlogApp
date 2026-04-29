using System.Reflection;

namespace BlogApp.Infrastructure.EFCore.Contexts;

public class BlogAppDbContext(DbContextOptions<BlogAppDbContext> options) : DbContext(options)
{
    public virtual DbSet<Blog> Blogs { get; set; }
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Role> Roles { get; set; }
    public virtual DbSet<Permission> Permissions { get; set; }
    public virtual DbSet<Category> Categories { get; set; }
    public virtual DbSet<Tag> Tags { get; set; }
    public virtual DbSet<Comment> Comments { get; set; }
    public virtual DbSet<Like> Likes { get; set; }
    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
    public virtual DbSet<SigningKey> SigningKeys { get; set; }
    public virtual DbSet<UserRole> UserRoles { get; set; }
    public virtual DbSet<RolePermission> RolePermissions { get; set; }
    public virtual DbSet<BlogCategory> BlogCategories { get; set; }
    public virtual DbSet<BlogTag> BlogTags { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime) ||
                    property.ClrType == typeof(DateTime?) ||
                    property.ClrType == typeof(DateTimeOffset)||
                    property.ClrType == typeof(DateTimeOffset?)) 
                    property.SetColumnType("timestamp without time zone");
            }
        }
    }
}