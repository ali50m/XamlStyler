// (c) Xavalon. All rights reserved.
#if NET10_0_OR_GREATER
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Xavalon.XamlStyler.Options
{
    internal sealed class ConfigurationBooleanConverter : JsonConverter<bool>
    {
        internal static bool ReadValue(ref Utf8JsonReader reader)
        {
            return reader.TokenType switch
            {
                JsonTokenType.True => true,
                JsonTokenType.False => false,
                JsonTokenType.String => bool.Parse(reader.GetString()),
                JsonTokenType.Number => reader.GetDouble() != 0,
                _ => throw new JsonException("Invalid Boolean setting.")
            };
        }
        public override bool Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options) => ReadValue(ref reader);
        public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options) => writer.WriteBooleanValue(value);
    }

    internal sealed class ConfigurationNullableBooleanConverter : JsonConverter<bool?>
    {
        public override bool? Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options) =>
            reader.TokenType == JsonTokenType.Null ? null : ConfigurationBooleanConverter.ReadValue(ref reader);
        public override void Write(Utf8JsonWriter writer, bool? value, JsonSerializerOptions options)
        {
            if (value.HasValue) writer.WriteBooleanValue(value.Value);
            else writer.WriteNullValue();
        }
    }
}
#endif
