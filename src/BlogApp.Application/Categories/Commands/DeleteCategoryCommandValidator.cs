using BlogApp.Core.Validations;
using BlogApp.Core.Validations.Extensions;

namespace BlogApp.Application.Categories.Commands;

public class DeleteCategoryCommandValidator : Validator<DeleteCategoryCommand>
{
    public DeleteCategoryCommandValidator()
    {
        RuleFor(nameof(DeleteCategoryCommand.CategoryId), x => x.CategoryId).NotNull().NotEmpty();
    }
}