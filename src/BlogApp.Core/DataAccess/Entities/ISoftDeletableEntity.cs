namespace BlogApp.Core.DataAccess.Entities;

public interface ISoftDeletableEntity : IBaseEntity
{
    string? DeletedBy { get; set; }
    DateTime? DeletedDate { get; set; }
}