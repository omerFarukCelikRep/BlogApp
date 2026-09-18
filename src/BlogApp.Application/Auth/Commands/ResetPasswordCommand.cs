using BlogApp.Domain.Models.Auth;

namespace BlogApp.Application.Auth.Commands;

public record ResetPasswordCommand(
    string Token,
    string NewPassword,
    string ConfirmPassword) : ResetPasswordArgs(
    Token,
    NewPassword,
    ConfirmPassword), IRequest<Result>;