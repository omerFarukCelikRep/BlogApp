using BlogApp.Core.Mediator.Abstractions;

namespace BlogApp.Core.Mediator.Behaviors;

public class RequestHandlerPostProcesserBehavior<TRequest, TResponse>(IEnumerable<IRequestPostProcessor<TRequest>> processors)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken = default)
    {
        var response = await next(cancellationToken);

        foreach (var processor in processors)
        {
            await processor.Process(request, cancellationToken);
        }

        return response;
    }
}