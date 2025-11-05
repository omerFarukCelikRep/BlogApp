using System.Globalization;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using BlogApp.Domain.Abstractions.Services;
using BlogApp.Domain.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;

namespace BlogApp.Api.Handlers;

public class JwtAuthenticationHandlerr : JwtBearerHandler
{
    private const string JwtTokenStartsWith = "Bearer ";
    private const string AuthenticationTokenName = "access_token";
    private const string AuthenticationInvalidToken = "invalid_token";

    [Obsolete("Obsolete")]
    public JwtAuthenticationHandlerr(IOptionsMonitor<JwtOptions> options, ILoggerFactory logger, UrlEncoder encoder,
        ISystemClock clock) : base(options, logger, encoder, clock)
    {
    }

    public JwtAuthenticationHandlerr(IOptionsMonitor<JwtOptions> options, ILoggerFactory logger, UrlEncoder encoder) :
        base(options, logger, encoder)
    {
    }

    private async Task<TokenValidationParameters> SetupValidationParametersAsync()
    {
        var tokenValidationParameters = Options.TokenValidationParameters.Clone();
        if (Options.ConfigurationManager is BaseConfigurationManager baseConfigurationManager)
        {
            tokenValidationParameters.ConfigurationManager = baseConfigurationManager;
        }
        else
        {
            if (Options.ConfigurationManager is null)
                return tokenValidationParameters;

            var configuration = await Options.ConfigurationManager.GetConfigurationAsync(Context.RequestAborted);
            string[] issuers = [configuration.Issuer];
            tokenValidationParameters.ValidIssuers = tokenValidationParameters.ValidIssuers is null
                ? issuers
                : tokenValidationParameters.ValidIssuers.Concat(issuers);
            tokenValidationParameters.IssuerSigningKeys = tokenValidationParameters.IssuerSigningKeys is null
                ? configuration.SigningKeys
                : tokenValidationParameters.IssuerSigningKeys.Concat(configuration.SigningKeys);
        }

        return tokenValidationParameters;
    }

    private void RecordTokenValidationError(Exception? exception, List<Exception> exceptions)
    {
        if (exception is not null)
        {
            Logger.LogError(exception, "Token validation failed.");
            exceptions.Add(exception);
        }

        if (Options is { RefreshOnIssuerKeyNotFound: true, ConfigurationManager: not null } &&
            exception is SecurityTokenSignatureKeyNotFoundException)
            Options.ConfigurationManager.RequestRefresh();
    }

    private static string CreateErrorDescription(Exception authFailure)
    {
        IReadOnlyCollection<Exception> exceptions = authFailure is AggregateException agEx
            ? agEx.InnerExceptions
            : [authFailure];

        var messages = new List<string>(exceptions.Count);

        foreach (var ex in exceptions)
        {
            // Order sensitive, some of these exceptions derive from others
            // and we want to display the most specific message possible.
            var message = ex switch
            {
                SecurityTokenInvalidAudienceException stia =>
                    $"The audience '{stia.InvalidAudience ?? "(null)"}' is invalid",
                SecurityTokenInvalidIssuerException stii => $"The issuer '{stii.InvalidIssuer ?? "(null)"}' is invalid",
                SecurityTokenNoExpirationException _ => "The token has no expiration",
                SecurityTokenInvalidLifetimeException stil => "The token lifetime is invalid; NotBefore: "
                                                              + $"'{stil.NotBefore?.ToString(CultureInfo.InvariantCulture) ?? "(null)"}'"
                                                              + $", Expires: '{stil.Expires?.ToString(CultureInfo.InvariantCulture) ?? "(null)"}'",
                SecurityTokenNotYetValidException stnyv =>
                    $"The token is not valid before '{stnyv.NotBefore.ToString(CultureInfo.InvariantCulture)}'",
                SecurityTokenExpiredException ste =>
                    $"The token expired at '{ste.Expires.ToString(CultureInfo.InvariantCulture)}'",
                SecurityTokenSignatureKeyNotFoundException _ => "The signature key was not found",
                SecurityTokenInvalidSignatureException _ => "The signature is invalid",
                _ => null,
            };

            if (message is not null)
            {
                messages.Add(message);
            }
        }

        return string.Join("; ", messages);
    }

