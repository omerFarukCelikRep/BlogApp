using BlogApp.Core.Security.Enums;

namespace BlogApp.Core.Security.Models;

public class TokenArgs
{
    public required Guid UserId { get; init; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Username { get; init; }
    public required string Email { get; init; }
    public required IReadOnlyList<string> Roles { get; init; }
    public IReadOnlyList<string> Permissions { get; init; } = [];
}