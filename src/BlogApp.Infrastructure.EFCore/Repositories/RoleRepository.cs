using BlogApp.Domain.Abstractions.Repositories;
using BlogApp.Infrastructure.EFCore.Contexts;

namespace BlogApp.Infrastructure.EFCore.Repositories;

public class RoleRepository(BlogAppDbContext context) : EFBaseRepository<Role, int>(context), IRoleRepository
{
}