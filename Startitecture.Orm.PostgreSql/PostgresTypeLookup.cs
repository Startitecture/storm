// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostgresTypeLookup.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.PostgreSql
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.NetworkInformation;

    using JetBrains.Annotations;

    using NpgsqlTypes;

    /// <summary>
    /// The postgres type lookup.
    /// </summary>
    internal class PostgresTypeLookup
    {
        /// <summary>
        /// The type dictionary.
        /// </summary>
        private static readonly Dictionary<Type, string> TypeDictionary = new Dictionary<Type, string>
                                                                              {
                                                                                  { typeof(bool), "boolean" },
                                                                                  { typeof(byte), "smallint" },
                                                                                  { typeof(sbyte), "smallint" },
                                                                                  { typeof(short), "smallint" },
                                                                                  { typeof(int), "integer" },
                                                                                  { typeof(long), "bigint" },
                                                                                  { typeof(float), "real" },
                                                                                  { typeof(double), "double precision" },
                                                                                  { typeof(decimal), "numeric" },
                                                                                  { typeof(string), "text" },
                                                                                  { typeof(char[]), "text" },
                                                                                  { typeof(char), "text" },
                                                                                  { typeof(Guid), "uuid" },
                                                                                  { typeof(DateTime), "timestamp without time zone" },
                                                                                  { typeof(DateTimeOffset), "timestamp with time zone" },
                                                                                  { typeof(TimeSpan), "interval" },
                                                                                  { typeof(byte[]), "bytea" },
                                                                                  { typeof(NpgsqlPoint), "point" },
                                                                                  { typeof(NpgsqlLSeg), "lseg" },
                                                                                  { typeof(NpgsqlPath), "path" },
                                                                                  { typeof(NpgsqlPolygon), "polygon" },
                                                                                  { typeof(NpgsqlLine), "line" },
                                                                                  { typeof(NpgsqlCircle), "circle" },
                                                                                  { typeof(NpgsqlBox), "box" },
                                                                                  { typeof(BitArray), "bit varying" },
                                                                                  { typeof(IPAddress), "inet" },
                                                                                  { typeof(ValueTuple<IPAddress, int>), "inet" },
                                                                                  { typeof(PhysicalAddress), "macaddr" },
                                                                                  { typeof(NpgsqlTsQuery), "tsquery" },
                                                                                  { typeof(NpgsqlTsVector), "tsvector" },
                                                                                  { typeof(NpgsqlDate), "date" },
                                                                                  { typeof(NpgsqlDateTime), "timestamp without time zone" }
                                                                              };

        /// <summary>
        /// Gets a value indicating whether the specified <paramref name="clrType"/> is supported for implicit conversion.
        /// </summary>
        /// <param name="clrType">
        /// The CLR type to evaluate.
        /// </param>
        /// <returns>
        /// <c>true</c> if the CLR type is supported; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="clrType"/> is null.
        /// </exception>
        public static bool IsSupported([NotNull] Type clrType)
        {
            if (clrType == null)
            {
                throw new ArgumentNullException(nameof(clrType));
            }

            return TypeDictionary.ContainsKey(clrType);
        }

        /// <summary>
        /// Gets the PostgreSQL data type for the specified <paramref name="clrType"/>.
        /// </summary>
        /// <param name="clrType">
        /// The CLR type to convert to a PostgreSQL data type.
        /// </param>
        /// <returns>
        /// The PostgreSQL data type as a <see cref="string"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="clrType"/> is null.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// <paramref name="clrType"/> is not one of the supported types for conversion.
        /// </exception>
        public static string GetType([NotNull] Type clrType)
        {
            if (clrType == null)
            {
                throw new ArgumentNullException(nameof(clrType));
            }

            var baseType = Nullable.GetUnderlyingType(clrType) ?? clrType;

            if (IsSupported(baseType))
            {
                return TypeDictionary[baseType];
            }

            throw new NotSupportedException($"The type '{baseType}' is not supported for implicit conversion.");
        }
    }
}