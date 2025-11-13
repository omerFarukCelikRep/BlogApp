namespace BlogApp.Domain.Models.Auth;

public class LoginResult
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}