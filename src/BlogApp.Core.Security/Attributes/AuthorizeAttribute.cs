using BlogApp.Core.Security.Enums;

namespace BlogApp.Core.Security.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public sealed class AuthorizeAttribute : Attribute
{
    public Role? Role { get; }
    public IReadOnlyList<string>? Permissions { get; }
    public bool RequireAll { get; init; } = false;

    public AuthorizeAttribute(Role role) => Role = role;
    public AuthorizeAttribute(params List<string> permissions) => Permissions = permissions;
}