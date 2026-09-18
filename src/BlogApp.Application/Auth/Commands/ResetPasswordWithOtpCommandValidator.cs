using BlogApp.Core.Validations;
using BlogApp.Core.Validations.Extensions;

namespace BlogApp.Application.Auth.Commands;

public class ResetPasswordWithOtpCommandValidator : Validator<ResetPasswordWithOtpCommand>
{
    public ResetPasswordWithOtpCommandValidator()
    {
        RuleFor(nameof(ResetPasswordWithOtpCommand.Email), x => x.Email).NotNull().NotEmpty();
        RuleFor(nameof(ResetPasswordWithOtpCommand.OtpCode), x => x.OtpCode).NotNull().NotEmpty();
        RuleFor(nameof(ResetPasswordWithOtpCommand.NewPassword), x => x.NewPassword)
            .NotNull()
            .NotEmpty()
            .MinLength(8);

        RuleFor(nameof(ResetPasswordWithOtpCommand.ConfirmPassword), x => x.ConfirmPassword)
            .NotNull()
            .NotEmpty()
            .EqualTo(x => x.NewPassword, nameof(ResetPasswordWithOtpCommand.ConfirmPassword));
    }
}