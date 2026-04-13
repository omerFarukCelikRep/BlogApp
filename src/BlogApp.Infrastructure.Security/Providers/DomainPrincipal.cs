using System.ComponentModel;
using System.Security.Claims;
using BlogApp.Core.Security.Abstractions;
using BlogApp.Core.Security.Constants;
using BlogApp.Core.Security.Enums;
using Microsoft.AspNetCore.Http;

namespace BlogApp.Infrastructure.Security.Providers;

public class DomainPrincipal(IHttpContextAccessor contextAccessor) : IDomainPrincipal
{
    private readonly ClaimsPrincipal? _principal =
        contextAccessor.HttpContext?.User ?? Thread.CurrentPrincipal as ClaimsPrincipal;

    public Guid UserId => Guid.TryParse(_principal?.FindFirstValue(ClaimTypes.NameIdentifier), out var id)
        ? id 
        : Guid.Empty;
    public string? Username => GetClaim<string>(ClaimTypes.Name);
    public string FirstName => GetClaim<string>(ClaimTypes.GivenName)!;
    public string LastName => GetClaim<string>(ClaimTypes.Surname)!;
    public string FullName => $"{FirstName} {LastName}";
    public string? Email => GetClaim<string>(ClaimTypes.Email)!;
    public bool IsAuthenticated => _principal?.Identity?.IsAuthenticated ?? false;
    public IReadOnlyList<Role> Roles => GetClaims<Role>(CustomClaimTypes.Roles);
    public IReadOnlyList<string> Permissions => GetClaims(CustomClaimTypes.Permissions);

    private T? GetClaim<T>(string claim)
    {
        var claimValue = _principal?.FindFirst(claim)?.Value;
        if (claimValue is null)
            return (T?)default;

        return (T?)TypeDescriptor.GetConverter(typeof(T)).ConvertTo(claimValue, typeof(T));
    }

    private string[] GetClaims(string claimType)
    {
        var claims = _principal?.FindAll(claimType);
        if (_principal?.Identity?.IsAuthenticated is not true || claims is null)
            return [];

        return claims.Select(c => c.Value).ToArray();
    }

    private T[] GetClaims<T>(string claimType)
    {
        var claims = _principal?.FindAll(claimType);
        if (_principal?.Identity?.IsAuthenticated is not true || claims is null)
            return [];
        
        var converter = TypeDescriptor.GetConverter(typeof(T));
        return claims.Select(x => x.Value)
            .Select(x => (T?)converter.ConvertFrom(x))
            .Where(x => x is not null)
            .Select(x => x!)
            .ToArray();
    }

    public bool IsInRole(Role role) => Roles.Contains(role);

    public bool HasPermission(string permissions) => Permissions.Contains(permissions);

    public bool HasAnyPermission(params List<string> permissions) => permissions.Any(HasPermission);

    public bool HasAllPermissions(params List<string> permissions) => permissions.All(HasPermission);
}