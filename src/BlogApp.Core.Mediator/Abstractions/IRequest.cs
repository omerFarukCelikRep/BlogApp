namespace BlogApp.Core.Mediator.Abstractions;

public interface IRequest : IBaseRequest
{
}

public interface IRequest<out TResponse> : IBaseRequest
{
}