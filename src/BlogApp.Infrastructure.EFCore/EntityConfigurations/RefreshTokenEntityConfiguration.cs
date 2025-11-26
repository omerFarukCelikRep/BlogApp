using BlogApp.Core.EFCore.EntityConfigurations;

namespace BlogApp.Infrastructure.EFCore.EntityConfigurations;

public class RefreshTokenEntityConfiguration : BaseEntityConfiguration<RefreshToken>
{
    private const string TableName = "refresh_tokens";

    public override void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        base.Configure(builder);

        builder.ToTable(TableName);

        builder.Property(x => x.Token)
            .IsRequired();
        builder.Property(x => x.UserId)
            .IsRequired();
        builder.Property(x => x.ExpireDate)
            .IsRequired();
        builder.Property(x => x.IsRevoked)
            .IsRequired();
        builder.Property(x => x.RevokedDate);
        builder.Property(x => x.IsUsed)
            .IsRequired();
        builder.Property(x => x.CreatedIp)
            .IsRequired();

        builder.HasOne(x => x.User)
            .WithMany(x => x.RefreshTokens)
            .HasForeignKey(x => x.UserId);
    }
}