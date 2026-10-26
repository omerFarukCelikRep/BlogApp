using BlogApp.Application.Admins.Commands;
using BlogApp.Core.Security.Enums;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Endpoints.Admins.Requests;

public record AssignRoleRequest([FromRoute(Name = "id")] Guid UserId, Role Role, bool Assign)
{
    public static explicit operator AssignRoleCommand(AssignRoleRequest request) =>
        new(request.UserId, request.Role, request.Assign);
}