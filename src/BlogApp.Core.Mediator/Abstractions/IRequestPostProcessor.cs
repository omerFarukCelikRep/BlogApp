namespace BlogApp.Core.Mediator.Abstractions;

public interface IRequestPostProcessor<in TRequest>
    where TRequest : notnull
{
    Task Process(TRequest request, CancellationToken cancellationToken = default);
}