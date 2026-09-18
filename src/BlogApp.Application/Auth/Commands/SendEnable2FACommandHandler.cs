using BlogApp.Domain.Abstractions.Services;

namespace BlogApp.Application.Auth.Commands;

public class SendEnable2FACommandHandler(IAuthenticationService authenticationService) : IRequestHandler<SendEnable2FACommand,Result>
{
    public async Task<Result> Handle(SendEnable2FACommand request, CancellationToken cancellationToken = default)
    {
        return await authenticationService.Enable2FAAsync(request, cancellationToken);
    }
}