namespace BlogApp.Domain.Models.Comments;

public record CreateCommentArgs(int BlogId, string Content, int? ParentId = null);