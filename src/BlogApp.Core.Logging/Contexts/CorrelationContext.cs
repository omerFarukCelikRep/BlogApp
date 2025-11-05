namespace BlogApp.Core.Logging.Contexts;

public static class CorrelationContext
{
    private static readonly AsyncLocal<string?> _currentId = new();
    
    internal const string CorrelationPropertyName = "CorrelationId";
    
    public static string? CurrentId => _currentId?.Value;
    
    public static void Set(string id) => _currentId.Value = id;
    public static void Clear() => _currentId.Value = null;
}