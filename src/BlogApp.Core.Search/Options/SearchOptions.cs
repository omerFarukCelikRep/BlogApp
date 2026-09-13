namespace BlogApp.Core.Search.Options;

public class SearchOptions
{
    public const string Section = "Search";

    public string   Provider        { get; set; } = "Postgres"; // Postgres | Elasticsearch
    public int      MinQueryLength  { get; set; } = 2;
    public int      MaxResults      { get; set; } = 50;
    public bool     HighlightEnabled { get; set; } = true;

    public ElasticsearchSearchOptions Elasticsearch { get; set; } = new();
}