namespace BlogApp.Core.Security.Constants;

public static class Permission
{
    public static class Blog
    {
        public const string Read   = "Blog.Read";
        public const string Create = "Blog.Create";
        public const string Edit   = "Blog.Edit";
        public const string Delete = "Blog.Delete";
    }

    public static class Comment
    {
        public const string Read     = "Comment.Read";
        public const string Create   = "Comment.Create";
        public const string Moderate = "Comment.Moderate";
    }
    
    public static class Tag
    {
        public const string Create = "Tag.Create";
        public const string Delete = "Tag.Delete";
    }

    public static class User
    {
        public const string Manage = "User.Manage";
        public const string View   = "User.View";
    }

    public static class Admin
    {
        public const string PanelAccess = "Admin.PanelAccess";
    }
}