using BlogApp.Core.Exceptions;
using BlogApp.Core.Mediator.Handlers;
using BlogApp.Core.Validations;

namespace BlogApp.Application.Auth.Commands;

public class LoginCommandValidator : Validator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(nameof(LoginCommand.Email), x => x.Email)
            .Must(x => !string.IsNullOrWhiteSpace(x), "Email cannot be empty");
        RuleFor(nameof(LoginCommand.Password), x => x.Password)
            .Must(x => !string.IsNullOrWhiteSpace(x), "Password cannot be empty");
    }
}