using BlogApp.Domain.Abstractions.Repositories;
using BlogApp.Infrastructure.EFCore.Contexts;

namespace BlogApp.Infrastructure.EFCore.Repositories;

public class BlogRepository(BlogAppDbContext context)
    : EFBaseRepository<Blog, int>(context), IBlogRepository
{
}