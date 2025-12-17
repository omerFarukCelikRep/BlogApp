using BlogApp.Core.Validations;
using BlogApp.Core.Validations.Extensions;

namespace BlogApp.Application.Auth.Commands;

public class LoginCommandValidator : Validator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(nameof(LoginCommand.Email), x => x.Email)
            .NotNull()
            .NotEmpty()
            .Email();

        RuleFor(nameof(LoginCommand.Password), x => x.Password)
            .NotNull()
            .NotEmpty();
    }
}