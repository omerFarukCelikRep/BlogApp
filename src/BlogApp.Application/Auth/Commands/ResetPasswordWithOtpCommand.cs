using BlogApp.Domain.Models.Auth;

namespace BlogApp.Application.Auth.Commands;

public record ResetPasswordWithOtpCommand(
    string Email,
    string OtpCode,
    string NewPassword,
    string ConfirmPassword)
    : ResetPasswordWithOtpArgs(
        Email,
        OtpCode,
        NewPassword,
        ConfirmPassword), IRequest<Result>;