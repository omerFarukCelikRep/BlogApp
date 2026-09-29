using BlogApp.Core.Results;
using BlogApp.Domain.Models.Authors;

namespace BlogApp.Domain.Abstractions.Services;

public interface IAuthorService
{
    Task<Result<AuthorProfileResult>> GetByUsernameAsync(GetByUsernameArgs args, CancellationToken cancellationToken = default);
}