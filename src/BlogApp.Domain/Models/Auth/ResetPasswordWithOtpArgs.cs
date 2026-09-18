namespace BlogApp.Domain.Models.Auth;

public record ResetPasswordWithOtpArgs(string Email, string OtpCode, string NewPassword, string ConfirmPassword);