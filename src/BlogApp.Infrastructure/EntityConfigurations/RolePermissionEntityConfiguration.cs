using BlogApp.Core.EFCore.EntityConfigurations;

namespace BlogApp.Infrastructure.EntityConfigurations;

public class RolePermissionEntityConfiguration : BaseEntityConfiguration<RolePermission>
{
    private const string TableName = "RolePermissions";

    public override void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        base.Configure(builder);

        builder.ToTable(TableName);

        builder.HasIndex(x => new { x.RoleId, x.PermissionId })
            .IsUnique();

        builder.HasOne(x => x.Role)
            .WithMany(x => x.RolePermissions)
            .HasForeignKey(x => x.RoleId);
        builder.HasOne(x => x.Permission)
            .WithMany(x => x.RolePermissions)
            .HasForeignKey(x => x.PermissionId);
    }
}