namespace BlogApp.Domain.Models.Admins;

public record AdminUserResult(
    Guid Id,
    string FullName,
    string Username,
    string Email,
    bool EmailConfirmed,
    bool TwoFactorEnabled,
    bool IsBanned,
    string? PhoneNumber,
    DateTime JoinDate,
    int PostCount,
    List<string> Roles);