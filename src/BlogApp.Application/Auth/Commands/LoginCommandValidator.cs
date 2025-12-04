using BlogApp.Core.Mediator.Handlers;

namespace BlogApp.Application.Auth.Commands;

public class LoginCommandValidator : Validator<LoginCommand>
{
    public override Task<IEnumerable<string>> ValidateAsync(LoginCommand request,
        CancellationToken cancellationToken = default)
    {
        IfEmpty(nameof(request.Email), request.Email);
        IfEmpty(nameof(request.Password), request.Password);

        return Task.FromResult<IEnumerable<string>>(Errors);
    }
}