// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumMapper.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   Maps enumerations to string values.
// </summary>

namespace Startitecture.Orm.Mapper
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;

    using JetBrains.Annotations;

    /// <summary>
    /// Maps enumerations to string values.
    /// </summary>
    internal static class EnumMapper
    {
        /// <summary>
        /// The types.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, Lazy<Dictionary<string, object>>> TypesCache =
            new ConcurrentDictionary<Type, Lazy<Dictionary<string, object>>>();

        /// <summary>
        /// Maps an enumeration from a string.
        /// </summary>
        /// <param name="enumType">
        /// The enumeration type.
        /// </param>
        /// <param name="value">
        /// The string value.
        /// </param>
        /// <returns>
        /// An object representing the enumeration specified by <paramref name="value"/>.
        /// </returns>
        public static object EnumFromString([NotNull] Type enumType, string value)
        {
            if (enumType == null)
            {
                throw new ArgumentNullException(nameof(enumType));
            }

            var lazyMap = new Lazy<Dictionary<string, object>>(() => CreateDictionary(enumType), LazyThreadSafetyMode.ExecutionAndPublication);
            var map = TypesCache.GetOrAdd(enumType, key => lazyMap).Value;

            ////var map = Types.Get(
            ////    enumType,
            ////    () => { return CreateDictionary(enumType); });

            return map[value];
        }

        private static Dictionary<string, object> CreateDictionary(Type enumType)
        {
            var values = Enum.GetValues(enumType);
            var dictionary = new Dictionary<string, object>(values.Length, StringComparer.InvariantCultureIgnoreCase);

            foreach (var value in values)
            {
                dictionary.Add(value.ToString(), value);
            }

            return dictionary;
        }
    }
}