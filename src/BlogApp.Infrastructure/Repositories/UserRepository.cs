using BlogApp.Domain.Abstractions.Repositories;
using BlogApp.Infrastructure.Contexts;

namespace BlogApp.Infrastructure.Repositories;

public class UserRepository(BlogAppDbContext context) : EFBaseRepository<User, Guid>(context), IUserRepository
{
}