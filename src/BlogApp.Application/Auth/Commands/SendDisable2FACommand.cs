using BlogApp.Core.Security.Attributes;

namespace BlogApp.Application.Auth.Commands;

[Authorize]
public record SendDisable2FACommand : IRequest<Result>;