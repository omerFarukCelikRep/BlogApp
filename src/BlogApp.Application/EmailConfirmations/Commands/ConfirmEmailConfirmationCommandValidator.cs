using BlogApp.Core.Validations;
using BlogApp.Core.Validations.Extensions;

namespace BlogApp.Application.EmailConfirmations.Commands;

public class ConfirmEmailConfirmationCommandValidator : Validator<ConfirmEmailConfirmationCommand>
{
    public ConfirmEmailConfirmationCommandValidator()
    {
        RuleFor(nameof(ConfirmEmailConfirmationCommand.Token), x => x.Token).NotNull().NotEmpty();
    }
}