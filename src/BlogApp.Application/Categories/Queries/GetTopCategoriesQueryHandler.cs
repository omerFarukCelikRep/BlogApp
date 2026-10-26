using BlogApp.Domain.Abstractions.Services;
using BlogApp.Domain.Models.Categories;

namespace BlogApp.Application.Categories.Queries;

public class GetTopCategoriesQueryHandler(ICategoryService categoryService) : IRequestHandler<GetTopCategoriesQuery, Result<List<CategoryResult>>>
{
    public async Task<Result<List<CategoryResult>>> Handle(GetTopCategoriesQuery request, CancellationToken cancellationToken = default)
    {
        return await categoryService.GetTopAsync(request, cancellationToken);
    }
}