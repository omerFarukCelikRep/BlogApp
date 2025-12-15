namespace BlogApp.Core.Validations;

public class ValidationRule<T, TProperty>(string propertyName, Func<T, TProperty> propertyFunc)
{
    private readonly List<Func<TProperty, bool>> _rules = [];
    private readonly List<string> _errors = [];

    private readonly List<Func<TProperty, CancellationToken, Task<bool>>> _asyncRules = [];
    private readonly List<string> _asyncErrors = [];

    public Func<T, TProperty> PropertyFunc { get; } = propertyFunc;
    public string PropertyName { get; } = propertyName;

    public ValidationRule<T, TProperty> Must(Func<TProperty, bool> condition, string message)
    {
        _rules.Add(condition);
        _errors.Add(message);
        return this;
    }

    public ValidationRule<T, TProperty> MustAsync(Func<TProperty, CancellationToken, Task<bool>> condition, string message)
    {
        _asyncRules.Add(condition);
        _asyncErrors.Add(message);
        return this;
    }

    internal async Task<IEnumerable<ValidationError>> ValidateAsync(T instance,
        CancellationToken cancellationToken = default)
    {
        var value = PropertyFunc(instance);
        List<ValidationError> validationErrors = [];

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
}