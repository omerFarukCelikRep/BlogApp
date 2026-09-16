using BlogApp.Domain.Abstractions.Services;
using BlogApp.Domain.Models.Comments;

namespace BlogApp.Application.Comments.Queries;

public class GetBlogCommentsQueryHandler(ICommentService commentService)
    : IRequestHandler<GetBlogCommentsQuery, Result<List<CommentResult>>>
{
    public async Task<Result<List<CommentResult>>> Handle(GetBlogCommentsQuery request,
        CancellationToken cancellationToken = default)
    {
        return await commentService.GetByBlogAsync(request, cancellationToken);
    }
}