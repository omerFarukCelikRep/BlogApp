using BlogApp.Core.Validations;
using BlogApp.Core.Validations.Extensions;

namespace BlogApp.Application.Comments.Commands;

public class DeleteCommentCommandValidator : Validator<DeleteCommentCommand>
{
    public DeleteCommentCommandValidator()
    {
        RuleFor(nameof(DeleteCommentCommand.Id), x => x.Id).NotNull().NotEmpty();
    }
}