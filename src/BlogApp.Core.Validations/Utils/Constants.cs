namespace BlogApp.Core.Validations.Utils;

public static class Constants
{
    public const string ResourceName = "Validations";

    public struct Fields
    {
        public const string PropertyName = "PropertyName";
        public const string Expected = "Expected";
        public const string Limit = "Limit";
        public const string MinLength = "MinLength";
        public const string MaxLength = "MaxLength";
        public const string Date = "Date";
        public const string Max = "Max";
        public const string Min = "Min";
    }

    public struct Rules
    {
        public const string NotEmpty = "NotEmpty";
        public const string NotNull = "NotNull";
        public const string Equal = "Equal";
        public const string NotEqual = "NotEqual";
        public const string IsMatchRegex = "IsMatchRegex";
        public const string GreaterThan = "GreaterThan";
        public const string GreaterThanOrEqual = "GreaterThanOrEqual";
        public const string LessThan = "LessThan";
        public const string LessThanOrEqual = "LessThanOrEqual";
        public const string MinLength = "MinLength";
        public const string MaxLength = "MaxLength";
        public const string Email = "Email";
        public const string DateTimeGreaterThan = "DateTimeGreaterThan";
        public const string DateTimeGreaterThanOrEqual = "DateTimeGreaterThanOrEqual";
        public const string DateTimeLessThan = "DateTimeLessThan";
        public const string DateTimeLessThanOrEqual = "DateTimeLessThanOrEqual";
        public const string Count = "Count";

        public static string GetDefaultErrorMessage(string rule, IReadOnlyDictionary<string, string> args)
        {
            var message = rule switch
            {
                NotEmpty => "'{PropertyName}' must not be empty!",
                NotNull => "'{PropertyName}' must not be null!",
                Equal => "'{PropertyName}' must be equal to '{Expected}'!",
                NotEqual => "'{PropertyName}' must not be equal to '{Expected}'!",
                IsMatchRegex => "'{PropertyName}' is not a valid!",
                GreaterThan => "'{PropertyName}' must be greater than '{Limit}'!",
                GreaterThanOrEqual => "'{PropertyName}' must be greater than or equal '{Limit}'!",
                LessThan => "'{PropertyName}' must be less than '{Limit}'!",
                LessThanOrEqual => "'{PropertyName}' must be less than or equal '{Limit}'!",
                Email => "'{PropertyName}' is not a valid email format",
                MinLength => "'{PropertyName}' length must be at least '{MinLength}' character!",
                MaxLength => "'{PropertyName}' length cannot exceed '{MaxLength}' character!",
                DateTimeGreaterThan => "'{PropertyName}' must be after '{Date}'!",
                DateTimeGreaterThanOrEqual => "'{PropertyName}' must be after or same as '{Date}'!",
                DateTimeLessThan => "'{PropertyName}' must be before '{Date}'!",
                DateTimeLessThanOrEqual => "'{PropertyName}' must be before or same as '{Date}'!",
                Count => "Collection count must be between '{Min}' and '{Max}'!",
                _ => "'{PropertyName}' has an invalid error message."
            };

            return args.Aggregate(message,
                (currentMessage, arg) => currentMessage.Replace($"{{{arg.Key}}}", arg.Value));
        }
    }

    public struct Pattern
    {
        public const string Email = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
    }
}