using System.ComponentModel;
using System.Security.Claims;
using BlogApp.Core.Security.Abstractions;
using BlogApp.Core.Security.Constants;
using Microsoft.AspNetCore.Http;

namespace BlogApp.Infrastructure.Security.Providers;

public class DomainPrincipal(IHttpContextAccessor contextAccessor) : IDomainPrincipal
{
    private readonly ClaimsPrincipal? _principal =
        contextAccessor.HttpContext?.User ?? Thread.CurrentPrincipal as ClaimsPrincipal;

    public Guid UserId => GetClaim<Guid>(ClaimTypes.NameIdentifier);
    public string FirstName => GetClaim<string>(ClaimTypes.GivenName)!;
    public string LastName => GetClaim<string>(ClaimTypes.Surname)!;
    public string FullName => $"{FirstName} {LastName}";
    public string Email => GetClaim<string>(ClaimTypes.Email)!;
    public bool IsAuthenticated => _principal?.Identity?.IsAuthenticated ?? false;
    public IReadOnlyList<string> Roles => GetClaims(ClaimTypes.Role);
    public IReadOnlyList<string> Permissions => GetClaims(CustomClaimTypes.Permission);

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
}