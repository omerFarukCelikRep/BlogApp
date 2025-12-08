namespace BlogApp.Core.Validations;

public class ValidationRule<T, TProperty>(string propertyName, Func<T, TProperty> propertyFunc)
{
    private readonly List<Func<TProperty, bool>> _rules = [];
    private readonly List<string> _errors = [];

    public Func<T, TProperty> PropertyFunc { get; } = propertyFunc;
    public string PropertyName { get; } = propertyName;

    public ValidationRule<T, TProperty> Must(Func<TProperty, bool> condition, string message)
    {
        _rules.Add(condition);
        _errors.Add(message);
        return this;
    }

    internal IEnumerable<ValidationError> Validate(T instance)
    {
        var value = PropertyFunc(instance);
        for (int i = 0; i < _rules.Count; i++)
        {
            if(!_rules[i](value))
                yield return new(PropertyName, _errors[i]);
        }
    }
}