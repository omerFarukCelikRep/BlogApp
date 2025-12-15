namespace BlogApp.Core.Validations;

public class Validator<T> : IValidator<T>
{
    private readonly List<object> _rules = [];
    private readonly Stack<Func<T, bool>> _conditionStack = [];

    protected ValidationRule<T, TProperty?> RuleFor<TProperty>(string name, Func<T, TProperty?> func)
    {
        var rule = new ValidationRule<T, TProperty?>(name, func);
        if (_conditionStack.Count > 0)
            rule.AddSharedCondition(instance => _conditionStack.All(condition => condition(instance)));

        _rules.Add(rule);
        return rule;
    }

    protected void When(Func<T, bool> predicate, Action action)
    {
        _conditionStack.Push(predicate);
        try
        {
            action();
        }
        finally
        {
            _conditionStack.Pop();
        }
    }

    public async Task<ValidationResult> ValidateAsync(T arg, CancellationToken cancellationToken = default)
    {
        var result = new ValidationResult();
        foreach (var rule in _rules)
        {
            var method = rule.GetType().GetMethod("ValidateAsync");
            var errors = await (Task<IEnumerable<ValidationError>>)(method?.Invoke(rule, [arg]) ??
                                                                    Task.FromResult(
                                                                        Enumerable.Empty<ValidationError>()));

            result.Errors.AddRange(errors);
        }

        return result;
    }
}