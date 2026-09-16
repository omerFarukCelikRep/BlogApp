using BlogApp.Core.DataAccess.Models;
using BlogApp.Core.DataAccess.Repositories;
using BlogApp.Domain.Models.Comments;

namespace BlogApp.Domain.Abstractions.Repositories;

public interface ICommentRepository : IAsyncCountableRepository<Comment, int>, IAsyncInsertableRepository<Comment, int>,
    IAsyncDeletableRepository<Comment, int>,
    IAsyncFindableRepository<Comment, int>,
    IAsyncRepository
{
    Task<List<Comment>> GetBlogCommentsAsync(int blogId, CancellationToken cancellationToken = default);
}