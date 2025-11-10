using System.Net.Mime;
using System.Reflection;
using System.Text.Json;
using BlogApp.Core.Results;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.Extensions.Options;

namespace BlogApp.Api.Endpoints.Shared.Responses;

public sealed class Response<TResult> : IResult, IEndpointMetadataProvider, IStatusCodeHttpResult, IValueHttpResult,
    IValueHttpResult<TResult>
{
    internal Response(Result<TResult> result)
    {
        Value = result.Data;
        StatusCode = result.StatusCode;
    }

    public TResult? Value { get; }

    object? IValueHttpResult.Value => Value;
    public int StatusCode { get; }

    int? IStatusCodeHttpResult.StatusCode => StatusCode;

    private static JsonSerializerOptions ResolveJsonOptions(HttpContext httpContext)
    {
        return httpContext.RequestServices.GetService<IOptions<JsonSerializerOptions>>()?.Value ??
               new JsonSerializerOptions();
    }

    public Task ExecuteAsync(HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        var logger = httpContext.RequestServices.GetRequiredService<ILogger<Response<TResult>>>();
        logger.Log(LogLevel.Information, message: "Setting Http status code {StatusCode}", StatusCode);

        httpContext.Response.StatusCode = StatusCode;

        var jsonSerializerOptions = ResolveJsonOptions(httpContext);

        httpContext.Response.ContentType = $"{MediaTypeNames.Application.Json}; charset=utf-8";
        return httpContext.Response.WriteAsJsonAsync(Value, jsonSerializerOptions);
    }

    public static void PopulateMetadata(MethodInfo method, EndpointBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(method);
        ArgumentNullException.ThrowIfNull(builder);

        builder.Metadata.Add(new ProducesResponseTypeMetadata(StatusCodes.Status200OK, typeof(TResult),
            [MediaTypeNames.Application.Json]));
    }
}