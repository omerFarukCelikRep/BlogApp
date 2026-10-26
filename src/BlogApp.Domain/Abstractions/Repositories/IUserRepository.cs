using BlogApp.Core.DataAccess.Models;
using BlogApp.Core.DataAccess.Repositories;
using BlogApp.Domain.Models.Admins;
using BlogApp.Domain.Models.Authors;

namespace BlogApp.Domain.Abstractions.Repositories;

public interface IUserRepository : IAsyncQueryableRepository<User, Guid>, IAsyncCountableRepository<User, Guid>,
    IAsyncInsertableRepository<User, Guid>,
    IAsyncFindableRepository<User, Guid>, IAsyncUpdateableRepository<User, Guid>
{
    Task<User?> GetAuthorProfileAsync(GetByUsernameArgs args, CancellationToken cancellationToken = default);
    Task<List<User>> GetRecentAsync(int count, CancellationToken cancellationToken = default);
    Task<List<DailyCountResult>> GetDailyCountAsync(DateTime from, CancellationToken cancellationToken = default);
    Task<IPaginate<User>> GetAllAdminsAsync(GetAllAdminsArgs args, CancellationToken cancellationToken = default);
}