using BlogApp.Domain.Abstractions.Services;
using BlogApp.Domain.Models.RefreshTokens;

namespace BlogApp.Application.Auth.Commands;

public class RefreshTokenCommandHandler(IRefreshTokenService refreshTokenService) : IRequestHandler<RefreshTokenCommand,Result<RefreshTokenResult>>
{
    public async Task<Result<RefreshTokenResult>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken = default)
    {
        return await refreshTokenService.RefreshTokenAsync(request, cancellationToken);
    }
}