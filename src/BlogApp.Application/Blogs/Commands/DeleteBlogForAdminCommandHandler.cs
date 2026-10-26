using BlogApp.Core.Search.Abstractions;
using BlogApp.Domain.Abstractions.Services;

namespace BlogApp.Application.Blogs.Commands;

public class DeleteBlogForAdminCommandHandler(IBlogService blogService, ISearchService searchService)
    : IRequestHandler<DeleteBlogForAdminCommand, Result>
{
    public async Task<Result> Handle(DeleteBlogForAdminCommand request, CancellationToken cancellationToken = default)
    {
        var result = await blogService.DeleteForAdminAsync(request, cancellationToken);

        await searchService.RemoveBlogFromIndexAsync(request.BlogId, cancellationToken);

        return result;
    }
}