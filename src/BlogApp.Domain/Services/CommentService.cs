using BlogApp.Core.Results;
using BlogApp.Core.Security.Abstractions;
using BlogApp.Core.Telemetry.Abstractions;
using BlogApp.Domain.Abstractions.Repositories;
using BlogApp.Domain.Abstractions.Services;
using BlogApp.Domain.Constants;
using BlogApp.Domain.Models.Comments;
using Permission = BlogApp.Core.Security.Constants.Permission;
using Role = BlogApp.Core.Security.Enums.Role;

namespace BlogApp.Domain.Services;

public class CommentService(
    ICommentRepository commentRepository,
    IBlogRepository blogRepository,
    ITelemetryService telemetryService,
    IDomainPrincipal domainPrincipal) : ICommentService
{
    private static CommentResult BuildTree(Comment comment, List<Comment> all)
    {
        var replies = all.Where(c => c.ParentId == comment.Id)
            .Select(c => BuildTree(c, all))
            .ToList();

        return new(
            Id: comment.Id,
            Content: comment.Content,
            IsEdited: comment.ModifiedDate.HasValue,
            CreatedDate: comment.CreatedDate.DateTime,
            Author: new(
                Id: comment.User!.Id,
                FullName: comment.User.FullName,
                Username: comment.User.Username,
                ProfilePicture: comment.User.ProfilePicture),
            ParentId: comment.ParentId,
            Replies: replies);
    }

    public async Task<Result<List<CommentResult>>> GetByBlogAsync(GetBlogCommentsArgs args,
        CancellationToken cancellationToken = default)
    {
        var comments = await commentRepository.GetBlogCommentsAsync(args.BlogId, cancellationToken);

        var topLevel = comments.Where(c => c.ParentId is null)
            .Select(c => BuildTree(c, comments))
            .ToList();

        return Result<List<CommentResult>>.Success(data: topLevel);
    }
}