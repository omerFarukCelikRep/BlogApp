using BlogApp.Core.DataAccess.Repositories;
using BlogApp.Domain.Models.Blogs;

namespace BlogApp.Domain.Abstractions.Repositories;

public interface IBlogRepository : IAsyncInsertableRepository<Blog, int>, IAsyncFindableRepository<Blog, int>,
    IAsyncRepository
{
    Task<List<Blog>> GetTopReadAsync(int count, CancellationToken cancellationToken = default);
    Task<List<Blog>> GetRandomAsync(RandomBlogArgs args, CancellationToken cancellationToken = default);
}