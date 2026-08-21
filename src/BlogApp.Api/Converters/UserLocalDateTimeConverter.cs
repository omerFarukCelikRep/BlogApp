using System.Text.Json;
using System.Text.Json.Serialization;
using BlogApp.Api.Extensions;

namespace BlogApp.Api.Converters;

public class UserLocalDateTimeConverter(IHttpContextAccessor httpContextAccessor) : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var localDateTime = reader.GetDateTime();

        var timeZone = httpContextAccessor.HttpContext?.GetUserTimeZone();
        return timeZone is null
            ? DateTime.SpecifyKind(localDateTime, DateTimeKind.Utc)
            : TimeZoneInfo.ConvertTimeToUtc(localDateTime, timeZone);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        var timeZone = httpContextAccessor.HttpContext?.GetUserTimeZone();

        var localTime = value;
        if (timeZone is not null)
            localTime = TimeZoneInfo.ConvertTimeFromUtc(value, timeZone);

        writer.WriteStringValue(localTime);
    }
}