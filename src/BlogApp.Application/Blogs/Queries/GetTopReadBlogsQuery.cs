using BlogApp.Domain.Models.Blogs;

namespace BlogApp.Application.Blogs.Queries;

public record GetTopReadBlogsQuery(int Count = 10) : TopReadBlogArgs(Count), IRequest<Result<List<BlogSummaryResult>>>;