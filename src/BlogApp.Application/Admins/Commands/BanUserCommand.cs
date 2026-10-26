using BlogApp.Core.Security.Attributes;
using BlogApp.Domain.Models.Admins;
using Role = BlogApp.Core.Security.Enums.Role;

namespace BlogApp.Application.Admins.Commands;

[Authorize(Role.Admin)]
public record BanUserCommand(Guid UserId, bool Ban) : BanUserArgs(UserId,Ban), IRequest<Result>;