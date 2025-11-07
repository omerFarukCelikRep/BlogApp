using BlogApp.Domain.Abstractions.Repositories;
using BlogApp.Infrastructure.EFCore.Contexts;

namespace BlogApp.Infrastructure.EFCore.Repositories;

public class CategoryRepository(BlogAppDbContext context) : EFBaseRepository<Category, int>(context), ICategoryRepository
{
}