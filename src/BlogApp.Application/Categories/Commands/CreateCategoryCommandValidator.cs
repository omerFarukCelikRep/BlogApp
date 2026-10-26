using BlogApp.Core.Validations;
using BlogApp.Core.Validations.Extensions;

namespace BlogApp.Application.Categories.Commands;

public class CreateCategoryCommandValidator : Validator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(nameof(CreateCategoryCommand.Name), x => x.Name).NotNull().NotEmpty();
    }
}