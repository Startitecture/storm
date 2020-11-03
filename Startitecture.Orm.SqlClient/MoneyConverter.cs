// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MoneyConverter.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.SqlClient
{
    using System;
    using System.Data.SqlTypes;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    using JetBrains.Annotations;

    /// <summary>
    /// Converts the <see cref="SqlMoney"/> type for use in JSON data.
    /// </summary>
    public class MoneyConverter : JsonConverter<SqlMoney?>
    {
        /// <inheritdoc />
        public override SqlMoney? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return decimal.TryParse(reader.GetString(), out decimal value)
                       ? new SqlMoney(value) as SqlMoney?
                       : null;
        }

        /// <inheritdoc />
        public override void Write([NotNull] Utf8JsonWriter writer, SqlMoney? value, JsonSerializerOptions options)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            writer.WriteStringValue(value.HasValue ? value.Value.ToString() : "null");
        }
    }
}