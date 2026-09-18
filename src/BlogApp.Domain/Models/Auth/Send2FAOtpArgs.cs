namespace BlogApp.Domain.Models.Auth;

public record Send2FAOtpArgs(Guid? UserId = null, TwoFactorPurpose Purpose = TwoFactorPurpose.Login);