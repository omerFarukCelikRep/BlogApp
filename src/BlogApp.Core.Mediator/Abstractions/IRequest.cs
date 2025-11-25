namespace BlogApp.Core.Mediator.Abstractions;

public interface IRequest
{
}

public interface IRequest<out TResponse>
{
}