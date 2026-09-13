namespace BlogApp.Core.Email.Options;

public class EmailOptions
{
    public const string Section = "Email";

    public string Host        { get; set; } = string.Empty;
    public int    Port        { get; set; } = 587;
    public bool   EnableSsl   { get; set; } = true;
    public string Username    { get; set; } = string.Empty;
    public string Password    { get; set; } = string.Empty;
    public string FromName    { get; set; } = "devlog";
    public string FromAddress { get; set; } = string.Empty;
    public string BaseUrl     { get; set; } = "https://localhost:5173";
}