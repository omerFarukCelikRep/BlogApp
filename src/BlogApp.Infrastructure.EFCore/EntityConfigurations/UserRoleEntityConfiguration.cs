using BlogApp.Core.EFCore.EntityConfigurations;

namespace BlogApp.Infrastructure.EFCore.EntityConfigurations;

public class UserRoleEntityConfiguration : BaseEntityConfiguration<UserRole>
{
    private const string TableName = "user_roles";

    public override void Configure(EntityTypeBuilder<UserRole> builder)
    {
        base.Configure(builder);

        builder.ToTable(TableName);

        builder.Ignore(x => x.ModifiedBy);
        builder.Ignore(x => x.ModifiedDate);

        builder.HasIndex(x => new
        {
            x.UserId,
            x.RoleId
        }).IsUnique();

        builder.HasOne(x => x.User)
            .WithMany(x => x.UserRoles)
            .HasForeignKey(x => x.UserId);

        builder.HasOne(x => x.Role)
            .WithMany(x => x.UserRoles)
            .HasForeignKey(x => x.RoleId);
    }
}