using BlogApp.Core.EFCore.EntityConfigurations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BlogApp.Infrastructure.EFCore.EntityConfigurations;

public class UserEntityConfiguration : SoftDeletableEntityConfiguration<User, Guid>
{
    private const string TableName = "Users";

    public override void Configure(EntityTypeBuilder<User> builder)
    {
        base.Configure(builder);

        builder.ToTable(TableName);

        builder.HasIndex(x => x.Email)
            .IsUnique();

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .Metadata
            .SetBeforeSaveBehavior(PropertySaveBehavior.Save);

        builder.Property(x => x.FirstName)
            .HasMaxLength(256)
            .IsRequired();
        builder.Property(x => x.LastName)
            .HasMaxLength(256)
            .IsRequired();
        builder.Property(x => x.Username)
            .HasMaxLength(256)
            .IsRequired();
        builder.Property(x => x.Email)
            .HasMaxLength(256)
            .IsRequired();
        builder.Property(x => x.EmailConfirmed)
            .HasDefaultValue(false);
        builder.Property(x => x.Password)
            .HasMaxLength(256)
            .IsRequired();
        builder.Property(x => x.TwoFactorEnabled)
            .HasDefaultValue(false);
        builder.Property(x => x.AccessFailedCount)
            .HasDefaultValue(0);

        builder.HasMany(x => x.Roles);
    }
}