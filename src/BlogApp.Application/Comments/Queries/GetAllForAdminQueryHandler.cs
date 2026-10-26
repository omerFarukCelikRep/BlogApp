using BlogApp.Domain.Abstractions.Services;
using BlogApp.Domain.Models.Comments;

namespace BlogApp.Application.Comments.Queries;

public class GetAllForAdminQueryHandler(ICommentService commentService):IRequestHandler<GetAllForAdminQuery, PaginatedResult<CommentResult>>
{
    public async Task<PaginatedResult<CommentResult>> Handle(GetAllForAdminQuery request, CancellationToken cancellationToken = default)
    {
        return await commentService.GetAllForAdminAsync(request, cancellationToken);
    }
}