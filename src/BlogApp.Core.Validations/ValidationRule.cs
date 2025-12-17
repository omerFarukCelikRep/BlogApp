using BlogApp.Core.Validations.Abstractions;
using BlogApp.Core.Validations.Results;

namespace BlogApp.Core.Validations;

public class ValidationRule<T, TProperty>(string propertyName, Func<T, TProperty> propertyFunc)
{
    private readonly List<Func<TProperty, bool>> _rules = [];
    private readonly List<string> _errors = [];

    private readonly List<Func<TProperty, CancellationToken, Task<bool>>> _asyncRules = [];
    private readonly List<string> _asyncErrors = [];

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

        for (int i = 0; i < _rules.Count; i++)
        {
            if (!_rules[i](value))
                validationErrors.Add(new(PropertyName, _errors[i]));
        }

        for (int i = 0; i < _asyncRules.Count; i++)
        {
            if (!await _asyncRules[i](value, cancellationToken))
                validationErrors.Add(new(PropertyName, _asyncErrors[i]));
        }

        return validationErrors;
    }

    public ValidationRule<T, TProperty> Must(Func<TProperty, bool> condition, string message)
    {
        _rules.Add(condition);
        _errors.Add(message);
        return this;
    }

    public ValidationRule<T, TProperty> MustAsync(Func<TProperty, CancellationToken, Task<bool>> condition,
        string message)
    {
        _asyncRules.Add(condition);
        _asyncErrors.Add(message);
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