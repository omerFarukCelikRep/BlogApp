using BlogApp.Domain.Abstractions.Services;

namespace BlogApp.Application.Auth.Commands;

public class LogoutCommandHandler(IRefreshTokenService refreshTokenService) : IRequestHandler<LogoutCommand,Result>
{
    public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken = default)
    {
        return await refreshTokenService.RevokeAllAsync( cancellationToken);
    }
}