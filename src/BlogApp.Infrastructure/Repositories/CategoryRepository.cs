using BlogApp.Domain.Abstractions.Repositories;
using BlogApp.Infrastructure.Contexts;

namespace BlogApp.Infrastructure.Repositories;

public class CategoryRepository(BlogAppDbContext context) : EFBaseRepository<Category, int>(context), ICategoryRepository
{
}