using BlogApp.Core.DataAccess.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogApp.Core.EFCore.EntityConfigurations;

public class SoftDeletableEntityConfiguration<TEntity, TId> : BaseEntityConfiguration<TEntity, TId>
    where TEntity : SoftDeletableEntity<TId>
    where TId : struct
{
    public override void Configure(EntityTypeBuilder<TEntity> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.DeletedDate)
               .HasMaxLength(128)
               .IsRequired(false);
        builder.Property(x => x.DeletedDate)
               .IsRequired(false);

        builder.HasQueryFilter(x => x.Status != DataAccess.Enums.Status.Deleted);
    }
}

public class SoftDeletableEntityConfiguration<TEntity> : SoftDeletableEntityConfiguration<TEntity, int>
    where TEntity : SoftDeletableEntity<int>
{
}