using BlogApp.Domain.Abstractions.Repositories;
using BlogApp.Infrastructure.Contexts;

namespace BlogApp.Infrastructure.Repositories;

public class RoleRepository(BlogAppDbContext context) : EFBaseRepository<Role, int>(context), IRoleRepository
{
}