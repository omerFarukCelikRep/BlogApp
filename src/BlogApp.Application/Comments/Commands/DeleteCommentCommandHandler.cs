using BlogApp.Domain.Abstractions.Services;

namespace BlogApp.Application.Comments.Commands;

public class DeleteCommentCommandHandler(ICommentService commentService) : IRequestHandler<DeleteCommentCommand, Result>
{
    public async Task<Result> Handle(DeleteCommentCommand request, CancellationToken cancellationToken = default)
    {
        return await commentService.DeleteAsync(request, cancellationToken);
    }
}