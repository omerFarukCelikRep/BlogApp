using BlogApp.Domain.Models.RefreshTokens;

namespace BlogApp.Application.Auth.Commands;

public class RefreshTokenCommand : RefreshTokenArgs, IRequest<Result<RefreshTokenResult>>
{
}