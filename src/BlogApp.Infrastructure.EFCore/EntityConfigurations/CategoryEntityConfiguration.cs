using BlogApp.Core.EFCore.EntityConfigurations;

namespace BlogApp.Infrastructure.EFCore.EntityConfigurations;

public class CategoryEntityConfiguration : BaseEntityConfiguration<Category>
{
    private const string TableName = "Categories";

    public override void Configure(EntityTypeBuilder<Category> builder)
    {
        base.Configure(builder);

        builder.ToTable(TableName);

        builder.Property(x => x.Name)
            .HasMaxLength(256)
            .IsRequired();
        builder.Property(x => x.Description)
            .HasMaxLength(512);
        builder.Property(x => x.Thumbnail)
            .IsRequired();
    }
}