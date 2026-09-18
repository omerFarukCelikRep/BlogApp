using BlogApp.Core.EFCore.EntityConfigurations;

namespace BlogApp.Infrastructure.EFCore.EntityConfigurations;

public class PasswordResetTokenEntityConfiguration : BaseEntityConfiguration<PasswordResetToken>
{
    private const string TableName = "password_reset_tokens";

    public override void Configure(EntityTypeBuilder<PasswordResetToken> builder)
    {
        base.Configure(builder);

        builder.ToTable(TableName);

        builder.Property(x => x.Token)
            .IsRequired();
        builder.Property(x => x.OtpCode)
            .IsRequired();
        builder.Property(x => x.ExpireDate)
            .IsRequired();
        builder.Property(x => x.IsUsed)
            .IsRequired();
        builder.Property(x => x.UserId)
            .IsRequired();

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}