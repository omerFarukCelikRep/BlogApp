namespace BlogApp.Domain.Models.Auth;

public record ConfirmEnable2FAArgs(string Code, string PhoneNumber);