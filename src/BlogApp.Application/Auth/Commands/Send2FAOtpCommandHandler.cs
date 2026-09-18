using BlogApp.Domain.Abstractions.Services;

namespace BlogApp.Application.Auth.Commands;

public class Send2FAOtpCommandHandler(IAuthenticationService authenticationService): IRequestHandler<Send2FAOtpCommand,Result>
{
    public async Task<Result> Handle(Send2FAOtpCommand request, CancellationToken cancellationToken = default)
    {
        return await authenticationService.Send2FAOtpAsync(request, cancellationToken);
    }
}