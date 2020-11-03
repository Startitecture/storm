// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionMethods.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.SqlClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data;
    using System.Data.SqlTypes;
    using System.Reflection;

    /// <summary>
    /// Extension methods for the <see cref="SqlClient"/> namespace.
    /// </summary>
    internal static class ExtensionMethods
    {
        /// <summary>
        /// The decimal data type.
        /// </summary>
        private const string DecimalType = "DECIMAL";

        /// <summary>
        /// The numeric data type.
        /// </summary>
        private const string NumericType = "NUMERIC";

        /// <summary>
        /// The NVARCHAR data type.
        /// </summary>
        private const string NvarcharType = "NVARCHAR";

        /// <summary>
        /// The VARBINARY data type.
        /// </summary>
        private const string VarbinaryType = "VARBINARY";

        /// <summary>
        /// A type map between CLR types and SQL types.
        /// </summary>
        private static readonly Dictionary<Type, SqlDbType> TypeMap = new Dictionary<Type, SqlDbType>
                                                                      {
                                                                          {
                                                                              typeof(byte), SqlDbType.TinyInt
                                                                          },
                                                                          {
                                                                              typeof(sbyte), SqlDbType.TinyInt
                                                                          },
                                                                          {
                                                                              typeof(short), SqlDbType.SmallInt
                                                                          },
                                                                          {
                                                                              typeof(ushort), SqlDbType.SmallInt
                                                                          },
                                                                          {
                                                                              typeof(int), SqlDbType.Int
                                                                          },
                                                                          {
                                                                              typeof(uint), SqlDbType.Int
                                                                          },
                                                                          {
                                                                              typeof(long), SqlDbType.BigInt
                                                                          },
                                                                          {
                                                                              typeof(ulong), SqlDbType.BigInt
                                                                          },
                                                                          {
                                                                              typeof(float), SqlDbType.Real
                                                                          },
                                                                          {
                                                                              typeof(double), SqlDbType.Float
                                                                          },
                                                                          {
                                                                              typeof(decimal), SqlDbType.Decimal
                                                                          },
                                                                          {
                                                                              typeof(bool), SqlDbType.Bit
                                                                          },
                                                                          {
                                                                              typeof(string), SqlDbType.NVarChar
                                                                          },
                                                                          {
                                                                              typeof(char), SqlDbType.NChar
                                                                          },
                                                                          {
                                                                              typeof(Guid), SqlDbType.UniqueIdentifier
                                                                          },
                                                                          {
                                                                              typeof(DateTime), SqlDbType.DateTime
                                                                          },
                                                                          {
                                                                              typeof(DateTimeOffset), SqlDbType.DateTimeOffset
                                                                          },
                                                                          {
                                                                              typeof(byte[]), SqlDbType.VarBinary
                                                                          },
                                                                          {
                                                                              typeof(SqlMoney), SqlDbType.Money
                                                                          },
                                                                          {
                                                                              typeof(byte?), SqlDbType.TinyInt
                                                                          },
                                                                          {
                                                                              typeof(sbyte?), SqlDbType.TinyInt
                                                                          },
                                                                          {
                                                                              typeof(short?), SqlDbType.SmallInt
                                                                          },
                                                                          {
                                                                              typeof(ushort?), SqlDbType.SmallInt
                                                                          },
                                                                          {
                                                                              typeof(int?), SqlDbType.Int
                                                                          },
                                                                          {
                                                                              typeof(uint?), SqlDbType.Int
                                                                          },
                                                                          {
                                                                              typeof(long?), SqlDbType.BigInt
                                                                          },
                                                                          {
                                                                              typeof(ulong?), SqlDbType.BigInt
                                                                          },
                                                                          {
                                                                              typeof(float?), SqlDbType.Real
                                                                          },
                                                                          {
                                                                              typeof(double?), SqlDbType.Float
                                                                          },
                                                                          {
                                                                              typeof(decimal?), SqlDbType.Decimal
                                                                          },
                                                                          {
                                                                              typeof(bool?), SqlDbType.Bit
                                                                          },
                                                                          {
                                                                              typeof(char?), SqlDbType.NChar
                                                                          },
                                                                          {
                                                                              typeof(Guid?), SqlDbType.UniqueIdentifier
                                                                          },
                                                                          {
                                                                              typeof(DateTime?), SqlDbType.DateTime
                                                                          },
                                                                          {
                                                                              typeof(DateTimeOffset?), SqlDbType.DateTimeOffset
                                                                          },
                                                                          {
                                                                              typeof(SqlMoney?), SqlDbType.Money
                                                                          }
                                                                      };

        /// <summary>
        /// Gets the SQL type of the property using DataAnnotations.
        /// </summary>
        /// <param name="property">
        /// The property to translate.
        /// </param>
        /// <returns>
        /// The SQL type name of the property as a <see cref="string" />.
        /// </returns>
        internal static string GetSqlType(this PropertyInfo property)
        {
            var baseType = property.GetCustomAttribute<ColumnAttribute>()?.TypeName
                           ?? (TypeMap.TryGetValue(property.PropertyType, out var typeMap)
#pragma warning disable CA1308 // Normalize strings to uppercase - unnecessary because this is not a comparison
                                   ? typeMap.ToString().ToLowerInvariant()
#pragma warning restore CA1308 // Normalize strings to uppercase
                                   : property.PropertyType.Name);

            var length = property.GetCustomAttribute<MaxLengthAttribute>()?.Length;

            // Need to set NVARCHAR to something if we didn't get it from the column
            if (baseType.ToUpperInvariant() == NvarcharType && length.HasValue == false)
            {
                length = 4000;
            }

            // Need to set NVARCHAR to something if we didn't get it from the column
            if (baseType.ToUpperInvariant() == VarbinaryType && length.HasValue == false)
            {
                length = 8000;
            }

            // We won't get precision and scale with DataAnnotations, but decimal defaults to a precision of 18.
            if (baseType.ToUpperInvariant() == DecimalType || baseType == NumericType)
            {
                length = System.Data.SqlTypes.SqlDecimal.MaxPrecision;
            }

            return length.HasValue ? $"{baseType}({length})" : baseType;
        }
    }
}