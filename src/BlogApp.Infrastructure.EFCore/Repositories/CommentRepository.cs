using BlogApp.Core.DataAccess.Enums;
using BlogApp.Core.DataAccess.Models;
using BlogApp.Core.EFCore.Extensions;
using BlogApp.Domain.Abstractions.Repositories;
using BlogApp.Domain.Models.Comments;
using BlogApp.Infrastructure.EFCore.Contexts;

namespace BlogApp.Infrastructure.EFCore.Repositories;

public class CommentRepository(BlogAppDbContext context) : EFBaseRepository<Comment, int>(context), ICommentRepository
{
    public async Task<List<Comment>> GetBlogCommentsAsync(int blogId, CancellationToken cancellationToken = default)
    {
        return await GetAll(false)
            .Where(x => x.BlogId == blogId)
            .Include(x => x.User)
            .OrderBy(x => x.CreatedDate)
            .ToListAsync(cancellationToken);
    }
}