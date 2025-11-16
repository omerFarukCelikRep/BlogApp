using BlogApp.Domain.Options;
using Microsoft.Extensions.Options;

namespace BlogApp.Api.Options;

public sealed class KeyRotationOptionsSetup(IConfiguration configuration) : IConfigureOptions<KeyRotationOptions>
{
    public void Configure(KeyRotationOptions options)
    {
        configuration.GetSection(KeyRotationOptions.SectionName).Bind(options);
    }
}