using BlogApp.Core.Security.Attributes;
using BlogApp.Domain.Models.Auth;

namespace BlogApp.Application.Auth.Commands;

[Authorize]
public record Verify2FACommand(string Code) : Verify2FAArgs(Code), IRequest<Result<LoginResult>>;