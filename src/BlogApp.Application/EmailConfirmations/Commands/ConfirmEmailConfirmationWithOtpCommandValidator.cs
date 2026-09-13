using BlogApp.Core.Validations;
using BlogApp.Core.Validations.Extensions;

namespace BlogApp.Application.EmailConfirmations.Commands;

public class ConfirmEmailConfirmationWithOtpCommandValidator : Validator<ConfirmEmailConfirmationWithOtpCommand>
{
    public ConfirmEmailConfirmationWithOtpCommandValidator()
    {
        RuleFor(nameof(ConfirmEmailConfirmationWithOtpCommand.OtpCode), x => x.OtpCode).NotNull().NotEmpty();
    }
}