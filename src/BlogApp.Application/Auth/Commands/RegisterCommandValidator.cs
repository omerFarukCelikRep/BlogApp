using BlogApp.Core.Validations;
using BlogApp.Core.Validations.Extensions;

namespace BlogApp.Application.Auth.Commands;

public class RegisterCommandValidator : Validator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(nameof(RegisterCommand.FirstName), x => x.FirstName)
            .NotNull()
            .NotEmpty()
            .MinLength(2)
            .MaxLength(256);

        RuleFor(nameof(RegisterCommand.LastName), x => x.LastName)
            .NotNull()
            .NotEmpty()
            .MinLength(2)
            .MaxLength(256);

        RuleFor(nameof(RegisterCommand.Email), x => x.Email)
            .NotNull()
            .NotEmpty()
            .Email();

        RuleFor(nameof(RegisterCommand.Username), x => x.Username)
            .NotNull()
            .NotEmpty()
            .MinLength(3)
            .MaxLength(256);

        RuleFor(nameof(RegisterCommand.Password), x => x.Password)
            .NotNull()
            .NotEmpty()
            .MinLength(8);

        RuleFor(nameof(RegisterCommand.ConfirmedPassword), x => x.ConfirmedPassword)
            .NotNull()
            .NotEmpty()
            .EqualTo(x => x.Password, nameof(RegisterCommand.ConfirmedPassword));
    }
}