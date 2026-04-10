namespace BlogApp.Domain.Models.Auth;

public record RegisterArgs(
    string FirstName,
    string LastName,
    string Email,
    string Username,
    string Password);