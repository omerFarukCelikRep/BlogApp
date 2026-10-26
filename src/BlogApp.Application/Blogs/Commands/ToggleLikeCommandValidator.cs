using BlogApp.Core.Validations;
using BlogApp.Core.Validations.Extensions;

namespace BlogApp.Application.Blogs.Commands;

public class ToggleLikeCommandValidator : Validator<ToggleLikeCommand>
{
    public ToggleLikeCommandValidator()
    {
        RuleFor(nameof(ToggleLikeCommand.Id), x => x.Id).NotNull().NotEmpty();
    }
}