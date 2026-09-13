using BlogApp.Core.Security.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace BlogApp.Infrastructure.Security.Options;

public class XssOptionsSetup(IConfiguration configuration) : IConfigureOptions<XssOptions>
{
    public void Configure(XssOptions options)
    {
        configuration.GetSection(XssOptions.Section).Bind(options);
    }
}