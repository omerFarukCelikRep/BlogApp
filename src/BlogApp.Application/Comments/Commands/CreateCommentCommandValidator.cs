using BlogApp.Core.Validations;
using BlogApp.Core.Validations.Extensions;

namespace BlogApp.Application.Comments.Commands;

public class CreateCommentCommandValidator : Validator<CreateCommentCommand>
{
    public CreateCommentCommandValidator()
    {
        RuleFor(nameof(CreateCommentCommand.BlogId), x => x.BlogId).NotNull().NotEmpty();
        RuleFor(nameof(CreateCommentCommand.Content), x => x.Content).NotNull()
            .NotEmpty()
            .MinLength(2)
            .MaxLength(2000);

        RuleFor(nameof(CreateCommentCommand.ParentId), x => x.ParentId)
            .NotNull()
            .NotEmpty()
            .When(x => x.ParentId.HasValue);
    }
}