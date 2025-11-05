using BlogApp.Core.EFCore.EntityConfigurations;

namespace BlogApp.Infrastructure.EntityConfigurations;

public class RoleEntityConfiguration : BaseEntityConfiguration<Role>
{
    private const string TableName = "Roles";

    public override void Configure(EntityTypeBuilder<Role> builder)
    {
        base.Configure(builder);

        builder.ToTable(TableName);

        builder.Property(x => x.Name)
            .HasMaxLength(256)
            .IsRequired();
    }
}