using BlogApp.Core.EFCore.EntityConfigurations;

namespace BlogApp.Infrastructure.EFCore.EntityConfigurations;

public class CommentEntityConfiguration : BaseEntityConfiguration<Comment>
{
    private const string TableName = "Comments";

    public override void Configure(EntityTypeBuilder<Comment> builder)
    {
        base.Configure(builder);

        builder.ToTable(TableName);
        builder.Property(x => x.Content)
            .IsRequired();
        builder.Property(x => x.BlogId)
            .IsRequired();
        builder.Property(x => x.UserId)
            .IsRequired();

        builder.HasOne(x => x.Blog)
            .WithMany(x => x.Comments)
            .HasForeignKey(x => x.BlogId);
        builder.HasOne(x => x.User)
            .WithMany(x => x.Comments)
            .HasForeignKey(x => x.UserId);
    }
}