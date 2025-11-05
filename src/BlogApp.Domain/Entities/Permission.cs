namespace BlogApp.Domain.Entities;

public class Permission : BaseEntity
{
    public string Action { get; set; } = null!;
    public string Type { get; set; } = null!;

    public ICollection<RolePermission> RolePermissions { get; set; } = new HashSet<RolePermission>();

    public override string ToString() => Action + Type;
}