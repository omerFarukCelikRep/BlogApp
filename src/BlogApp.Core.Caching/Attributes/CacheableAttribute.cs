namespace BlogApp.Core.Caching.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class CacheableAttribute(string? key = null, int durationSeconds = 60) : Attribute
{
    public string? Key { get; } = key;
    public int DurationSeconds { get; } = durationSeconds;
}