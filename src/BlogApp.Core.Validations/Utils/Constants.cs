namespace BlogApp.Core.Validations.Utils;

public static class Constants
{
    public const string ResourceName = "Validations";

    public struct Rules
    {
        public const string NotEmpty = "NotEmpty";
        public const string NotNull = "NotNull";
        public const string MinLength = "MinLength";
        public const string MaxLength = "MaxLength";
        public const string Email = "Email";
    }
}