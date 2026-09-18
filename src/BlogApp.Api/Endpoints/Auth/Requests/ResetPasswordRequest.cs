using BlogApp.Application.Auth.Commands;

namespace BlogApp.Api.Endpoints.Auth.Requests;

public record ResetPasswordRequest(string Token, string NewPassword, string ConfirmPassword)
{
    public static explicit operator ResetPasswordCommand(ResetPasswordRequest request) =>
        new(request.Token, request.NewPassword, request.ConfirmPassword);
}