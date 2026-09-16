using BlogApp.Core.Results;
using BlogApp.Domain.Models.Comments;

namespace BlogApp.Domain.Abstractions.Services;

public interface ICommentService
{
    Task<Result<List<CommentResult>>> GetByBlogAsync(GetBlogCommentsArgs args,
        CancellationToken cancellationToken = default);

    Task<Result<CommentResult>> CreateAsync(CreateCommentArgs args, CancellationToken cancellationToken = default);