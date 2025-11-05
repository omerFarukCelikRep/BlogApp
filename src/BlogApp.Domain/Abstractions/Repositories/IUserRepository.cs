using BlogApp.Core.DataAccess.Repositories;

namespace BlogApp.Domain.Abstractions.Repositories;

public interface IUserRepository : IAsyncFindableRepository<User, Guid>
{
}