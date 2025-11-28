using BlogApp.Core.Mediator.Abstractions;

namespace BlogApp.Core.Mediator.Behaviors;

public class RequestHandlerPreProcesserBehavior<TRequest, TResponse>(IEnumerable<IRequestPreProcessor<TRequest>> processors)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken = default)
    {
        foreach (var processor in processors)
        {
            await processor.Process(request, cancellationToken);
        }

        return await next(cancellationToken);
    }
}