using Microsoft.AspNetCore.Builder;
using Serilog;

namespace BlogApp.Core.Logging.Extensions;

public static class SerilogMiddlewareExtensions
{
    public static IApplicationBuilder UseSerilog(this IApplicationBuilder app)
    {
        app.UseSerilogRequestLogging();

        return app;
    }
}