using BlogApp.Core.Validations;
using BlogApp.Core.Validations.Extensions;

namespace BlogApp.Application.Auth.Commands;

public class ResetPasswordCommandValidator : Validator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(nameof(ResetPasswordCommand.Token), x => x.Token).NotNull().NotEmpty();
        RuleFor(nameof(ResetPasswordCommand.NewPassword), x => x.NewPassword)
            .NotNull()
            .NotEmpty()
            .MinLength(8);

        RuleFor(nameof(ResetPasswordCommand.ConfirmPassword), x => x.ConfirmPassword)
            .NotNull()
            .NotEmpty()
            .EqualTo(x => x.NewPassword, nameof(ResetPasswordCommand.ConfirmPassword));
    }
}