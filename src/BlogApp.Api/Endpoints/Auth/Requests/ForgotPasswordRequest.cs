using BlogApp.Application.Auth.Commands;
using BlogApp.Core.Security.Attributes;

namespace BlogApp.Api.Endpoints.Auth.Requests;

public record ForgotPasswordRequest([Sanitize] string Email)
{
    public static explicit operator ForgotPasswordCommand(ForgotPasswordRequest request) => new(request.Email);
}