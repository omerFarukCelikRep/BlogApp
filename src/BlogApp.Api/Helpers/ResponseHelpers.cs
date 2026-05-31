using System.Text.Json;
using Microsoft.Extensions.Options;

namespace BlogApp.Api.Helpers;

internal static class ResponseHelpers
{
    internal static JsonSerializerOptions ResolveJsonSerializerOptions(HttpContext httpContext) =>
        httpContext.RequestServices.GetService<IOptions<JsonSerializerOptions>>()?.Value ?? new JsonSerializerOptions();
}