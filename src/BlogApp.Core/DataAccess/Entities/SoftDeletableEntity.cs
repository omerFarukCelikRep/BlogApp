namespace BlogApp.Core.DataAccess.Entities;

public class SoftDeletableEntity<TId> : BaseEntity<TId>, ISoftDeletableEntity
    where TId : struct
{
    public string? DeletedBy { get; set; }
    public DateTime? DeletedDate { get; set; }
}

public class SoftDeletableEntity : SoftDeletableEntity<int>
{
}