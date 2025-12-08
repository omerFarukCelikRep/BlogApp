using System.Text.RegularExpressions;

namespace BlogApp.Core.Validations;

public static class ValidationRuleExtensions
{
    extension<T, TProperty>(ValidationRule<T, TProperty> rule)
    {
        public ValidationRule<T, TProperty> NotEmpty(string? message = null)
        {
            return rule.Must(value => value switch
                {
                    string s => !string.IsNullOrWhiteSpace(s),
                    int or double => !value.Equals(0),
                    DateTime d => d != default,
                    _ => value is not null
                }, message ?? $"{rule.PropertyName}  must not be empty.");
        }

        public ValidationRule<T, TProperty> NotNull(string? message = null)
        {
            return rule.Must(value => value is not null, message ?? $"{rule.PropertyName}  must not be null.");
        }

        public ValidationRule<T, TProperty> MinLength(int min, string? message = null)
        {
            return rule.Must(value => value is string s && s.Length > min,
                message ?? $"{rule.PropertyName} length must be at least {min}");
        }

        public ValidationRule<T, TProperty> MaxLength(int max, string? message = null)
        {
            return rule.Must(value => value is string s && s.Length <= max,
                message ?? $"{rule.PropertyName} length cannot exceed {max}");
        }

        public ValidationRule<T, TProperty> IsMatchRegex(string pattern,
            RegexOptions regexOptions = RegexOptions.IgnoreCase,
            string? message = null)
        {
            return rule.Must(value => Regex.IsMatch(value!.ToString()!, pattern, regexOptions),
                message ?? $"{rule.PropertyName} is not a valid regular expression");
        }

        public ValidationRule<T, TProperty> Email(string? message = null)
        {
            return IsMatchRegex(rule, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase,
                message ?? $"{rule.PropertyName} is not a valid email format");
        }

        public ValidationRule<T, TProperty> GreaterThan(int limit, string? message = null)
        {
            return rule.Must(value => value switch
                {
                    int intValue => intValue > limit,
                    double doubleValue => doubleValue > limit,
                    decimal decimalValue => decimalValue > limit,
                    float floatValue => floatValue > limit,
                    _ => false
                }, message ?? $"{rule.PropertyName} must be greater than {limit}");
        }

        public ValidationRule<T, TProperty> GreaterThanOrEqual(int limit, string? message = null)
        {
            return rule.Must(value => value switch
                {
                    int intValue => intValue >= limit,
                    double doubleValue => doubleValue >= limit,
                    decimal decimalValue => decimalValue >= limit,
                    float floatValue => floatValue >= limit,
                    _ => false
                }, message ?? $"{rule.PropertyName} must be greater than or equal {limit}");
        }

        public ValidationRule<T, TProperty> LessThan(int limit, string? message = null)
        {
            return rule.Must(value => value switch
                {
                    int intValue => intValue < limit,
                    double doubleValue => doubleValue < limit,
                    decimal decimalValue => decimalValue < limit,
                    float floatValue => floatValue < limit,
                    _ => false
                }, message ?? $"{rule.PropertyName} must be less than {limit}");
        }

        public ValidationRule<T, TProperty> LessThanOrEqual(int limit, string? message = null)
        {
            return rule.Must(value => value switch
                {
                    int intValue => intValue <= limit,
                    double doubleValue => doubleValue <= limit,
                    decimal decimalValue => decimalValue <= limit,
                    float floatValue => floatValue <= limit,
                    _ => false
                }, message ?? $"{rule.PropertyName} must be less than or equal {limit}");
        }
    }
}