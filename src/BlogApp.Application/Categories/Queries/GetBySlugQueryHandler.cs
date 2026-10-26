using BlogApp.Domain.Abstractions.Services;
using BlogApp.Domain.Models.Categories;

namespace BlogApp.Application.Categories.Queries;

public class GetBySlugQueryHandler(ICategoryService categoryService)
    : IRequestHandler<GetBySlugQuery, Result<CategoryDetailsResult>>
{
    public async Task<Result<CategoryDetailsResult>> Handle(GetBySlugQuery request,
        CancellationToken cancellationToken = default)
    {
        return await categoryService.GetBySlug(request, cancellationToken);
    }
}