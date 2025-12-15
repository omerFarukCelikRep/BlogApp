namespace BlogApp.Core.Validations;

public class ValidationRule<T, TProperty>(string propertyName, Func<T, TProperty> propertyFunc)
{
    private readonly List<Func<TProperty, bool>> _rules = [];
    private readonly List<string> _errors = [];

    private readonly List<Func<TProperty, CancellationToken, Task<bool>>> _asyncRules = [];
    private readonly List<string> _asyncErrors = [];

    private Func<T, bool>? _condition;

    public Func<T, TProperty> PropertyFunc { get; } = propertyFunc;
    public string PropertyName { get; } = propertyName;

    internal bool CanExecute(T instance) => _condition?.Invoke(instance) ?? true;

    internal async Task<IEnumerable<ValidationError>> ValidateAsync(T instance,
        CancellationToken cancellationToken = default)
    {
        var value = PropertyFunc(instance);
        List<ValidationError> validationErrors = [];

        if (!CanExecute(instance))
            return validationErrors;

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
        _condition = predicate;
        return this;
    }
}