    private async Task<AuthenticateResult> ValidateRefreshTokenAsync()
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
        return AuthenticateResult.Success(new(principal, Scheme.Name));
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        try
        {
            var messageReceivedContext = new MessageReceivedContext(Context, Scheme, Options);
            await Events.MessageReceived(messageReceivedContext);
            if (messageReceivedContext.Result is not null)
                return messageReceivedContext.Result;

            var token = messageReceivedContext.Token;
            if (string.IsNullOrEmpty(token))
            {
                var authorization = Request.Headers.Authorization.ToString();
                if (string.IsNullOrEmpty(authorization))
                    return AuthenticateResult.NoResult();

                if (authorization.StartsWith(JwtTokenStartsWith, StringComparison.OrdinalIgnoreCase))
                    token = authorization[JwtTokenStartsWith.Length..].Trim();

                if (string.IsNullOrEmpty(token))
                    return AuthenticateResult.NoResult();
            }

            var tokenValidationParameters = await SetupValidationParametersAsync();
            List<Exception> exceptions = [];
            SecurityToken? validatedToken = null;
            ClaimsPrincipal? principal = null;

            if (!Options.UseSecurityTokenValidators)
            {
                foreach (var tokenHandler in Options.TokenHandlers)
                {
                    try
                    {
                        var tokenValidationResult =
                            await tokenHandler.ValidateTokenAsync(token, tokenValidationParameters);
                        if (tokenValidationResult.IsValid)
                        {
                            principal = new ClaimsPrincipal(tokenValidationResult.ClaimsIdentity);
                            validatedToken = tokenValidationResult.SecurityToken;
                            break;
                        }

                        RecordTokenValidationError(tokenValidationResult.Exception
                                                   ?? new SecurityTokenValidationException(
                                                       $"The TokenHandler: '{tokenHandler}', was unable to validate the Token."),
                            exceptions);
                    }
                    catch (Exception ex)
                    {
                        RecordTokenValidationError(ex, exceptions);
                    }
                }
            }

            if (principal is not null && validatedToken is not null)
            {
                var tokenValidatedContext = new TokenValidatedContext(Context, Scheme, Options)
                {
                    Principal = principal,
                    SecurityToken = validatedToken,
                    Properties =
                    {
                        ExpiresUtc = validatedToken.ValidTo,
                        IssuedUtc = validatedToken.ValidFrom
                    }
                };

                await Events.TokenValidated(tokenValidatedContext);
                if (tokenValidatedContext.Result is not null)
                    return tokenValidatedContext.Result;

                if (Options.SaveToken)
                {
                    tokenValidatedContext.Properties.StoreTokens([
                        new AuthenticationToken
                        {
                            Name = AuthenticationTokenName,
                            Value = token
                        }
                    ]);
                }

                tokenValidatedContext.Success();
                return tokenValidatedContext.Result!;
            }

            if (exceptions is not { Count: > 0 })
                return AuthenticateResult.Fail(!Options.UseSecurityTokenValidators
                    ? "No TokenHandler was able to validate the token."
                    : "No SecurityTokenValidator available for token.");

            var authenticationFailedContext = new AuthenticationFailedContext(Context, Scheme, Options)
            {
                Exception = exceptions.Count > 1 ? new AggregateException(exceptions) : exceptions[0]
            };

            await Events.AuthenticationFailed(authenticationFailedContext);
            return authenticationFailedContext.Result ??
                   AuthenticateResult.Fail(authenticationFailedContext.Exception);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Token validation failed.");

            var authenticationFailedContext = new AuthenticationFailedContext(Context, Scheme, Options)
            {
                Exception = ex
            };

            await Events.AuthenticationFailed(authenticationFailedContext);
            if (authenticationFailedContext.Result is not null)
                return authenticationFailedContext.Result;

            throw;
        }
    }

    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        var authResult = await HandleAuthenticateOnceSafeAsync();
        var eventContext = new JwtBearerChallengeContext(Context, Scheme, Options, properties)
        {
            AuthenticateFailure = authResult.Failure
        };

        if (Options.IncludeErrorDetails && eventContext.AuthenticateFailure is not null)
        {
            eventContext.Error = AuthenticationInvalidToken;
            eventContext.ErrorDescription = CreateErrorDescription(eventContext.AuthenticateFailure);
        }

        await Events.Challenge(eventContext);
        if (eventContext.Handled)
            return;

        Response.StatusCode = StatusCodes.Status401Unauthorized;

        if (string.IsNullOrEmpty(eventContext.Error) && string.IsNullOrEmpty(eventContext.ErrorDescription) &&
            string.IsNullOrEmpty(eventContext.ErrorUri))
        {
            Response.Headers.Append(HeaderNames.WWWAuthenticate, Options.Challenge);
            return;
        }

        var builder = new StringBuilder(Options.Challenge);
        if (Options.Challenge.IndexOf(' ') > 0)
        {
            // Only add a comma after the first param, if any
            builder.Append(',');
        }

        if (!string.IsNullOrEmpty(eventContext.Error))
        {
            builder.Append(" error=\"");
            builder.Append(eventContext.Error);
            builder.Append('\"');
        }

        if (!string.IsNullOrEmpty(eventContext.ErrorDescription))
        {
            if (!string.IsNullOrEmpty(eventContext.Error))
            {
                builder.Append(',');
            }

            builder.Append(" error_description=\"");
            builder.Append(eventContext.ErrorDescription);
            builder.Append('\"');
        }

        if (!string.IsNullOrEmpty(eventContext.ErrorUri))
        {
            if (!string.IsNullOrEmpty(eventContext.Error) ||
                !string.IsNullOrEmpty(eventContext.ErrorDescription))
            {
                builder.Append(',');
            }

            builder.Append(" error_uri=\"");
            builder.Append(eventContext.ErrorUri);
            builder.Append('\"');
        }

        Response.Headers.Append(HeaderNames.WWWAuthenticate, builder.ToString());
    }
}