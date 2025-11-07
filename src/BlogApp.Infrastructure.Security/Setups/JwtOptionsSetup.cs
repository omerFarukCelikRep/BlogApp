using BlogApp.Core.Security.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace BlogApp.Infrastructure.Security.Setups;

public sealed class JwtOptionsSetup(IConfiguration configuration)
    : IConfigureOptions<JwtOptions>
{
    public void Configure(JwtOptions options)
    {
        configuration.GetSection(JwtOptions.SectionName).Bind(options);
    }
}