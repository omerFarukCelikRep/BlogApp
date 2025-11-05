using BlogApp.Domain.Abstractions.Repositories;
using BlogApp.Infrastructure.Contexts;
using Microsoft.Extensions.Logging;

namespace BlogApp.Infrastructure.Repositories;

public class BlogRepository(BlogAppDbContext context)
    : EFBaseRepository<Blog, int>(context), IBlogRepository
{
}