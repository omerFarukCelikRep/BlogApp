using System.Security.Cryptography;
using BlogApp.Core.Security.Options;
using BlogApp.Domain.Abstractions.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BlogApp.Infrastructure.Security.Options;

public class JwtBearerOptionsSetup(IOptions<JwtOptions> options, ISigningKeyService signingKeyService)
    : IPostConfigureOptions<JwtBearerOptions>
{
    private readonly JwtOptions _options = options.Value;

    public void PostConfigure(string? name, JwtBearerOptions options)
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _options.Issuer,
            ValidAudience = _options.Audience,
            RequireExpirationTime = true,
            ClockSkew = TimeSpan.Zero,
            IssuerSigningKeyResolver = (_, _, kid, _) =>
            {
                var signingKey = signingKeyService.GetByKeyIdAsync(kid, CancellationToken.None)
                    .GetAwaiter()
                    .GetResult();
                if (signingKey is null)
                    return [];

                var rsa = RSA.Create();
                rsa.ImportFromPem(signingKey.PublicKey);
                return [new RsaSecurityKey(rsa) { KeyId = kid }];
            }
        };

        options.SaveToken = true;
    }
}