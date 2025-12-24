using Microsoft.Extensions.Options;

namespace BlogApp.Api.Options;

public sealed class CultureOptionsSetup(IConfiguration configuration) : IConfigureOptions<CultureOptions>
{
    public void Configure(CultureOptions options)
    {
        configuration.GetSection(CultureOptions.SectionName).Bind(options);
    }
}