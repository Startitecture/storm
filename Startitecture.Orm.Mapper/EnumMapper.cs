// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumMapper.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Maps enumerations to string values.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Mapper
{
    using System;
    using System.Collections.Generic;

    using Startitecture.Core;

    /// <summary>
    /// Maps enumerations to string values.
    /// </summary>
    internal static class EnumMapper
    {
        #region Static Fields

        /// <summary>
        /// The _types.
        /// </summary>
        private static readonly MemoryCache<Type, Dictionary<string, object>> Types = new MemoryCache<Type, Dictionary<string, object>>();

        #endregion

        #region Public Methods and Operators

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
        public static object EnumFromString(Type enumType, string value)
        {
            var map = Types.Get(
                enumType, 
                () =>
                    {
                        var values = Enum.GetValues(enumType);

                        var dictionary = new Dictionary<string, object>(values.Length, StringComparer.InvariantCultureIgnoreCase);

                        foreach (var v in values)
                        {
                            dictionary.Add(v.ToString(), v);
                        }

                        return dictionary;
                    });

            return map[value];
        }

        #endregion
    }
}