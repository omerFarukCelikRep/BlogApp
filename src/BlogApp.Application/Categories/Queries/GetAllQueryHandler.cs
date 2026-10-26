using BlogApp.Domain.Abstractions.Services;
using BlogApp.Domain.Models.Categories;

namespace BlogApp.Application.Categories.Queries;

public class GetAllQueryHandler(ICategoryService categoryService) : IRequestHandler<GetAllQuery, Result<List<CategoryDetailsResult>>>
{
    public async Task<Result<List<CategoryDetailsResult>>> Handle(GetAllQuery request, CancellationToken cancellationToken = default)
    {
        return await categoryService.GetAllAsync(cancellationToken);
    }
}