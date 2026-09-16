namespace BlogApp.Api.Extensions;

public static class ConfigurationBuilderExtensions
{
    extension(IConfigurationBuilder builder)
    {
        public IConfigurationBuilder AddSettingFiles()
        {
            builder.AddJsonFile("Settings/caching.json", false, true)
                .AddJsonFile("Settings/culture.json", false, true)
                .AddJsonFile("Settings/database.json", false, true)
                .AddJsonFile("Settings/logging.json", false, true)
                .AddJsonFile("Settings/security.json", false, true)
                .AddJsonFile("Settings/telemetry.json", false, true)
                .AddJsonFile("Settings/search.json", false, true)
                .AddJsonFile("Settings/email.json", false, true)
                .AddJsonFile("Settings/sms.json", false, true);

            return builder;
        }
    }
}