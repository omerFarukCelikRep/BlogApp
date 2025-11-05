using BlogApp.Core.DataAccess.Enums;

namespace BlogApp.Core.DataAccess.Entities;

public interface IBaseEntity
{
    Status Status { get; set; }
    string CreatedBy { get; set; }
    DateTime CreatedDate { get; set; }
    string? ModifiedBy { get; set; }
    DateTime? ModifiedDate { get; set; }
}