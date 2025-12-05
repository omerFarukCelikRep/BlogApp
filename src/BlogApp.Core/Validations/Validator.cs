namespace BlogApp.Core.Validations;

public class Validator<T> : IValidator<T>
{
    private readonly List<object> _rules = [];

    protected ValidationRule<T, TProperty> RuleFor<TProperty>(string name, Func<T, TProperty> func)
    {
        var rule = new ValidationRule<T, TProperty>(name, func);
        _rules.Add(rule);
        return rule;
    }

    public Task<ValidationResult> ValidateAsync(T arg, CancellationToken cancellationToken = default)
    {
        var result = new ValidationResult();
        foreach (var rule in _rules)
        {
            var method = rule.GetType().GetMethod("Validate");
            var errors = (IEnumerable<ValidationError>?)method?.Invoke(rule, [arg]) ?? [];

            result.Errors.AddRange(errors);
        }

        return Task.FromResult(result);
    }
}