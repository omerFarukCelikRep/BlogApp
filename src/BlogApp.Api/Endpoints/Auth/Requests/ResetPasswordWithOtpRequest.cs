using BlogApp.Application.Auth.Commands;
using BlogApp.Core.Security.Attributes;

namespace BlogApp.Api.Endpoints.Auth.Requests;

public record ResetPasswordWithOtpRequest(
    [Sanitize] string Email,
    string OtpCode,
    string NewPassword,
    string ConfirmPassword)
{
    public static explicit operator ResetPasswordWithOtpCommand(ResetPasswordWithOtpRequest request) =>
        new(request.Email, request.OtpCode, request.NewPassword, request.ConfirmPassword);
}