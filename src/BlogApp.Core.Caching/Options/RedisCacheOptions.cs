namespace BlogApp.Core.Caching.Options;

public class RedisCacheOptions
{
    public string ConnectionString { get; set; } = "localhost:6379";
    public string? Password { get; set; }
    public int Database { get; set; }
    public string InstanceName { get; set; } = "BlogApp";
}