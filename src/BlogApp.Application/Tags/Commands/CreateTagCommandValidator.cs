using BlogApp.Core.Validations;
using BlogApp.Core.Validations.Extensions;

namespace BlogApp.Application.Tags.Commands;

public class CreateTagCommandValidator : Validator<CreateTagCommand>
{
    public CreateTagCommandValidator()
    {
        RuleFor(nameof(CreateTagCommand.Name), x => x.Name).NotNull().NotEmpty();
    }
}