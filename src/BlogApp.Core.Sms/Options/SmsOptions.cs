namespace BlogApp.Core.Sms.Options;

public class SmsOptions
{
    public const string Section = "Sms";

    public string Provider   { get; set; } = "Twilio";
    public string AccountSid { get; set; } = string.Empty;
    public string AuthToken  { get; set; } = string.Empty;
    public string FromNumber { get; set; } = string.Empty;
}