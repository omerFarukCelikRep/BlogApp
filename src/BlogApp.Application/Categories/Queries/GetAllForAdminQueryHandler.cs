using BlogApp.Domain.Abstractions.Services;
using BlogApp.Domain.Models.Categories;

namespace BlogApp.Application.Categories.Queries;

public class GetAllForAdminQueryHandler(ICategoryService categoryService)
    : IRequestHandler<GetAllForAdminQuery, PaginatedResult<CategoryResult>>
{
    public async Task<PaginatedResult<CategoryResult>> Handle(GetAllForAdminQuery request,
        CancellationToken cancellationToken = default)
    {
        return await categoryService.GetAllForAdminAsync(request, cancellationToken);
    }
}