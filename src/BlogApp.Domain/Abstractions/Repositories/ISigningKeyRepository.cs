using BlogApp.Core.DataAccess.Repositories;

namespace BlogApp.Domain.Abstractions.Repositories;

public interface ISigningKeyRepository : IAsyncFindableRepository<SigningKey, int>
{
}