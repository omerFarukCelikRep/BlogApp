using BlogApp.Core.Results;
using BlogApp.Domain.Models.Auth;

namespace BlogApp.Domain.Abstractions.Services;

public interface IAuthenticationService
{
    Task<Result<LoginResult>> LoginAsync(LoginArgs args, CancellationToken cancellationToken = default);
    Task<Result> RegisterAsync(RegisterArgs args, CancellationToken cancellationToken = default);
}