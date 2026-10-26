using BlogApp.Core.Results;
using BlogApp.Domain.Models.Categories;

namespace BlogApp.Domain.Abstractions.Services;

public interface ICategoryService
{
    Task<Result<List<CategoryResult>>> GetTopAsync(TopCategoryArgs args, CancellationToken cancellationToken = default);
    Task<Result<List<CategoryDetailsResult>>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Result<CategoryDetailsResult>> GetBySlug(GetCategoryBySlugArgs args,
        CancellationToken cancellationToken = default);

    Task<Result<CategoryResult>> CreateAsync(CreateCategoryArgs args, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(DeleteCategoryArgs args, CancellationToken cancellationToken = default);

    Task<PaginatedResult<CategoryResult>> GetAllForAdminAsync(GetAllCategoriesForAdminArgs args,
        CancellationToken cancellationToken = default);
}