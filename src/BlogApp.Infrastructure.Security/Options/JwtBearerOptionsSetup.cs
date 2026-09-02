using System.Globalization;
using System.Security.Claims;
using System.Text;
using BlogApp.Core.Security.Options;
using BlogApp.Domain.Abstractions.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;

namespace BlogApp.Infrastructure.Security.Options;

public class JwtBearerOptionsSetup(IOptions<JwtOptions> options, IServiceProvider serviceProvider)
    : IPostConfigureOptions<JwtBearerOptions>
{
    private readonly JwtOptions _options = options.Value;

    private static string CreateErrorDescription(Exception authFailure)
    {
        IEnumerable<Exception> exceptions = authFailure is AggregateException aggregateException
            ? aggregateException.InnerExceptions
            : [authFailure];
        
        var messages = exceptions
            .Select(ex => ex switch
            {
                SecurityTokenInvalidAudienceException stia =>
                    $"The audience '{stia.InvalidAudience ?? "(null)"}' is invalid",

                SecurityTokenInvalidIssuerException stii =>
                    $"The issuer '{stii.InvalidIssuer ?? "(null)"}' is invalid",

                SecurityTokenNoExpirationException =>
                    "The token has no expiration",

                SecurityTokenInvalidLifetimeException stil =>
                    $"The token lifetime is invalid; NotBefore: " +
                    $"'{stil.NotBefore?.ToString(CultureInfo.InvariantCulture) ?? "(null)"}'" +
                    $", Expires: '{stil.Expires?.ToString(CultureInfo.InvariantCulture) ?? "(null)"}'",

                SecurityTokenNotYetValidException stnyv =>
                    $"The token is not valid before " +
                    $"'{stnyv.NotBefore.ToString(CultureInfo.InvariantCulture)}'",

                SecurityTokenExpiredException ste =>
                    $"The token expired at " +
                    $"'{ste.Expires.ToString(CultureInfo.InvariantCulture)}'",

                SecurityTokenSignatureKeyNotFoundException =>
                    "The signature key was not found",

                SecurityTokenInvalidSignatureException =>
                    "The signature is invalid",

                _ => null
            })
            .Where(m => m is not null)
            .ToList();

        return string.Join("; ", messages);
    }

    private static string BuildWwwAuthenticationHeader(string challenge, string error, string errorDescription)
    {
        var builder = new StringBuilder();
        
        if(challenge.IndexOf(' ')>0)
            builder.Append(',');
        
        if(!string.IsNullOrEmpty(error))
            builder.Append($" error: {error}");
        
        if(!string.IsNullOrEmpty(errorDescription) && !string.IsNullOrEmpty(error)) 
            builder.Append($" error description: {errorDescription}");

        return builder.ToString();
    }

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
            IssuerSigningKeyResolver = (token, securityToken, kid, parameters) =>
            {
                if (string.IsNullOrEmpty(kid))
                    return [];

                var scope = serviceProvider.CreateScope();
                var signingKeyService = scope.ServiceProvider.GetRequiredService<ISigningKeyService>();

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

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var authorization = context.Request.Headers.Authorization.ToString();
                if (!string.IsNullOrEmpty(authorization))
                    context.Token = authorization["Bearer ".Length..].Trim();
                
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<JwtBearerOptionsSetup>>();
                logger.LogWarning(context.Exception, "Jwt Authentication failed : {Message}",context.Exception.Message);
                
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                context.HandleResponse();

                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                if (context.AuthenticateFailure is not null)
                {
                    var errorDescription = CreateErrorDescription(context.AuthenticateFailure);
                    var headerValue =
                        BuildWwwAuthenticationHeader(context.Options.Challenge, "invalid_token", errorDescription);

                    context.Response.Headers.Append(HeaderNames.WWWAuthenticate, headerValue);
                }
                else
                {
                    context.Response.Headers.Append(HeaderNames.WWWAuthenticate, context.Options.Challenge);
                }
                
                return  Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<JwtBearerOptionsSetup>>();
                logger.LogDebug("Token validated for user: {UserId}",context.Principal?.FindFirstValue(JwtRegisteredClaimNames.Sub));
                
                return Task.CompletedTask;
            }
        };
    }
}