using BlogApp.Application.Blogs.Queries;

namespace BlogApp.Api.Endpoints.Blogs.Requests;

public record TopReadBlogRequest(int? Count)
{
    public static explicit operator GetTopReadBlogsQuery(TopReadBlogRequest request)
    {
        return request.Count.HasValue
            ? new(request.Count.Value)
            : new();
    }
}