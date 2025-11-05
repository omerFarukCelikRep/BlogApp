using BlogApp.Domain.Abstractions.Repositories;
using BlogApp.Infrastructure.Contexts;

namespace BlogApp.Infrastructure.Repositories;

public class CommentRepository(BlogAppDbContext context) : EFBaseRepository<Comment, int>(context), ICommentRepository
{
}