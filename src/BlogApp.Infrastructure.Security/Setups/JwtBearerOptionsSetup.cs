using System.Text;
using BlogApp.Core.Security.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BlogApp.Infrastructure.Security.Setups;

public class JwtBearerOptionsSetup(IOptions<JwtOptions> options) : IPostConfigureOptions<JwtBearerOptions>
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key)),
            RequireExpirationTime = true
        };

        options.SaveToken = true;
    }
}