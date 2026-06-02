using BlogApp.Core.Validations;
using BlogApp.Core.Validations.Extensions;

namespace BlogApp.Application.Auth.Commands;

public class RefreshTokenCommandValidator : Validator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(nameof(RefreshTokenCommand.Token), x => x.Token)
            .NotNull()
            .NotEmpty();
    }
}