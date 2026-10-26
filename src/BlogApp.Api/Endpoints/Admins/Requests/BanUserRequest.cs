using BlogApp.Application.Admins.Commands;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Endpoints.Admins.Requests;

public record BanUserRequest([FromRoute(Name = "id")] Guid UserId, bool Ban)
{
    public static explicit operator BanUserCommand(BanUserRequest request) => new(request.UserId, request.Ban);
}