using BlogApp.Core.Validations;
using BlogApp.Core.Validations.Extensions;

namespace BlogApp.Application.Blogs.Commands;

public class DeleteBlogForAdminCommandValidator : Validator<DeleteBlogForAdminCommand>
{
    public DeleteBlogForAdminCommandValidator()
    {
        RuleFor(nameof(DeleteBlogForAdminCommand.BlogId), x => x.BlogId).NotNull().NotEmpty();
    }
}