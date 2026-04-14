namespace BlogApp.Domain.Constants;

public static class Errors
{
    public const string ResourceName = "Errors";
    public const string MessageNotFound = "Message Not Found!";

    public struct Auth
    {
        public const string LoginFailed = "LoginFailed";
        public const string AccountLocked = "AccountLocked";
        public const string InvalidCredentials = "InvalidCredentials";
        public const string EmailAlreadyExist = "EmailAlreadyExist";
    }

    public struct User
    {
        public const string NotFound = "UserNotFound";
    }

    public struct Role
    {
        public const string NotFound = "RoleNotFound";
        public const string AlreadyAssigned = "RoleAlreadyAssigned";
        public const string NotAssigned = "RoleNotAssigned";
    }
}