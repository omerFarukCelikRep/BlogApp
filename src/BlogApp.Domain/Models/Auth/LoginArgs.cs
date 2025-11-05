namespace BlogApp.Domain.Models.Auth;

public class LoginArgs
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}