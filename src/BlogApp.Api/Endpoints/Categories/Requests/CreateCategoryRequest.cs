using BlogApp.Application.Categories.Commands;

namespace BlogApp.Api.Endpoints.Categories.Requests;

public record CreateCategoryRequest(string Name, int? ParentId)
{
    public static explicit operator CreateCategoryCommand(CreateCategoryRequest request) =>
        new(request.Name, request.ParentId);
}