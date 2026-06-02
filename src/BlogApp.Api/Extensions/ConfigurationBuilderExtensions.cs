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
                .AddJsonFile("Settings/security.json", false, true);
            
            return builder;
        }
    }
}