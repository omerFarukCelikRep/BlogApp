namespace BlogApp.Core.Mediator.Abstractions;

public interface IMediator
{
    Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : IRequest;

    Task<TResponse> Send<TRequest, TResponse>(TRequest request,
        CancellationToken cancellationToken = default)
        where TRequest : IRequest<TResponse>;
}