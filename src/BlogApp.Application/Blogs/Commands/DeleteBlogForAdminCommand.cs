using BlogApp.Core.Security.Attributes;
using BlogApp.Core.Security.Enums;
using BlogApp.Domain.Models.Blogs;

namespace BlogApp.Application.Blogs.Commands;

[Authorize(Role.Admin)]
public record DeleteBlogForAdminCommand(int BlogId) : DeleteBlogForAdminArgs(BlogId), IRequest<Result>;