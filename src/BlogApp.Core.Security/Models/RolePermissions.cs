using BlogApp.Core.Security.Enums;

namespace BlogApp.Core.Security.Models;

public static class RolePermissions
{
    private static readonly Dictionary<Role, Permission[]> RolePermissionMap = new()
    {
        [Role.Guest] = [Permission.BlogRead],
        [Role.Reader] = [Permission.BlogRead],
        [Role.Author] = [Permission.BlogRead, Permission.BlogCreate, Permission.BlogEdit],
        [Role.Moderator] = [Permission.BlogRead, Permission.CommentModerate],
        [Role.Admin] = Enum.GetValues<Permission>()
    };

    public static IReadOnlyCollection<Permission> GetPermissions(Role role)
        => RolePermissionMap.TryGetValue(role, out var perms) ? perms : [];
}