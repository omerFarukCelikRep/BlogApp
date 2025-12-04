using BlogApp.Core.Mediator.Abstractions;

namespace BlogApp.Core.Mediator.Handlers;

public abstract class Validator<TRequest> : IValidator<TRequest>
{
    private const string EmptyPropertyError = "{Property} cannot be empty."; //TODO:Resource

    protected List<string> Errors => [];

    protected static string CreateError(string property)
    {
        return EmptyPropertyError.Replace("{Property}", property);
    }

    protected void IfEmpty(string propertyName, string value)
    {
        if (string.IsNullOrEmpty(value))
            Errors.Add(CreateError(propertyName));
    }

    public abstract Task<IEnumerable<string>> ValidateAsync(TRequest request,
        CancellationToken cancellationToken = default);
}