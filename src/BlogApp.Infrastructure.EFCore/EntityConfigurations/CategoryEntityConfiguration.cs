using BlogApp.Core.EFCore.EntityConfigurations;

namespace BlogApp.Infrastructure.EFCore.EntityConfigurations;

public class CategoryEntityConfiguration : BaseEntityConfiguration<Category>
{
    private const string TableName = "categories";

    public override void Configure(EntityTypeBuilder<Category> builder)
    {
        base.Configure(builder);

        builder.ToTable(TableName);

        builder.Property(x => x.Name)
            .HasMaxLength(256)
            .IsRequired();
        builder.HasIndex(x => x.Name)
            .IsUnique();
        builder.Property(x => x.Description)
            .HasMaxLength(512)
            .IsRequired(false);
        builder.Property(x => x.Thumbnail)
            .IsRequired(false);
        builder.Property(x => x.Slug)
            .HasMaxLength(256)
            .IsRequired();
        
        builder.HasIndex(x => x.Slug)
            .IsUnique();

        builder.HasOne(x => x.Parent)
            .WithMany(x => x.SubCategories)
            .HasForeignKey(x => x.ParentId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
    }
}