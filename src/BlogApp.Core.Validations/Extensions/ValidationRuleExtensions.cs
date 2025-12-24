using System.Text.RegularExpressions;
using Rules = BlogApp.Core.Validations.Utils.Constants.Rules;
using Fields = BlogApp.Core.Validations.Utils.Constants.Fields;
using Pattern = BlogApp.Core.Validations.Utils.Constants.Pattern;

namespace BlogApp.Core.Validations.Extensions;

public static class ValidationRuleExtensions
{
    //TODO:Resource magic string
    extension<T, TProperty>(ValidationRule<T, TProperty> rule)
    {
        public ValidationRule<T, TProperty> WithMessage(string errorCode,
            IReadOnlyDictionary<string, string>? args = null)
        {
            rule.SetErrorCode(errorCode, args);
            return rule;
        }

        public ValidationRule<T, TProperty> NotEmpty(string? message = null)
        {
            Dictionary<string, string> args = new()
            {
                { Fields.PropertyName, rule.PropertyName }
            };
            return rule.Must(value => value switch
                {
                    string s => !string.IsNullOrWhiteSpace(s),
                    int or double or float or decimal => !value.Equals(0),
                    DateTime d => d != default,
                    IEnumerable<TProperty> collection => collection.Any(),
                    _ => value is not null
                }, message ?? Rules.GetDefaultErrorMessage(Rules.NotEmpty, args), Rules.NotEmpty,
                args);
        }

        public ValidationRule<T, TProperty> NotNull(string? message = null)
        {
            Dictionary<string, string> args = new()
            {
                { Fields.PropertyName, rule.PropertyName }
            };
            return rule.Must(value => value is not null, message ?? Rules.GetDefaultErrorMessage(Rules.NotNull, args),
                Rules.NotNull, args);
        }

        public ValidationRule<T, TProperty> Equal(TProperty expected, string? message = null)
        {
            Dictionary<string, string> args = new()
            {
                { Fields.PropertyName, rule.PropertyName },
                { Fields.Expected, expected?.ToString() ?? string.Empty }
            };
            return rule.Must(value => value?.Equals(expected) is true,
                message ?? Rules.GetDefaultErrorMessage(Rules.Equal, args), Rules.Equal, args);
        }

        public ValidationRule<T, TProperty> NotEqual(TProperty expected, string? message = null)
        {
            Dictionary<string, string> args = new()
            {
                { Fields.PropertyName, rule.PropertyName },
                { Fields.Expected, expected?.ToString() ?? string.Empty }
            };
            return rule.Must(value => value?.Equals(expected) is false,
                message ?? Rules.GetDefaultErrorMessage(Rules.NotEqual, args), Rules.NotEqual, args);
        }

        public ValidationRule<T, TProperty> IsMatchRegex(string pattern,
            RegexOptions regexOptions = RegexOptions.IgnoreCase,
            string? message = null,
            string? errorCode = null,
            IReadOnlyDictionary<string, string>? args = null)
        {
            errorCode ??= Rules.IsMatchRegex;
            args ??= new Dictionary<string, string>()
            {
                { Fields.PropertyName, rule.PropertyName },
            };
            return rule.Must(value => Regex.IsMatch(value?.ToString() ?? string.Empty, pattern, regexOptions),
                message ?? Rules.GetDefaultErrorMessage(Rules.IsMatchRegex, args), errorCode, args);
        }


        public ValidationRule<T, TProperty> GreaterThan(int limit, string? message = null)
        {
            Dictionary<string, string> args = new()
            {
                { Fields.PropertyName, rule.PropertyName },
                { Fields.Limit, limit.ToString() },
            };
            return rule.Must(value => value switch
            {
                int intValue => intValue > limit,
                double doubleValue => doubleValue > limit,
                decimal decimalValue => decimalValue > limit,
                float floatValue => floatValue > limit,
                _ => false
            }, message ?? Rules.GetDefaultErrorMessage(Rules.GreaterThan, args), Rules.GreaterThan, args);
        }

        public ValidationRule<T, TProperty> GreaterThanOrEqual(int limit, string? message = null)
        {
            Dictionary<string, string> args = new()
            {
                { Fields.PropertyName, rule.PropertyName },
                { Fields.Limit, limit.ToString() },
            };
            return rule.Must(value => value switch
                {
                    int intValue => intValue >= limit,
                    double doubleValue => doubleValue >= limit,
                    decimal decimalValue => decimalValue >= limit,
                    float floatValue => floatValue >= limit,
                    _ => false
                }, message ?? Rules.GetDefaultErrorMessage(Rules.GreaterThanOrEqual, args), Rules.GreaterThanOrEqual,
                args);
        }

