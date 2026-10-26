using BlogApp.Core.Security.Attributes;
using BlogApp.Core.Security.Enums;
using BlogApp.Domain.Models.Admins;

namespace BlogApp.Application.Admins.Commands;

[Authorize(Role.Admin)]
public record AssignRoleCommand(Guid UserId, Role Role, bool Assign)
    : AssignRoleArgs(UserId, Role, Assign), IRequest<Result>;