using BlogApp.Application.Tags.Commands;

namespace BlogApp.Api.Endpoints.Tags.Requests;

public record CreateTagRequest(string Name)
{
    public static explicit operator CreateTagCommand(CreateTagRequest request) => new(request.Name);
}