namespace BlogApp.Domain.Options;

public class KeyRotationOptions
{
    public const string SectionName = "KeyRotation";

    public TimeSpan Period { get; set; }
}