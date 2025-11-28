namespace BlogApp.Core.Mediator.Abstractions;

public interface IRequestPreProcessor<in TRequest>
    where TRequest : notnull
{
    Task Process(TRequest request, CancellationToken cancellationToken = default);
}