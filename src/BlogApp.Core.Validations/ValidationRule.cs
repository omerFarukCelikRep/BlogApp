using BlogApp.Core.Validations.Abstractions;
using BlogApp.Core.Validations.Results;

namespace BlogApp.Core.Validations;

public class ValidationRule<T, TProperty>(string propertyName, Func<T, TProperty> propertyFunc) : IValidationRule<T>
{
    private record SyncRule(
        Func<TProperty, bool> Predicate,
        string Message,
        string? ErrorCode,
        IReadOnlyDictionary<string, string>? Args);

    private record AsyncRule(
        Func<TProperty, CancellationToken, Task<bool>> Predicate,
        string Message,
        string? ErrorCode,
        IReadOnlyDictionary<string, string>? Args);

    private readonly List<SyncRule> _rules = [];
    private readonly List<AsyncRule> _asyncRules = [];

    private Func<T, bool>? _ruleCondition;
    private Func<T, bool>? _sharedCondition;

    private IValidator<TProperty>? _childValidator;

    private Func<T, TProperty> PropertyFunc { get; } = propertyFunc;
    public string PropertyName { get; } = propertyName;

    private bool CanExecute(T instance)
    {
        if (_sharedCondition != null && !_sharedCondition(instance))
            return false;

        return _ruleCondition == null || _ruleCondition(instance);
    }

    internal void AddSharedCondition(Func<T, bool> condition) => _sharedCondition = condition;

    internal void SetErrorCode(string errorCode, IReadOnlyDictionary<string, string>? args)
    {
        if (_rules is { Count: > 0 })
            _rules[^1] = _rules[^1] with { ErrorCode = errorCode, Args = args };
        else if (_asyncRules is { Count: > 0 })
            _asyncRules[^1] = _asyncRules[^1] with { ErrorCode = errorCode, Args = args };
        else
            throw new InvalidOperationException($"WithMessage() called on '{PropertyName}' before any rule was added.");
    }

    public async Task<IEnumerable<ValidationError>> ValidateAsync(T instance,
        CancellationToken cancellationToken = default)
    {
        List<ValidationError> validationErrors = [];

        if (!CanExecute(instance))
            return validationErrors;

        var value = PropertyFunc(instance);
        if (_childValidator is not null && value is not null)
        {
            var childResult = await _childValidator.ValidateAsync(value, cancellationToken);
            validationErrors.AddRange(childResult.Errors.Select(error =>
                error with { PropertyName = $"{PropertyName}.{error.PropertyName}" }));
        }

        foreach (var syncRule in _rules)
        {
            if (!syncRule.Predicate(value))
                validationErrors.Add(new(PropertyName, syncRule.Message, syncRule.ErrorCode, syncRule.Args));
        }

        if (validationErrors.Count == 0)
        {
            foreach (var asyncRule in _asyncRules)
            {
                if (!await asyncRule.Predicate(value, cancellationToken))
                    validationErrors.Add(new(PropertyName, asyncRule.Message, asyncRule.ErrorCode, asyncRule.Args));
            }
        }

        return validationErrors;
    }

    public ValidationRule<T, TProperty> Must(Func<TProperty, bool> condition, string message,
        string? errorCode = null, IReadOnlyDictionary<string, string>? args = null)
    {
        _rules.Add(new SyncRule(condition, message, errorCode, args));
        return this;
    }

    public ValidationRule<T, TProperty> MustAsync(Func<TProperty, CancellationToken, Task<bool>> condition,
        string message,
        string? errorCode = null, IReadOnlyDictionary<string, string>? args = null)
    {
        _asyncRules.Add(new AsyncRule(condition, message, errorCode, args));
        return this;
    }

    public ValidationRule<T, TProperty> When(Func<T, bool> predicate)
    {
        _ruleCondition = predicate;
        return this;
    }

    public ValidationRule<T, TProperty> SetValidator(IValidator<TProperty> validator)
    {
        _childValidator = validator;
        return this;
    }
}