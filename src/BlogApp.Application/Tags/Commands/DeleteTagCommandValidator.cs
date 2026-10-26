using BlogApp.Core.Validations;
using BlogApp.Core.Validations.Extensions;

namespace BlogApp.Application.Tags.Commands;

public class DeleteTagCommandValidator : Validator<DeleteTagCommand>
{
    public DeleteTagCommandValidator()
    {
        RuleFor(nameof(DeleteTagCommand.TagId), x => x.TagId).NotNull().NotEmpty();
    }
}