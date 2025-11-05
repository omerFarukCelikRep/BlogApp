using BlogApp.Domain.Abstractions.Repositories;
using BlogApp.Infrastructure.Contexts;

namespace BlogApp.Infrastructure.Repositories;

public class LikeRepository(BlogAppDbContext context) : EFBaseRepository<Like, int>(context), ILikeRepository
{
}