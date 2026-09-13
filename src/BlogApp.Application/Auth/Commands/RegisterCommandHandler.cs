using BlogApp.Application.EmailConfirmations.Commands;
using BlogApp.Domain.Abstractions.Services;

namespace BlogApp.Application.Auth.Commands;

public class RegisterCommandHandler(IAuthenticationService authenticationService, IMediator mediator)
    : IRequestHandler<RegisterCommand, Result>
{
    public async Task<Result> Handle(RegisterCommand request, CancellationToken cancellationToken = default)
    {
        var result = await authenticationService.RegisterAsync(request, cancellationToken);

        await mediator.Send<SendEmailConfirmationCommand, Result>(new SendEmailConfirmationCommand(),
            cancellationToken);

        return result;
    }
}