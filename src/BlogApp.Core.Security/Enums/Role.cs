namespace BlogApp.Core.Security.Enums;

[Flags]
public enum Role
{
    Guest = 1,
    Reader = 2,
    Author = 4,
    Moderator = 8,
    Admin = 16
}