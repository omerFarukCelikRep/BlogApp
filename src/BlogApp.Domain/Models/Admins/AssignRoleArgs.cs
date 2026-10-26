using Role = BlogApp.Core.Security.Enums.Role;

namespace BlogApp.Domain.Models.Admins;

public record AssignRoleArgs(Guid UserId, Role Role, bool Assign);