using BlogApp.Core.DataAccess.Models;
using BlogApp.Core.DataAccess.Repositories;
using BlogApp.Domain.Models.Admins;
using BlogApp.Domain.Models.Blogs;

namespace BlogApp.Domain.Abstractions.Repositories;

public interface IBlogRepository : IAsyncDeletableRepository<Blog, int>, IAsyncQueryableRepository<Blog, int>,
    IAsyncCountableRepository<Blog, int>,
    IAsyncInsertableRepository<Blog, int>,
    IAsyncFindableRepository<Blog, int>,
    IAsyncRepository
{
    Task<List<Blog>> GetTopReadAsync(int count, CancellationToken cancellationToken = default);
    Task<List<Blog>> GetRandomAsync(RandomBlogArgs args, CancellationToken cancellationToken = default);
    Task<Blog?> GetDetailsAsync(int id, CancellationToken cancellationToken = default);
    Task<IPaginate<Blog>> GetAllByTagAsync(GetBlogByTagArgs args, CancellationToken cancellationToken = default);

    Task<IPaginate<Blog>> GetAllByCategoryAsync(GetBlogByCategoryArgs args,
        CancellationToken cancellationToken = default);

    Task<IPaginate<Blog>> GetAllByAuthorAsync(GetBlogByAuthorArgs args, CancellationToken cancellationToken = default);
    Task<List<Blog>> GetRecentAsync(int count, CancellationToken cancellationToken = default);
    Task<List<DailyCountResult>> GetDailyCountAsync(DateTime from, CancellationToken cancellationToken = default);
    Task<IPaginate<Blog>> GetAllAsync(GetAllForAdminsArgs args, CancellationToken cancellationToken = default);
}