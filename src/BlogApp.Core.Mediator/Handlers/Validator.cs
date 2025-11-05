using BlogApp.Core.Mediator.Abstractions;

namespace BlogApp.Core.Mediator.Handlers;

public abstract class Validator<TRequest> : IValidator<TRequest>
{
    private const string EmptyPropertyError = "{Property} cannot be empty.";

    protected static string CreateError(string property)
    {
        return EmptyPropertyError.Replace("{Property}", property);
    }

    public abstract Task<IEnumerable<string>> ValidateAsync(TRequest request,
        CancellationToken cancellationToken = default);
}