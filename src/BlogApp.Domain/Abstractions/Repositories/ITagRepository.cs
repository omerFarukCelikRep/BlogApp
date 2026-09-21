using BlogApp.Core.DataAccess.Repositories;

namespace BlogApp.Domain.Abstractions.Repositories;

public interface ITagRepository : IAsyncPaginateRepository<Tag, int>, IAsyncDeletableRepository<Tag, int>,
    IAsyncInsertableRepository<Tag, int>,
    IAsyncCountableRepository<Tag, int>,
    IAsyncQueryableRepository<Tag, int>,
    IAsyncFindableRepository<Tag, int>,
    IAsyncRepository
{
    Task<List<Tag>> GetTopAsync(int count, CancellationToken cancellationToken = default);
}