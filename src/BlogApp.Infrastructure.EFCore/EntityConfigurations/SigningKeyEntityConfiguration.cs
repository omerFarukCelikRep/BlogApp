using BlogApp.Core.EFCore.EntityConfigurations;

namespace BlogApp.Infrastructure.EFCore.EntityConfigurations;

public class SigningKeyEntityConfiguration : SoftDeletableEntityConfiguration<SigningKey>
{
    public override void Configure(EntityTypeBuilder<SigningKey> builder)
    {
        base.Configure(builder);

        builder.Property(s => s.KeyId)
            .IsRequired();
        builder.Property(s => s.PrivateKey)
            .IsRequired();
        builder.Property(s => s.PublicKey)
            .IsRequired();
        builder.Property(s => s.IsActive)
            .IsRequired();
        builder.Property(s => s.ExpireDate);
    }
}