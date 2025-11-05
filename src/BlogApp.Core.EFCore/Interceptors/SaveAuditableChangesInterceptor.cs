using System.Xml.Schema;
using BlogApp.Core.DataAccess.Entities;
using BlogApp.Core.DataAccess.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BlogApp.Core.EFCore.Interceptors;

public class SaveAuditableChangesInterceptor : SaveChangesInterceptor
{
    private static void AssignBaseProperties(DbContext context)
    {
        var entries = context.ChangeTracker.Entries<IBaseEntity>();
        var user = "User"; //TODO:Domain principal ile alınacak

        foreach (var entry in entries)
        {
            SetIfAdded(entry, user);

            SetIfModified(entry, user);

            SetIfDeleted(entry, user);
        }
    }

    private static void SetIfDeleted(EntityEntry<IBaseEntity> entry, string user)
    {
        if (entry.State != EntityState.Deleted)
            return;

        if (entry.Entity is not ISoftDeletableEntity deletableEntity)
            return;

        entry.State = EntityState.Modified;
        entry.Property(nameof(ISoftDeletableEntity.DeletedBy)).CurrentValue = user;
        entry.Property(nameof(ISoftDeletableEntity.DeletedDate)).CurrentValue = DateTime.Now;
        entry.Property(x => x.Status).CurrentValue = Status.Deleted;
    }

    private static void SetIfAdded(EntityEntry<IBaseEntity> entry, string user)
    {
        if (entry.State != EntityState.Added)
            return;

        entry.Property(x => x.CreatedBy).CurrentValue = user;
        entry.Property(x => x.CreatedDate).CurrentValue = DateTime.Now;
        entry.Property(x => x.Status).CurrentValue = Status.Added;
    }

    private static void SetIfModified(EntityEntry<IBaseEntity> entry, string user)
    {
        if (entry.State == EntityState.Modified)
            return;

        entry.Property(x => x.ModifiedBy).CurrentValue = user;
        entry.Property(x => x.ModifiedDate).CurrentValue = DateTime.Now;
        entry.Property(x => x.Status).CurrentValue = Status.Modified;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        if (eventData.Context is not null)
            AssignBaseProperties(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        if (eventData.Context is not null)
            AssignBaseProperties(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        if (eventData.Context is not null)
            AssignBaseProperties(eventData.Context);

        return base.SavedChanges(eventData, result);
    }

    public override ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        if (eventData.Context is not null)
            AssignBaseProperties(eventData.Context);

        return base.SavedChangesAsync(eventData, result, cancellationToken);
    }
}