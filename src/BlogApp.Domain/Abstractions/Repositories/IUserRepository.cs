using BlogApp.Core.DataAccess.Repositories;

namespace BlogApp.Domain.Abstractions.Repositories;

public interface IUserRepository : IAsyncInsertableRepository<User, Guid>, IAsyncFindableRepository<User, Guid>
{
}