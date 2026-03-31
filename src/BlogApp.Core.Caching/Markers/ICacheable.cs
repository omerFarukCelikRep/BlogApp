namespace BlogApp.Core.Caching.Markers;

public interface ICacheable
{
    string Key { get; }
    TimeSpan? Expiry { get; }
}