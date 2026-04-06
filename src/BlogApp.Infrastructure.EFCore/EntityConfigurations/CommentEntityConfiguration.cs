using BlogApp.Core.EFCore.EntityConfigurations;

namespace BlogApp.Infrastructure.EFCore.EntityConfigurations;

public class CommentEntityConfiguration : BaseEntityConfiguration<Comment>
{
    private const string TableName = "comments";

    public override void Configure(EntityTypeBuilder<Comment> builder)
    {
        base.Configure(builder);

        builder.ToTable(TableName);
        builder.Property(x => x.Content)
            .HasMaxLength(2000)
            .IsRequired();
        builder.Property(x => x.BlogId)
            .IsRequired();
        builder.Property(x => x.UserId)
            .IsRequired();

        builder.HasOne(x => x.Parent)
            .WithMany(x => x.Replies)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Blog)
            .WithMany(x => x.Comments)
            .HasForeignKey(x => x.BlogId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.User)
            .WithMany(x => x.Comments)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}