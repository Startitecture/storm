// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumMapper.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Maps enumerations to string values.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Internal
{
    using System;
    using System.Collections.Generic;

    using SAF.Core;

    /// <summary>
    /// Maps enumerations to string values.
    /// </summary>
    internal static class EnumMapper
    {
        #region Static Fields

        /// <summary>
        /// The _types.
        /// </summary>
        private static readonly Cache<Type, Dictionary<string, object>> Types = new Cache<Type, Dictionary<string, object>>();

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
            Dictionary<string, object> map = Types.Get(
                enumType, 
                () =>
                    {
                        Array values = Enum.GetValues(enumType);

                        var newmap = new Dictionary<string, object>(values.Length, StringComparer.InvariantCultureIgnoreCase);

                        foreach (object v in values)
                        {
                            newmap.Add(v.ToString(), v);
                        }

                        return newmap;
                    });

            return map[value];
        }

        #endregion
    }
}