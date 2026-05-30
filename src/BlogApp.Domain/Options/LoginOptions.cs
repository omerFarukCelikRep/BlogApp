namespace BlogApp.Domain.Options;

public class LoginOptions
{
    public const string SectionName = "Login";

    public int FailLimit { get; set; }
}