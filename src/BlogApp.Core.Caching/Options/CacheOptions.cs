using BlogApp.Core.Caching.Enums;

namespace BlogApp.Core.Caching.Options;

public class CacheOptions
{
    public const string SectionName = "Cache";

    public CacheProvider Provider { get; set; } = CacheProvider.Memory;
    public TimeSpan Expiration { get; set; } = TimeSpan.FromMinutes(5);
    public RedisCacheOptions? Redis  { get; set; }
}