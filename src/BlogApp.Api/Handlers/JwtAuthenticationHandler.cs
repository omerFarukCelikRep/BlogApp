using BlogApp.Domain.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Encodings.Web;
using BlogApp.Domain.Abstractions.Services;

namespace BlogApp.Api.Handlers;

public class JwtAuthenticationHandler : JwtBearerHandler
{
    private const string AuthorizationHeaderName = "Authorization";

    [Obsolete("Obsolete")]
    public JwtAuthenticationHandler(IOptionsMonitor<JwtOptions> options, ILoggerFactory logger, UrlEncoder encoder,
        ISystemClock clock) : base(options, logger, encoder, clock)
    {
    }

    public JwtAuthenticationHandler(IOptionsMonitor<JwtOptions> options, ILoggerFactory logger, UrlEncoder encoder) :
        base(options, logger, encoder)
    {
    }

    private async Task<AuthenticateResult> ValidateAccessToken(string token)
    {
        var handlers = Options.TokenHandlers is { Count: > 0 }
            ? Options.TokenHandlers
            : [new JwtSecurityTokenHandler()];
        foreach (var tokenHandler in handlers)
        {
            var validationResult = await tokenHandler.ValidateTokenAsync(token, Options.TokenValidationParameters);
            if (!validationResult.IsValid || validationResult.ClaimsIdentity == null)
            {
                return AuthenticateResult.Fail(string.Empty);
            }

            var principal = new ClaimsPrincipal(validationResult.ClaimsIdentity);
            return AuthenticateResult.Success(new(principal, Scheme.Name));
        }

        return AuthenticateResult.Fail(string.Empty);
    }

    private async Task<AuthenticateResult> ValidateRefreshToken(string token)
    {
        if (!Request.Headers.TryGetValue("X-Refresh-Token", out var refreshTokenHeader))
            return AuthenticateResult.Fail("Access token is invalid or expired.");
        var refreshToken = refreshTokenHeader.FirstOrDefault();

        if (string.IsNullOrEmpty(refreshToken))
            return AuthenticateResult.Fail("Access token is invalid or expired.");
        var refreshTokenService = Context.RequestServices.GetRequiredService<IRefreshTokenService>();

        if (!await refreshTokenService.IsValidAsync(refreshToken))
            return AuthenticateResult.Fail("Refresh token is invalid.");
        var identity = await refreshTokenService.GetClaimsFromRefreshTokenAsync(refreshToken);
        if (identity == null)
            return AuthenticateResult.Fail("Refresh token is invalid.");

        var principal = new ClaimsPrincipal(identity);
        return AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name));
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(AuthorizationHeaderName, out var authorizationHeaderValues))
            return AuthenticateResult.NoResult();

        var authorizationHeader = authorizationHeaderValues.First();
        if (string.IsNullOrWhiteSpace(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
            return AuthenticateResult.NoResult();

        var token = authorizationHeader["Bearer ".Length..].Trim();
        try
        {
            var validationResult = await ValidateAccessToken(token);
            if (validationResult.Succeeded)
                return validationResult;

            return await ValidateRefreshToken(token);
        }
        catch (SecurityTokenExpiredException)
        {
            return AuthenticateResult.Fail("Access token has expired.");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Token validation failed.");
            return AuthenticateResult.Fail("Authentication failed.");
        }
    }
}