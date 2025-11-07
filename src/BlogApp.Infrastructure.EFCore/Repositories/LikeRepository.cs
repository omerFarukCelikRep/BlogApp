using BlogApp.Domain.Abstractions.Repositories;
using BlogApp.Infrastructure.EFCore.Contexts;

namespace BlogApp.Infrastructure.EFCore.Repositories;

public class LikeRepository(BlogAppDbContext context) : EFBaseRepository<Like, int>(context), ILikeRepository
{
}