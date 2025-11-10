namespace BlogApp.Domain.Entities;

public class Permission : BaseEntity
{
    public required string Action { get; set; }
    public required string Type { get; set; }

    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new HashSet<RolePermission>();

    public override string ToString() => Action + Type;
}