using BlogApp.Core.Security.Attributes;
using BlogApp.Domain.Models.RefreshTokens;

namespace BlogApp.Application.Auth.Commands;

[Authorize]
public record RefreshTokenCommand(string Token) : RefreshTokenArgs(Token), IRequest<Result<RefreshTokenResult>>;