namespace BlogApp.Domain.Models.Comments;

public record GetAllCommentsForAdminArgs(int? Page = 1, int? PageSize = 20);