using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BlogApp.Core.Security.Abstractions;
using BlogApp.Core.Security.Constants;
using BlogApp.Core.Security.Options;
using BlogApp.Domain.Abstractions.Repositories;
using BlogApp.Domain.Exceptions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BlogApp.Infrastructure.Security.Providers;

public class JwtProvider(
    IUserRepository userRepository,
    ISigningKeyRepository signingKeyRepository,
    IOptions<JwtOptions> jwtOptions) : IJwtProvider
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public async Task<string> GenerateTokenAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(userId, false, cancellationToken);
        if (user is null)
            throw new UserNotFoundException();

        var signingKey =
            await signingKeyRepository.GetAsync(x => x.IsActive, tracking: false, cancellationToken: cancellationToken);
        if (signingKey is null)
            throw new UnauthorizedAccessException();

        var privateKeyBytes = Convert.FromBase64String(signingKey.PrivateKey);
        var rsa = RSA.Create();
        rsa.ImportRSAPrivateKey(privateKeyBytes, out _);
        var rsaSecurityKey = new RsaSecurityKey(rsa)
        {
            KeyId = signingKey.KeyId
        };

        var credentials = new SigningCredentials(rsaSecurityKey, SecurityAlgorithms.HmacSha512);
        List<Claim> claims =
        [
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.CreateVersion7().ToString()),
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.GivenName, user.FirstName),
            new(ClaimTypes.Surname, user.LastName),
            new(ClaimTypes.Email, user.Email)
        ];

        var userRoles = user.Roles.Select(x => x.Role).ToList();
        var userPermissions = userRoles.SelectMany(x => x!.RolePermissions).Select(x => x.Permission);

        claims.AddRange(userRoles.Select(userRole => new Claim(ClaimTypes.Role, userRole!.ToString())));
        claims.AddRange(userPermissions.Select(permission =>
            new Claim(CustomClaimTypes.Permission, permission!.ToString())));

        var token = new JwtSecurityToken(issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: DateTimeOffset.Now.AddMinutes(_jwtOptions.ExpirationMinutes).DateTime,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        throw new NotImplementedException();
    }
}