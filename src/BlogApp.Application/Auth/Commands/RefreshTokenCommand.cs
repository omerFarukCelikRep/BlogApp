using BlogApp.Domain.Models.RefreshTokens;

namespace BlogApp.Application.Auth.Commands;

public record RefreshTokenCommand(string Token) : RefreshTokenArgs(Token), IRequest<Result<RefreshTokenResult>>;