using BlogApp.Core.Validations;
using BlogApp.Core.Validations.Extensions;

namespace BlogApp.Application.Auth.Commands;

public class ForgotPasswordCommandValidator : Validator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
    {
        RuleFor(nameof(ForgotPasswordCommand.Email), x => x.Email).NotNull().NotEmpty();
    }
}