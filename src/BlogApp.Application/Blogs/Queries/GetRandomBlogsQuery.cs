using BlogApp.Domain.Models.Blogs;

namespace BlogApp.Application.Blogs.Queries;

public record GetRandomBlogsQuery(List<string>? Categories, int Count = 6) : RandomBlogArgs(Count,Categories),IRequest<Result<List<BlogSummaryResult>>>;