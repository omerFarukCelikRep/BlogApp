using System.Reflection;
using BlogApp.Core.Mediator.Abstractions;
using BlogApp.Core.Results;
using BlogApp.Core.Security.Abstractions;
using BlogApp.Core.Security.Attributes;
using BlogApp.Core.Security.Enums;
using BlogApp.Core.Security.Exceptions;
using Microsoft.Extensions.Logging;

namespace BlogApp.Core.Security.Behaviors;

public sealed class AuthorizeBehavior<TRequest, TResponse>(
    IDomainPrincipal principal,
    IAuthorizationManager authorizationManager,
    ILogger<AuthorizeBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : Result
{
    private async Task AuthorizeByRoleAsync(Role role, CancellationToken cancellationToken = default)
    {
        var authorized = await authorizationManager.AuthorizeByRoleAsync(role, cancellationToken);
        if (!authorized)
            throw new ForbiddenAccessException();
    }

    private async Task AuthorizeByPermissionsAsync(IReadOnlyList<string> requiredPermissions, bool requireAll,
        CancellationToken cancellationToken = default)
    {
        var authorized =
            await authorizationManager.AuthorizeByPermissionAsync(requiredPermissions, requireAll,
                cancellationToken);
        if (!authorized)
            throw new ForbiddenAccessException();
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken = default)
    {
        var attributes = request.GetType()
            .GetCustomAttributes<AuthorizeAttribute>(inherit: true)
            .ToList();
        if (attributes is not { Count: > 0 })
            return await next(cancellationToken);

        if (!principal.IsAuthenticated)
        {
            logger.LogWarning("Unauthorized request for {RequestType}", typeof(TRequest).Name);
            throw new UnauthorizedAccessException();
        }

        foreach (var attribute in attributes)
        {
            if (attribute.Role.HasValue)
                await AuthorizeByRoleAsync(attribute.Role.Value, cancellationToken);

            if (attribute.Permissions is { Count: > 0 })
                await AuthorizeByPermissionsAsync(attribute.Permissions, attribute.RequireAll, cancellationToken);
        }

        return await next(cancellationToken);
    }
}