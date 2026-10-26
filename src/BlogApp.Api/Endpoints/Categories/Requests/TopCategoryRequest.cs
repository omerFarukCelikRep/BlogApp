using BlogApp.Application.Categories.Queries;

namespace BlogApp.Api.Endpoints.Categories.Requests;

public record TopCategoryRequest(int? Count)
{
    public static explicit operator GetTopCategoriesQuery(TopCategoryRequest request)
    {
        return request.Count.HasValue
            ? new GetTopCategoriesQuery(request.Count.Value)
            : new GetTopCategoriesQuery();
    }
}