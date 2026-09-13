namespace BlogApp.Core.Search.Models;

public record SearchFilter(
    string Query,
    string Type = "all", // all | blogs | tags | authors
    string? Tag = null,
    string? Author = null,
    string? DateFrom = null,
    string? DateTo = null,
    string SortBy = "relevance", // relevance | latest | most-read
    int Page = 1,
    int PageSize = 10);