// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionMethods.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   Defines the ExtensionMethods type.
// </summary>

namespace Startitecture.Orm.Schema
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// The extension methods.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Gets a collection of properties for the specified type.
        /// </summary>
        /// <param name="entityType">
        /// The entity type.
        /// </param>
        /// <param name="propertyNames">
        /// The names of the properties to collect. If no names are specified, all the entity's properties are returned.
        /// </param>
        /// <returns>
        /// A collection of non-indexed properties for the specified type.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// None of the specified property names (case-sensitive) matched properties of the entity type.
        /// </exception>
        public static IEnumerable<PropertyInfo> GetNonIndexedProperties(this Type entityType, params string[] propertyNames)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException(nameof(entityType));
            }

            var nonIndexedProperties = entityType.GetProperties().Where(prop => !prop.GetIndexParameters().Any());

            return propertyNames.Any() ? nonIndexedProperties.Where(x => propertyNames.Contains(x.Name)) : nonIndexedProperties;
        }
    }
}
