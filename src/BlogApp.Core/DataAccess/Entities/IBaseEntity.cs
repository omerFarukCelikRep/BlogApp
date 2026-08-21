using BlogApp.Core.DataAccess.Enums;

namespace BlogApp.Core.DataAccess.Entities;

public interface IBaseEntity
{
    Status Status { get; set; }
    string CreatedBy { get; set; }
    DateTimeOffset CreatedDate { get; set; }
    string? ModifiedBy { get; set; }
    DateTimeOffset? ModifiedDate { get; set; }
}