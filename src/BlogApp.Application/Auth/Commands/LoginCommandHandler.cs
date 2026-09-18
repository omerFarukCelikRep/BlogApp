using BlogApp.Core.Results;
using BlogApp.Core.Security.Constants;
using BlogApp.Domain.Abstractions.Services;
using BlogApp.Domain.Enums;
using BlogApp.Domain.Models.Auth;

namespace BlogApp.Application.Auth.Commands;

public class LoginCommandHandler(IAuthenticationService authenticationService, IMediator mediator)
    : IRequestHandler<LoginCommand, Result<LoginResult>>
{
    public async Task<Result<LoginResult>> Handle(LoginCommand request, CancellationToken cancellationToken = default)
    {
        var result = await authenticationService.LoginAsync(request, cancellationToken);

        if (result.Data?.TwoFactorEnabled != true)
            return result;

        await mediator.Send<Send2FAOtpCommand, Result>(
            new Send2FAOtpCommand(result.Data.UserId!.Value, TwoFactorPurpose.Login), cancellationToken);

        return Result<LoginResult>.Success(data: result.Data with { Scope = TokenScope.TwoFactorChallenge });
    }
}