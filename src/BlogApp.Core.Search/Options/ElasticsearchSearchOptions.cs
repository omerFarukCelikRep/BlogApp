namespace BlogApp.Core.Search.Options;

public class ElasticsearchSearchOptions
{
    public string Uri       { get; set; } = "http://localhost:9200";
    public string IndexName { get; set; } = "blogapp";
    public string Username  { get; set; } = string.Empty;
    public string Password  { get; set; } = string.Empty;
}