using BlogApp.Core.Results;
using BlogApp.Domain.Abstractions.Services;
using BlogApp.Domain.Models.Auth;

namespace BlogApp.Domain.Services;

public class AuthService : IAuthService
{
    public Task<Result<LoginResult>> LoginAsync(LoginArgs args, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}