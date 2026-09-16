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

    public async Task<Result<CommentResult>> CreateAsync(CreateCommentArgs args,
        CancellationToken cancellationToken = default)
    {
        var blog = await blogRepository.GetByIdAsync(args.BlogId, false, cancellationToken);
        if (blog is null)
            return Result<CommentResult>.Failed(404, Error.Create(Errors.Blog.NotFound));

        if (!blog.IsPublished())
            return Result<CommentResult>.Failed(400, Error.Create(Errors.Blog.NotPublished));

        if (args.ParentId.HasValue)
        {
            var parent = await commentRepository.GetByIdAsync(args.ParentId.Value, false, cancellationToken);
            if (parent is null)
                return Result<CommentResult>.Failed(404, Error.Create(Errors.Comment.NotFound));

            if (parent.ParentId.HasValue)
                return Result<CommentResult>.Failed(400, Error.Create(Errors.Comment.NestedReplyNotAllowed));
        }

        var comment = new Comment
        {
            BlogId = blog.Id,
            Content = args.Content,
            UserId = domainPrincipal.UserId,
            ParentId = args.ParentId
        };

        await commentRepository.AddAsync(comment, cancellationToken);
        await commentRepository.SaveChangesAsync(cancellationToken);

        telemetryService.RecordBlogComment(args.BlogId);

        return Result<CommentResult>.Success(data: new(
            Id: comment.Id,
            Content: comment.Content,
            IsEdited: false,
            CreatedDate: comment.CreatedDate.DateTime,
            Author: new(
                Id: domainPrincipal.UserId,
                FullName: domainPrincipal.FullName,
                Username: domainPrincipal.Username!,
                ProfilePicture: null),
            ParentId: comment.ParentId,
            Replies: []
        ));
    }

    public async Task<Result> DeleteAsync(DeleteCommentArgs args, CancellationToken cancellationToken = default)
    {
        var comment = await commentRepository.GetByIdAsync(args.Id, cancellationToken: cancellationToken);
        if (comment is null)
            return Result.Failed(404, Error.Create(Errors.Comment.NotFound));

        if (domainPrincipal.Roles.All(x => x != Role.Admin))
            if (comment.UserId != domainPrincipal.UserId && !domainPrincipal.HasPermission(Permission.Comment.Moderate))
                return Result.Failed(403, Error.Create(Errors.Comment.NotAuthor));


        await commentRepository.DeleteAsync(comment, cancellationToken);
        await commentRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}