using BlogApp.Domain.Models.Tags;

namespace BlogApp.Application.Tags.Queries;

public record GetTopTagsQuery(int Count = 10) : TopTagArgs(Count), IRequest<Result<List<TagResult>>>;