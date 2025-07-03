using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BuildingService.Converters;

public class CustomUtcDateTimeConverter : JsonConverter<DateTime>
{
    private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            string? dateString = reader.GetString();
            if (DateTime.TryParseExact(dateString, DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
            {
                return DateTime.SpecifyKind(result, DateTimeKind.Utc);
            }
            throw new JsonException($"Unable to convert \"{dateString}\" to DateTime. Expected format: {DateTimeFormat}");
        }
        throw new JsonException($"Unexpected JSON token type for DateTime. Expected String, Got {reader.TokenType}");
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToUniversalTime().ToString(DateTimeFormat, CultureInfo.InvariantCulture));
    }
}