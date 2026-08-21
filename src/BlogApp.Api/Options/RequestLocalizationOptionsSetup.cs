using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;

namespace BlogApp.Api.Options;

public class RequestLocalizationOptionsSetup(IOptions<CultureOptions> cultureOptions)
    : IConfigureOptions<RequestLocalizationOptions>
{
    public void Configure(RequestLocalizationOptions options)
    {
        var opts = cultureOptions.Value;

        var supportedCultures = opts.Supported.Select(x => new CultureInfo(x)).ToList();

        options.DefaultRequestCulture = new RequestCulture(opts.Default);
        options.SupportedCultures = supportedCultures;
        options.SupportedUICultures = supportedCultures;
    }
}