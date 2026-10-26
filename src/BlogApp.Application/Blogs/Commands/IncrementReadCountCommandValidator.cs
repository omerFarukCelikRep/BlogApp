using BlogApp.Core.Validations;
using BlogApp.Core.Validations.Extensions;

namespace BlogApp.Application.Blogs.Commands;

public class IncrementReadCountCommandValidator : Validator<IncrementReadCountCommand>
{
    public IncrementReadCountCommandValidator()
    {
        RuleFor(nameof(IncrementReadCountCommand.Id), x => x.Id).NotNull().NotEmpty();
    }
}