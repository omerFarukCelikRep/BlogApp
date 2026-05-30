using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace BlogApp.Domain.Options;

public sealed class LoginOptionsSetup(IConfiguration configuration) : IConfigureOptions<LoginOptions>
{
    public void Configure(LoginOptions options)
    {
        configuration.GetSection(LoginOptions.SectionName).Bind(options);
    }
}