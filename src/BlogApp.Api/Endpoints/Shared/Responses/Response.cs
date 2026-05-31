using System.Net.Mime;
using System.Reflection;
using System.Text.Json;
using BlogApp.Api.Helpers;
using BlogApp.Core.Results;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.Extensions.Options;

namespace BlogApp.Api.Endpoints.Shared.Responses;

public sealed class Response : IResult, IEndpointMetadataProvider, IStatusCodeHttpResult, IValueHttpResult,
    IValueHttpResult<Result>
{
    internal Response(Result result)
    {
        Value = result;
        StatusCode = result.StatusCode;
    }

    public Result? Value { get; }

    object? IValueHttpResult.Value => Value;
    public int StatusCode { get; }

    int? IStatusCodeHttpResult.StatusCode => StatusCode;
    
    public Task ExecuteAsync(HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        var logger = httpContext.RequestServices.GetRequiredService<ILogger<Response>>();
        var logLevel = StatusCode >= 500 ? LogLevel.Error
            : StatusCode >= 400 ? LogLevel.Warning
            : LogLevel.Information;

        logger.Log(logLevel, "Setting Http status code {StatusCode}", StatusCode);

        httpContext.Response.StatusCode = StatusCode;

        var jsonSerializerOptions = ResponseHelpers.ResolveJsonSerializerOptions(httpContext);

        httpContext.Response.ContentType = $"{MediaTypeNames.Application.Json}; charset=utf-8";
        return httpContext.Response.WriteAsJsonAsync(Value, jsonSerializerOptions);
    }

    public static void PopulateMetadata(MethodInfo method, EndpointBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(method);
        ArgumentNullException.ThrowIfNull(builder);

        builder.Metadata.Add(new ProducesResponseTypeMetadata(
            StatusCodes.Status200OK, typeof(Result), [MediaTypeNames.Application.Json]));
        builder.Metadata.Add(new ProducesResponseTypeMetadata(
            StatusCodes.Status400BadRequest, typeof(Result), [MediaTypeNames.Application.Json]));
        builder.Metadata.Add(new ProducesResponseTypeMetadata(
            StatusCodes.Status401Unauthorized, typeof(Result), [MediaTypeNames.Application.Json]));
        builder.Metadata.Add(new ProducesResponseTypeMetadata(
            StatusCodes.Status500InternalServerError, typeof(Result), [MediaTypeNames.Application.Json]));
    }
}