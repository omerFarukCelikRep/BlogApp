using BlogApp.Application.Tags.Queries;

namespace BlogApp.Api.Endpoints.Tags.Requests;

public record TopTagsRequest(int? Count)
{
    public static explicit operator GetTopTagsQuery(TopTagsRequest request)
    {
        return request.Count.HasValue
            ? new GetTopTagsQuery(request.Count.Value)
            : new GetTopTagsQuery();
    }
}