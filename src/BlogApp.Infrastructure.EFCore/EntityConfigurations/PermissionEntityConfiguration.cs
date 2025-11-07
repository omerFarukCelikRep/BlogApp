using BlogApp.Core.EFCore.EntityConfigurations;

namespace BlogApp.Infrastructure.EFCore.EntityConfigurations;

public class PermissionEntityConfiguration : BaseEntityConfiguration<Permission>
{
    private const string TableName = "Permissions";

    public override void Configure(EntityTypeBuilder<Permission> builder)
    {
        base.Configure(builder);

        builder.ToTable(TableName);

        builder.HasIndex(x => new { x.Action, x.Type })
            .IsUnique();

        builder.Property(x => x.Action)
            .IsRequired();
        builder.Property(x => x.Type)
            .IsRequired();
    }
}