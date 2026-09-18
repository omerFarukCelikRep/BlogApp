namespace BlogApp.Domain.Models.Auth;

public record ResetPasswordArgs(
    string Token,
    string NewPassword,
    string ConfirmPassword);