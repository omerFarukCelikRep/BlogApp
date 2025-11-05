namespace BlogApp.Core.Mediator.Abstractions;

public interface IValidator<in TRequest>
{
    Task<IEnumerable<string>> ValidateAsync(TRequest request, CancellationToken cancellationToken = default);
}