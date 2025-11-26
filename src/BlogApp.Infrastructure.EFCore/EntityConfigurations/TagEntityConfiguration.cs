using BlogApp.Core.EFCore.EntityConfigurations;

namespace BlogApp.Infrastructure.EFCore.EntityConfigurations;

public class TagEntityConfiguration : BaseEntityConfiguration<Tag>
{
    private const string TableName = "tags";

    public override void Configure(EntityTypeBuilder<Tag> builder)
    {
        base.Configure(builder);

        builder.ToTable(TableName);

        builder.Property(x => x.Name)
            .IsRequired();

        builder.HasMany(x => x.BlogTags)
            .WithOne(x => x.Tag)
            .HasForeignKey(x => x.TagId);
    }
}