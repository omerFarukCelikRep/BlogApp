using BlogApp.Core.Results;
using BlogApp.Domain.Models.Auth;

namespace BlogApp.Domain.Abstractions.Services;

public interface IAuthService
{
    Task<Result<LoginResult>> LoginAsync(LoginArgs args, CancellationToken cancellationToken = default);
}