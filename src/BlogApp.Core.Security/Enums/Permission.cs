namespace BlogApp.Core.Security.Enums;

[Flags]
public enum Permission
{
    None = 0,
    BlogRead = 1,
    BlogCreate = 2,
    BlogEdit = 4,
    BlogDelete = 8,
    CommentModerate = 16,
    UserManage = 32,
    AdminPanelAccess = 64
}