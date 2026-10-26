using BlogApp.Core.Security.Attributes;
using BlogApp.Core.Security.Constants;
using BlogApp.Domain.Models.Blogs;

namespace BlogApp.Application.Blogs.Queries;

[Authorize(Permission.Blog.Edit)]
public record GetForUpdateQuery(int Id) : GetBlogByIdArgs(Id), IRequest<Result<BlogForUpdateResult>>;