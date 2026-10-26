namespace BlogApp.Domain.Models.Admins;

public record BanUserArgs(Guid UserId, bool Ban);