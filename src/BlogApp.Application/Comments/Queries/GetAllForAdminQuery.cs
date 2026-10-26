using BlogApp.Domain.Models.Comments;

namespace BlogApp.Application.Comments.Queries;

public record GetAllForAdminQuery(int? Page, int? PageSize)
    : GetAllCommentsForAdminArgs(Page, PageSize), IRequest<PaginatedResult<CommentResult>>;