using BlogApp.Core.DataAccess.Enums;
using BlogApp.Core.EFCore.EntityConfigurations;

namespace BlogApp.Infrastructure.EFCore.EntityConfigurations;

public class RoleEntityConfiguration : BaseEntityConfiguration<Role>
{
    private const string TableName = "roles";

    public override void Configure(EntityTypeBuilder<Role> builder)
    {
        base.Configure(builder);

        builder.ToTable(TableName);

        builder.Property(x => x.Name)
            .HasMaxLength(256)
            .IsRequired();

        builder.HasData(Enum.GetNames<Core.Security.Enums.Role>().Select((role, i) => new Role()
        {
            Id = i + 1,
            Name = role,
            CreatedBy = nameof(BlogApp),
            CreatedDate = DateTime.Now,
            Status = Status.Added
        }));
    }
}