namespace BlogApp.Domain.Constants;

public static class Errors
{
    public const string ResourceName = "Errors";
    public const string MessageNotFound = "Message Not Found!";

    public struct Auth
    {
        public const string LoginFailed = "Auth.LoginFailed";
        public const string AccountLocked = "Auth.AccountLocked";
        public const string InvalidCredentials = "Auth.InvalidCredentials";
        public const string EmailAlreadyExists = "Auth.EmailAlreadyExist";
        public const string RefreshTokenFailed = "Auth.RefreshTokenFailed";
    }

    public struct User
    {
        public const string NotFound = "User.NotFound";
        public const string EmailAlreadyConfirmed = "User.EmailAlreadyConfirmed";
        public const string PhoneNumberNotFound = "User.PhoneNumberNotFound";
    }

    public struct Role
    {
        public const string NotFound = "Role.NotFound";
        public const string AlreadyAssigned = "Role.AlreadyAssigned";
        public const string NotAssigned = "Role.NotAssigned";
    }

    public struct Category
    {
        public const string NotFound = "Category.NotFound";
    }

    public struct Tag
    {
        public const string NotFound = "Tag.NotFound";
        public const string AlreadyExists = "Tag.AlreadyExists";
    }

    public struct Blog
    {
        public const string NotFound = "Blog.NotFound";
        public const string UnauthorizeUser = "Blog.UnauthorizeUser";
        public const string NotPublished = "Blog.NotPublished";
    }

    public struct Comment
    {
        public const string NotFound = "Comment.NotFound";
        public const string NestedReplyNotAllowed = "Comment.NestedReplyNotAllowed";
        public const string NotAuthor = "Comment.NotAuthor";
    }
}