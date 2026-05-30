using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace BlogApp.Domain.Options;

public sealed class KeyRotationOptionsSetup(IConfiguration configuration) : IConfigureOptions<KeyRotationOptions>
{
    public void Configure(KeyRotationOptions options)
    {
        configuration.GetSection(KeyRotationOptions.SectionName).Bind(options);
    }
}