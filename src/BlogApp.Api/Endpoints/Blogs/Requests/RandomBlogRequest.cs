using BlogApp.Application.Blogs.Queries;

namespace BlogApp.Api.Endpoints.Blogs.Requests;

public record RandomBlogRequest(int? Count, List<string>? Categories)
{
    public static explicit operator GetRandomBlogsQuery(RandomBlogRequest request)
    {
        return request.Count.HasValue
            ? new GetRandomBlogsQuery(request.Categories, request.Count.Value)
            : new GetRandomBlogsQuery(request.Categories);
    }
}