        public ValidationRule<T, TProperty> LessThan(int limit, string? message = null)
        {
            Dictionary<string, string> args = new()
            {
                { Fields.PropertyName, rule.PropertyName },
                { Fields.Limit, limit.ToString() },
            };
            return rule.Must(value => value switch
            {
                int intValue => intValue < limit,
                double doubleValue => doubleValue < limit,
                decimal decimalValue => decimalValue < limit,
                float floatValue => floatValue < limit,
                _ => false
            }, message ?? Rules.GetDefaultErrorMessage(Rules.LessThan, args), Rules.LessThan, args);
        }

        public ValidationRule<T, TProperty> LessThanOrEqual(int limit, string? message = null)
        {
            Dictionary<string, string> args = new()
            {
                { Fields.PropertyName, rule.PropertyName },
                { Fields.Limit, limit.ToString() },
            };
            return rule.Must(value => value switch
            {
                int intValue => intValue <= limit,
                double doubleValue => doubleValue <= limit,
                decimal decimalValue => decimalValue <= limit,
                float floatValue => floatValue <= limit,
                _ => false
            }, message ?? Rules.GetDefaultErrorMessage(Rules.LessThanOrEqual, args), Rules.LessThanOrEqual, args);
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
            Dictionary<string, string> args = new()
            {
                { Fields.PropertyName, rule.PropertyName },
                { Fields.MinLength, min.ToString() },
            };
            return rule.Must(value => value?.Length > min,
                message ?? Rules.GetDefaultErrorMessage(Rules.MinLength, args), Rules.MinLength, args);
        }

        public ValidationRule<T, string?> MaxLength(int max, string? message = null)
        {
            Dictionary<string, string> args = new()
            {
                { Fields.PropertyName, rule.PropertyName },
                { Fields.MaxLength, max.ToString() },
            };
            return rule.Must(value => value?.Length <= max,
                message ?? Rules.GetDefaultErrorMessage(Rules.MaxLength, args), Rules.MaxLength, args);
        }

        public ValidationRule<T, string?> Email(string? message = null)
        {
            Dictionary<string, string> args = new()
            {
                { Fields.PropertyName, rule.PropertyName }
            };
            return rule.IsMatchRegex(Pattern.Email, RegexOptions.IgnoreCase,
                message ?? Rules.GetDefaultErrorMessage(Rules.Email, args), Rules.Email, args);
        }
    }

    extension<T>(ValidationRule<T, DateTime> rule)
    {
        public ValidationRule<T, DateTime> GreaterThan(DateTime date, string? message = null)
        {
            Dictionary<string, string> args = new()
            {
                { Fields.PropertyName, rule.PropertyName },
                { Fields.Date, date.ToString("g") }
            };
            return rule.Must(value => value > date,
                message ?? Rules.GetDefaultErrorMessage(Rules.DateTimeGreaterThan, args), Rules.DateTimeGreaterThan,
                args);
        }

        public ValidationRule<T, DateTime> GreaterThanOrEqual(DateTime date, string? message = null)
        {
            Dictionary<string, string> args = new()
            {
                { Fields.PropertyName, rule.PropertyName },
                { Fields.Date, date.ToString("g") }
            };
            return rule.Must(value => value >= date,
                message ?? Rules.GetDefaultErrorMessage(Rules.DateTimeGreaterThanOrEqual, args),
                Rules.DateTimeGreaterThanOrEqual,
                args);
        }

        public ValidationRule<T, DateTime> LessThan(DateTime date, string? message = null)
        {
            Dictionary<string, string> args = new()
            {
                { Fields.PropertyName, rule.PropertyName },
                { Fields.Date, date.ToString("g") }
            };
            return rule.Must(value => value < date,
                message ?? Rules.GetDefaultErrorMessage(Rules.DateTimeLessThan, args), Rules.DateTimeLessThan,
                args);
        }

        public ValidationRule<T, DateTime> LessThanOrEqual(DateTime date, string? message = null)
        {
            Dictionary<string, string> args = new()
            {
                { Fields.PropertyName, rule.PropertyName },
                { Fields.Date, date.ToString("g") }
            };
            return rule.Must(value => value <= date,
                message ?? Rules.GetDefaultErrorMessage(Rules.DateTimeLessThanOrEqual, args),
                Rules.DateTimeLessThanOrEqual,
                args);
        }
    }

    extension<T, TProperty>(ValidationRule<T, IEnumerable<TProperty>> rule)
    {
        public ValidationRule<T, IEnumerable<TProperty>> Count(int max, int min = 0, string? message = null)
        {
            Dictionary<string, string> args = new()
            {
                { Fields.PropertyName, rule.PropertyName },
                { Fields.Max, max.ToString() },
                { Fields.Min, min.ToString() }
            };
            return rule.Must(value => value.Count() <= max,
                message ?? Rules.GetDefaultErrorMessage(Rules.Count, args), Rules.Count, args);
        }
    }
}