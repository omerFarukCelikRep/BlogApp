using BlogApp.Application.Comments.Commands;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Endpoints.Comments.Requests;

public record DeleteCommentRequest([FromRoute(Name = "id")] int Id, [FromRoute(Name = "blogId")] int BlogId)
{
    public static explicit operator DeleteCommentCommand(DeleteCommentRequest request) =>
        new(request.Id, request.BlogId);
}