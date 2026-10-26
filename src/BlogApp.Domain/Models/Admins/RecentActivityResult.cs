namespace BlogApp.Domain.Models.Admins;

public record RecentActivityResult(
    string Type, // "blog" | "user" | "comment"
    string Message,
    string Actor,
    DateTime Date);