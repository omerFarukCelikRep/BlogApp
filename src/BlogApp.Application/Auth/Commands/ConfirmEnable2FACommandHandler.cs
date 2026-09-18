using BlogApp.Domain.Abstractions.Services;

namespace BlogApp.Application.Auth.Commands;

public class ConfirmEnable2FACommandHandler(IAuthenticationService authenticationService) : IRequestHandler<ConfirmEnable2FACommand,Result>
{
    public async Task<Result> Handle(ConfirmEnable2FACommand request, CancellationToken cancellationToken = default)
    {
        return await authenticationService.ConfirmEnable2FAAsync(request, cancellationToken);
    }
}