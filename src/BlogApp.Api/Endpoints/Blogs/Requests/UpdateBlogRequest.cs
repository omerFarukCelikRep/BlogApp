using BlogApp.Application.Blogs.Commands;
using BlogApp.Core.Security.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Endpoints.Blogs.Requests;

public record UpdateBlogRequest(
    [FromRoute(Name = "id")] int Id,
    [Sanitize] string Title,
    [Sanitize(AllowHtml = true)] string Content,
    string? Thumbnail,
    List<int> Categories,
    List<int> Tags)
{
    public static explicit operator UpdateBlogCommand(UpdateBlogRequest request) =>
        new(request.Id, request.Title, request.Content, request.Thumbnail, request.Categories, request.Tags);
}