using BlogApp.Core.Search.Abstractions;
using BlogApp.Core.Search.Models;

namespace BlogApp.Application.Search;

public class SearchQueryHandler(ISearchService searchService) : IRequestHandler<SearchQuery, Result<SearchResult>>
{
    public async Task<Result<SearchResult>> Handle(SearchQuery request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Q))
            return Result<SearchResult>.Success(data: new SearchResult(0, 0, 1, request.PageSize, string.Empty, "none",
                [], [], []));

        var result = await searchService.SearchAsync(request, cancellationToken);

        return Result<SearchResult>.Success(data: result);
    }
}