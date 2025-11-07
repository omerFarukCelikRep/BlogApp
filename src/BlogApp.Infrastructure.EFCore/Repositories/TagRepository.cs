using BlogApp.Domain.Abstractions.Repositories;
using BlogApp.Infrastructure.EFCore.Contexts;

namespace BlogApp.Infrastructure.EFCore.Repositories;

public class TagRepository(BlogAppDbContext context) : EFBaseRepository<Tag, int>(context), ITagRepository
{
}