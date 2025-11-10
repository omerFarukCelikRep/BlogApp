using BlogApp.Domain.Abstractions.Services;

namespace BlogApp.Application.Auth.Commands;

public class RegisterCommandHandler(IAuthenticationService authenticationService)
    : IRequestHandler<RegisterCommand, Result>
{
    public async Task<Result> Handle(RegisterCommand request, CancellationToken cancellationToken = default)
    {
        return await authenticationService.RegisterAsync(request, cancellationToken);
    }
}