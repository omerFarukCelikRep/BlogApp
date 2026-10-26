using BlogApp.Domain.Abstractions.Services;
using BlogApp.Domain.Models.Categories;

namespace BlogApp.Application.Categories.Commands;

public class CreateCategoryCommandHandler(ICategoryService categoryService) : IRequestHandler<CreateCategoryCommand,Result<CategoryResult>>
{
    public async Task<Result<CategoryResult>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken = default)
    {
        return await categoryService.CreateAsync(request, cancellationToken);
    }
}