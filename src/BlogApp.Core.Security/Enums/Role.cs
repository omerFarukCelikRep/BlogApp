namespace BlogApp.Core.Security.Enums;

[Flags]
public enum Role
{
    Guest = 0,
    Reader = 1,
    Author = 2,
    Moderator = 4,
    Admin = 8
}