using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BlogApp.Domain.Abstractions.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;

namespace BlogApp.Domain.Services;

public class TokenService(IOptions<JwtBearerOptions> options, ILoggerFactory loggerFactory) : ITokenService
{
    private readonly JwtBearerOptions _options = options.Value;
    private readonly ILogger<TokenService> _logger = loggerFactory.CreateLogger<TokenService>();

    public async Task<AuthenticateResult> ValidateToken(string token, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return await Task.FromCanceled<AuthenticateResult>(cancellationToken);
        }

        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            var validationResult = await tokenHandler.ValidateTokenAsync(token, _options.TokenValidationParameters);
            if (!validationResult.IsValid)
            {
                return AuthenticateResult.Fail(validationResult.Exception);
            }

            var tokenn = tokenHandler.ReadJwtToken(token);
            if (tokenn is null)
            {
                return AuthenticateResult.Fail("Token is invalid");
            }

            var claimsIdentity = new ClaimsIdentity(tokenn.Claims, "Token");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            return AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, "Jwt"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error {Message}", ex.Message);
            throw;
        }
    }
}