namespace BlogApp.Domain.Models.Admins;

public record GetAllAdminsArgs(
    string? Search = null,
    string? Role = null,
    bool? IsBanned = null,
    int? Page = 1,
    int? PageSize = 20);