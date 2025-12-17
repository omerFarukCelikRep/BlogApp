using System.Text.RegularExpressions;

namespace BlogApp.Core.Validations.Extensions;

public static class ValidationRuleExtensions
{
    //TODO:Resource magic string
    extension<T, TProperty>(ValidationRule<T, TProperty> rule)
    {
        public ValidationRule<T, TProperty> NotEmpty(string? message = null)
        {
            return rule.Must(value => value switch
                {
                    string s => !string.IsNullOrWhiteSpace(s),
                    int or double or float or decimal => !value.Equals(0),
                    DateTime d => d != default,
                    IEnumerable<TProperty> collection => collection.Any(),
                    _ => value is not null
                }, message ?? $"{rule.PropertyName}  must not be empty.");
        }

        public ValidationRule<T, TProperty> NotNull(string? message = null)
        {
            return rule.Must(value => value is not null, message ?? $"{rule.PropertyName}  must not be null.");
        }

        public ValidationRule<T, TProperty> Equal(TProperty expected, string propertyName, string? message = null)
        {
            return rule.Must(value => value?.Equals(expected) is true,
                message ?? $"{rule.PropertyName} must be equal to {expected}");
        }

        public ValidationRule<T, TProperty> NotEqual(TProperty expected, TProperty actual, string? message = null)
        {
            return rule.Must(value => value?.Equals(expected) is false, message ??  $"{rule.PropertyName} must not be equal to {expected}");
        }

        public ValidationRule<T, TProperty> IsMatchRegex(string pattern,
            RegexOptions regexOptions = RegexOptions.IgnoreCase,
            string? message = null)
        {
            return rule.Must(value => Regex.IsMatch(value?.ToString() ?? string.Empty, pattern, regexOptions),
                message ?? $"{rule.PropertyName} is not a valid regular expression");
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

        public ValidationRule<T, TProperty> Between(int min, int max, string? message = null)
        {
            return rule.GreaterThanOrEqual(max, message).LessThanOrEqual(min, message);
        }
    }

    extension<T>(ValidationRule<T, string?> rule)
    {
        public ValidationRule<T, string?> MinLength(int min, string? message = null)
        {
            return rule.Must(value => value?.Length > min,
                message ?? $"{rule.PropertyName} length must be at least {min}");
        }

        public ValidationRule<T, string?> MaxLength(int max, string? message = null)
        {
            return rule.Must(value => value?.Length <= max,
                message ?? $"{rule.PropertyName} length cannot exceed {max}");
        }

        public ValidationRule<T, string?> Email(string? message = null)
        {
            return IsMatchRegex(rule, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase,
                message ?? $"{rule.PropertyName} is not a valid email format");
        }
    }

    extension<T>(ValidationRule<T, DateTime> rule)
    {
        public ValidationRule<T, DateTime> GreaterThan(DateTime date, string? message = null)
        {
            return rule.Must(value => value > date, message ?? $"Date must be before {date}");
        }

        public ValidationRule<T, DateTime> GreaterThanOrEqual(DateTime date, string? message = null)
        {
            return rule.Must(value => value >= date, message ?? $"Date must be before {date}");
        }

        public ValidationRule<T, DateTime> LessThan(DateTime date, string? message = null)
        {
            return rule.Must(value => value < date, message ?? $"Date must be after {date}");
        }

        public ValidationRule<T, DateTime> LessThanOrEqual(DateTime date, string? message = null)
        {
            return rule.Must(value => value <= date, message ?? $"Date must be after {date}");
        }
    }

    extension<T, TProperty>(ValidationRule<T, IEnumerable<TProperty>> rule)
    {
        public ValidationRule<T, IEnumerable<TProperty>> Count(int max, int min = 0, string? message = null)
        {
            return rule.Must(value => value.Count() <= max,
                message ?? $"Collection count must be between {min} and {max}");
        }
    }
}