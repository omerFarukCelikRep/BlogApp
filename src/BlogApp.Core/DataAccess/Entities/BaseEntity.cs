using BlogApp.Core.DataAccess.Enums;

namespace BlogApp.Core.DataAccess.Entities;

public abstract class BaseEntity<TId> : IBaseEntity
    where TId : struct
{
    public TId Id { get; set; }
    public Status Status { get; set; }
    public string CreatedBy { get; set; } = null!;
    public DateTime CreatedDate { get; set; }
    public string? ModifiedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }
}

public abstract class BaseEntity : BaseEntity<int>
{
}