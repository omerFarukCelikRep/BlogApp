using BlogApp.Domain.Abstractions.Repositories;
using BlogApp.Infrastructure.EFCore.Contexts;

namespace BlogApp.Infrastructure.EFCore.Repositories;

public class CommentRepository(BlogAppDbContext context) : EFBaseRepository<Comment, int>(context), ICommentRepository
{
}