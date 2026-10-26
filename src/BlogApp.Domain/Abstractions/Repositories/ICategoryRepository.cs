using BlogApp.Core.DataAccess.Repositories;
using BlogApp.Domain.Models.Categories;

namespace BlogApp.Domain.Abstractions.Repositories;

public interface ICategoryRepository : IAsyncPaginateRepository<Category, int>,
    IAsyncDeletableRepository<Category, int>,
    IAsyncInsertableRepository<Category, int>, IAsyncCountableRepository<Category, int>,
    IAsyncQueryableRepository<Category, int>,
    IAsyncFindableRepository<Category, int>, IAsyncRepository
{
    Task<List<Category>> GetTopAsync(int count, CancellationToken cancellationToken = default);
    Task<List<Category>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Category?> GetBySlugAsync(GetCategoryBySlugArgs args, CancellationToken cancellationToken = default);
}