namespace BlogApp.Core.Mediator.Abstractions;

public interface IPipelineBehavior<in TRequest, TResponse>
    where TRequest : notnull
{
    Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken = default);
}

public delegate Task RequestHandlerDelegate();

public delegate Task<TResponse> RequestHandlerDelegate<TResponse>();