using System.Linq.Expressions;
using BlogApp.Core.DataAccess.Repositories;

namespace BlogApp.Domain.Abstractions.Repositories;

public interface ILikeRepository : IAsyncCountableRepository<Like, int>, IAsyncInsertableRepository<Like, int>,
    IAsyncDeletableRepository<Like, int>,
    IAsyncFindableRepository<Like, int>,
    IAsyncRepository
{
}