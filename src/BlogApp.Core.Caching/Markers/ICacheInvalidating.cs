namespace BlogApp.Core.Caching.Markers;

public interface ICacheInvalidating
{
    List<string> InvalidationKeys  { get; }
}