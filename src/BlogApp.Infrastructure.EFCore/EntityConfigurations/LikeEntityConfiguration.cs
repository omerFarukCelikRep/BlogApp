using BlogApp.Core.EFCore.EntityConfigurations;

namespace BlogApp.Infrastructure.EFCore.EntityConfigurations;

public class LikeEntityConfiguration : BaseEntityConfiguration<Like>
{
    private const string TableName = "Likes";

    public override void Configure(EntityTypeBuilder<Like> builder)
    {
        base.Configure(builder);

        builder.ToTable(TableName);

        builder.HasIndex(x => new { x.BlogId, x.UserId })
            .IsUnique();

        builder.HasOne(x => x.Blog)
            .WithMany(x => x.Likes)
            .HasForeignKey(x => x.BlogId);
        builder.HasOne(x => x.User)
            .WithMany(x => x.Likes)
            .HasForeignKey(x => x.UserId);
    }
}