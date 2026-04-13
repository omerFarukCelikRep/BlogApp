using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using BlogApp.Core.Security.Abstractions;
using BlogApp.Core.Security.Constants;
using BlogApp.Core.Security.Models;
using BlogApp.Core.Security.Options;
using BlogApp.Domain.Abstractions.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BlogApp.Infrastructure.Security.Providers;

public partial class JwtProvider(
    ISigningKeyRepository signingKeyRepository,
    IOptions<JwtOptions> jwtOptions,
    ILogger<JwtProvider> logger) : IJwtProvider
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    private static List<Claim> BuildClaims(TokenArgs args)
    {
        List<Claim> claims =
        [
            new(JwtRegisteredClaimNames.Sub, args.UserId.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.CreateVersion7().ToString()),
            new(ClaimTypes.NameIdentifier, args.UserId.ToString()),
            new(ClaimTypes.GivenName, args.FirstName),
            new(ClaimTypes.Surname, args.LastName),
            new(ClaimTypes.Email, args.Email),
            new(ClaimTypes.Name, args.Username)
        ];

        claims.AddRange(args.Roles.Select(userRole => new Claim(ClaimTypes.Role, userRole!.ToString())));
        claims.AddRange(args.Permissions.Select(permission =>
            new Claim(CustomClaimTypes.Permissions, permission!.ToString())));

        return claims;
    }

    public async Task<string> GenerateTokenAsync(TokenArgs args, CancellationToken cancellationToken = default)
    {
        var signingKey = await signingKeyRepository.GetAsync(x => x.IsActive, false, cancellationToken)
                         ?? throw new UnauthorizedAccessException();

        var rsa = RSA.Create();
        rsa.ImportFromPem(signingKey.PrivateKey);

        var rsaSecurityKey = new RsaSecurityKey(rsa)
        {
            KeyId = signingKey.KeyId
        };

        var credentials = new SigningCredentials(rsaSecurityKey, SecurityAlgorithms.RsaSha512);
        var claims = BuildClaims(args);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience,
            Expires = DateTimeOffset.Now.AddMinutes(_jwtOptions.ExpirationMinutes).DateTime,
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public async Task<ClaimsPrincipal?> ValidateTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            var kid = jwtToken.Header.Kid;
            if (string.IsNullOrEmpty(kid))
                return null;

            var signingKey = await signingKeyRepository.GetByKeyIdAsync(kid, cancellationToken);
            if (signingKey is null)
                return null;

            var rsa = RSA.Create();
            rsa.ImportFromPem(signingKey.PublicKey);

            var validatorParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtOptions.Issuer,
                ValidAudience = _jwtOptions.Audience,
                IssuerSigningKey = new RsaSecurityKey(rsa) { KeyId = signingKey.KeyId },
                ClockSkew = TimeSpan.Zero
            };

            var validationResult = await tokenHandler.ValidateTokenAsync(token, validatorParameters);
            return !validationResult.IsValid
                ? null
                : new ClaimsPrincipal(validationResult.ClaimsIdentity);
        }
        catch (Exception e)
        {
            LogMessage(e.Message);
            return null;
        }
    }

    [LoggerMessage(LogLevel.Error, "{Message}")]
    partial void LogMessage(string message);
}