using BlogApp.Core.EFCore.EntityConfigurations;

namespace BlogApp.Infrastructure.EFCore.EntityConfigurations;

public class BlogCategoryEntityConfiguration : BaseEntityConfiguration<BlogCategory>
{
    private const string TableName = "BlogCategory";

    public override void Configure(EntityTypeBuilder<BlogCategory> builder)
    {
        base.Configure(builder);

        builder.ToTable(TableName);

        builder.HasIndex(x => new { x.BlogId, x.CategoryId })
            .IsUnique();

        builder.HasOne(x => x.Blog)
            .WithMany(x => x.BlogCategories)
            .HasForeignKey(x => x.BlogId);
        builder.HasOne(x => x.Category)
            .WithMany(x => x.BlogCategories)
            .HasForeignKey(x => x.CategoryId);
    }
}