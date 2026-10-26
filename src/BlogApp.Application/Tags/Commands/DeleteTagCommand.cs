using BlogApp.Domain.Models.Tags;

namespace BlogApp.Application.Tags.Commands;

public record DeleteTagCommand(int TagId) : DeleteTagArgs(TagId), IRequest<Result>;