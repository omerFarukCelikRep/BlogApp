using BlogApp.Core.Security.Enums;

namespace BlogApp.Core.Security.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public sealed class AuthorizeAttribute : Attribute
{
    public Role? Role { get; }
    public Permission? Permission { get; }

    public AuthorizeAttribute(Role role) => Role = role;
    public AuthorizeAttribute(Permission permission) => Permission = permission;
}