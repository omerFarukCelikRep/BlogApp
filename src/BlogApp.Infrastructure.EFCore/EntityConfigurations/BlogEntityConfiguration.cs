using BlogApp.Core.EFCore.EntityConfigurations;
using BlogApp.Domain.Enums;

namespace BlogApp.Infrastructure.EFCore.EntityConfigurations;

public class BlogEntityConfiguration : SoftDeletableEntityConfiguration<Blog>
{
    private const string TableName = "blogs";

    public override void Configure(EntityTypeBuilder<Blog> builder)
    {
        base.Configure(builder);

        builder.ToTable(TableName);

        builder.Property(x => x.Title)
            .HasMaxLength(512)
            .IsRequired();
        builder.Property(x => x.Content)
            .IsRequired();
        builder.Property(x => x.Slug)
            .IsRequired();
        builder.Property(x => x.Thumbnail)
            .IsRequired(false);
        builder.Property(x => x.ReadingTimeInMinutes)
            .IsRequired();
        builder.Property(x => x.PostStatus)
            .HasConversion<string>();
        builder.Property(x => x.ReadCount)
            .HasDefaultValue(0);
        builder.Property(x => x.PublishDate);
            
        builder.HasIndex(x => x.Slug)
            .IsUnique();

        builder.HasOne(x => x.Author)
            .WithMany(x => x.Blogs)
            .HasForeignKey(x => x.AuthorId);
        builder.HasMany(x => x.BlogTags)
            .WithOne(x => x.Blog)
            .IsRequired(false);
        builder.HasMany(x => x.Comments)
            .WithOne(x => x.Blog)
            .IsRequired(false);
        builder.HasMany(x => x.Likes)
            .WithOne(x => x.Blog)
            .IsRequired(false);

        builder.HasQueryFilter(x => x.PostStatus != PostStatus.Deleted);
    }
}