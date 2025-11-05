namespace BlogApp.Domain.Entities;

public class Role : BaseEntity
{
    public required string Name { get; set; }

    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new HashSet<RolePermission>();
    public virtual ICollection<UserRole> UserRoles { get; set; } = new HashSet<UserRole>();

    public override string ToString() => Name;
}