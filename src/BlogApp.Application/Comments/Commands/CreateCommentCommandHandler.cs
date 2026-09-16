using BlogApp.Domain.Abstractions.Services;
using BlogApp.Domain.Models.Comments;

namespace BlogApp.Application.Comments.Commands;

public class CreateCommentCommandHandler(ICommentService commentService)
    : IRequestHandler<CreateCommentCommand, Result<CommentResult>>
{
    public async Task<Result<CommentResult>> Handle(CreateCommentCommand request,
        CancellationToken cancellationToken = default)
    {
        return await commentService.CreateAsync(request, cancellationToken);
    }
}