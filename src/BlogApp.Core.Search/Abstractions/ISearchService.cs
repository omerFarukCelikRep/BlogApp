using BlogApp.Core.Search.Models;

namespace BlogApp.Core.Search.Abstractions;

public interface ISearchService
{
    Task<SearchResult> SearchAsync(SearchFilter filter, CancellationToken cancellationToken = default);

    Task IndexBlogAsync(int blogId, CancellationToken cancellationToken = default);

    Task RemoveBlogFromIndexAsync(int blogId, CancellationToken cancellationToken = default);
}