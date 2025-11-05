using BlogApp.Core.Mediator.Handlers;

namespace BlogApp.Application.Auth.Login.Commands;

public class LoginCommandValidator : Validator<LoginCommand>
{
    public override Task<IEnumerable<string>> ValidateAsync(LoginCommand request,
        CancellationToken cancellationToken = default)
    {
        List<string> errors = [];
        if (string.IsNullOrEmpty(request.Email))
        {
            errors.Add(CreateError(nameof(request.Email)));
        }

        return Task.FromResult<IEnumerable<string>>(errors);
    }
}