using BlogApp.Core.EFCore.EntityConfigurations;
using BlogApp.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace BlogApp.Infrastructure.EntityConfigurations;

public class BlogEntityConfiguration : SoftDeletableEntityConfiguration<Blog>
{
    private const string TableName = "Blogs";

    public override void Configure(EntityTypeBuilder<Blog> builder)
    {
        base.Configure(builder);

        builder.ToTable(TableName);
        

        builder.Property(x => x.Title)
            .HasMaxLength(512)
            .IsRequired();
        builder.Property(x => x.Content)
            .IsRequired();
        builder.Property(x => x.Thumbnail)
            .IsRequired(false);
        builder.Property(x => x.ReadingTimeInMinutes)
            .IsRequired();
        builder.Property(x => x.PostStatus)
            .HasConversion<string>();

        builder.HasOne(x => x.Author)
            .WithMany(x => x.Blogs)
            .HasForeignKey(x => x.AuthorId);
        
        builder.HasQueryFilter(x => x.PostStatus != PostStatus.Deleted);
    }
}