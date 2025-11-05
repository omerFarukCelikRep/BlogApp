using BlogApp.Core.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogApp.Core.EFCore.EntityConfigurations;

public class BaseEntityConfiguration<TEntity, TId> : IEntityTypeConfiguration<TEntity>
    where TEntity : BaseEntity<TId>
    where TId : struct
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.CreatedDate)
            .IsDescending();

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();
        builder.Property(x => x.Status)
            .IsRequired();
        builder.Property(x => x.CreatedBy)
            .HasMaxLength(128)
            .IsRequired();
        builder.Property(x => x.CreatedDate)
            .IsRequired();
        builder.Property(x => x.ModifiedBy)
            .HasMaxLength(128)
            .IsRequired(false);
        builder.Property(x => x.ModifiedDate)
            .IsRequired(false);
    }
}

public class BaseEntityConfiguration<TEntity> : BaseEntityConfiguration<TEntity, int>
    where TEntity : BaseEntity<int>
{
}