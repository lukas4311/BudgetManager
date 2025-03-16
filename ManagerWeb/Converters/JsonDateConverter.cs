using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BudgetManager.ManagerWeb.Converters
{
    /// <summary>
    /// Converts <see cref="DateTime"/> to and from JSON using the "yyyy-MM-dd" format.
    /// </summary>
    internal class JsonDateConverter : JsonConverter<DateTime>
    {
        /// <summary>
        /// Reads and converts the JSON to <see cref="DateTime"/> using the "yyyy-MM-dd" format.
        /// </summary>
        /// <param name="reader">The <see cref="Utf8JsonReader"/> to read from.</param>
        /// <param name="typeToConvert">The type to convert.</param>
        /// <param name="options">Options to control the conversion behavior.</param>
        /// <returns>The converted <see cref="DateTime"/>.</returns>
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => DateTime.ParseExact(reader.GetString(), "yyyy-MM-dd", CultureInfo.InvariantCulture);

        /// <summary>
        /// Writes a <see cref="DateTime"/> to JSON using the "yyyy-MM-dd" format.
        /// </summary>
        /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write to.</param>
        /// <param name="value">The <see cref="DateTime"/> value to convert.</param>
        /// <param name="options">Options to control the conversion behavior.</param>
        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
    }
}
