using BlogApp.Core.EFCore.EntityConfigurations;

namespace BlogApp.Infrastructure.EntityConfigurations;

public class BlogTagEntityConfiguration : BaseEntityConfiguration<BlogTag>
{
    private const string TableName = "BlogTags";

    public override void Configure(EntityTypeBuilder<BlogTag> builder)
    {
        base.Configure(builder);

        builder.ToTable(TableName);

        builder.HasIndex(x => new { x.BlogId, x.TagId })
            .IsUnique();

        builder.HasOne(x => x.Blog)
            .WithMany(x => x.BlogTags)
            .HasForeignKey(x => x.BlogId);
        builder.HasOne(x => x.Tag)
            .WithMany(x => x.BlogTags)
            .HasForeignKey(x => x.TagId);
    }
}