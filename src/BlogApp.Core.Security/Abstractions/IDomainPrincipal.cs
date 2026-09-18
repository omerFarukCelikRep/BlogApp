using BlogApp.Core.Security.Constants;
using BlogApp.Core.Security.Enums;

namespace BlogApp.Core.Security.Abstractions;

public interface IDomainPrincipal
{
    public Guid UserId { get; }
    public string FullName { get; }
    public string? Username { get; }
    public string? Email { get; }
    bool IsAuthenticated { get; }
    public string? Scope { get; }
    IReadOnlyList<Role> Roles { get; }
    IReadOnlyList<string> Permissions { get; }

    bool IsInRole(Role role);
    bool HasPermission(string permission);
    bool HasAnyPermission(params List<string> permissions);
    bool HasAllPermissions(params List<string> permissions);
}