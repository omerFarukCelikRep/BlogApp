using System.Reflection;
using BlogApp.Core.Mediator.Abstractions;
using BlogApp.Core.Security.Abstractions;
using BlogApp.Core.Security.Attributes;
using BlogApp.Core.Results;
using BlogApp.Core.Security.Exceptions;
using Microsoft.Extensions.Logging;

namespace BlogApp.Core.Mediator.Behaviors;

public sealed class AuthorizeBehavior<TRequest, TResponse>(
    IDomainPrincipal principal,
    IAuthorizationManager authorizationManager,
    ILogger<AuthorizeBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : Result
{
    private static TResponse CreateUnauthorizedResponse(string message)
    {
        var responseType = typeof(TResponse);
        if (responseType == typeof(Result))
            return (TResponse)Result.Failed(message, 401);

        if (!responseType.IsGenericType || responseType.GetGenericTypeDefinition() != typeof(Result<>))
            throw new ForbiddenAccessException("Unauthorized");

        var valueType = responseType.GetGenericArguments()[0];
        var method = typeof(Result)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .First(x => x is { Name: nameof(Result.Failed), IsGenericMethod: true })
            .MakeGenericMethod(valueType);

        return (TResponse)method.Invoke(null, [message, 400])!;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken = default)
    {
        var attributes = request.GetType().GetCustomAttributes<AuthorizeAttribute>().ToArray();
        if (attributes is not { Length: > 0 })
            return await next(cancellationToken);

        if (!principal.IsAuthenticated)
        {
            logger.LogWarning("Unauthorized request for {RequestType}", typeof(TRequest).Name);
            return CreateUnauthorizedResponse("User not authenticated");
        }

        foreach (var attribute in attributes)
        {
            if (attribute.Role.HasValue)
            {
                var hasRole =
                    await authorizationManager.AuthorizeByRoleAsync(principal.UserId, attribute.Role.Value,
                        cancellationToken);
                if (!hasRole)
                    return CreateUnauthorizedResponse("User is not authorized");
            }

            if (!attribute.Permission.HasValue)
                continue;

            var hasPermission = await authorizationManager.AuthorizeByPermissionAsync(principal.UserId,
                attribute.Permission.Value, cancellationToken);
            if (!hasPermission)
                CreateUnauthorizedResponse("User is not authorized");
        }

        return await next(cancellationToken);
    }
}