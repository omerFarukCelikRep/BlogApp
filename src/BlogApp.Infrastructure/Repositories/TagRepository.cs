using BlogApp.Domain.Abstractions.Repositories;
using BlogApp.Infrastructure.Contexts;

namespace BlogApp.Infrastructure.Repositories;

public class TagRepository(BlogAppDbContext context) : EFBaseRepository<Tag, int>(context), ITagRepository
{
}