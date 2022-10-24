using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Vestaboard.Common;

public sealed class NumericEnumJsonConverter<T> : JsonConverter<T>
    where T : Enum
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => Unsafe.As<int, T>(ref Unsafe.AsRef(reader.GetInt32()));

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options) => writer.WriteNumberValue(Unsafe.As<T, int>(ref value));
}