using BlogApp.Core.Validations;
using BlogApp.Core.Validations.Extensions;

namespace BlogApp.Application.Auth.Commands;

public class RegisterCommandValidator : Validator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(nameof(RegisterCommand.Email), x => x.Email)
            .NotNull()
            .NotEmpty()
            .Email();

        RuleFor(nameof(RegisterCommand.Password), x => x.Password)
            .NotNull()
            .NotEmpty();
    }
}