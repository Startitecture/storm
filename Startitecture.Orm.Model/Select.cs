// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Select.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model
{
    using System;
    using System.Linq.Expressions;

    using JetBrains.Annotations;

    /// <summary>
    /// A static class for creating Transact-SQL queries.
    /// </summary>
#pragma warning disable CA1716 // Identifiers should not match keywords
    public static class Select
#pragma warning restore CA1716 // Identifiers should not match keywords
    {
        /// <summary>
        /// Creates an entity selection for the specified entity type.
        /// </summary>
        /// <param name="selections">
        /// The properties to return.
        /// </param>
        /// <typeparam name="T">
        /// The type of entity to query.
        /// </typeparam>
        /// <returns>
        /// A new <see cref="EntitySelection{T}"/> for the specified type.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="selections"/> is null.
        /// </exception>
        public static EntitySelection<T> From<T>([NotNull] params Expression<Func<T, object>>[] selections)
        {
            if (selections == null)
            {
                throw new ArgumentNullException(nameof(selections));
            }

            return new EntitySelection<T>().Select(selections);
        }

        /// <summary>
        /// Creates an entity set for the specified entity type.
        /// </summary>
        /// <typeparam name="T">
        /// The type of entity to query.
        /// </typeparam>
        /// <returns>
        /// A new <see cref="EntitySet{T}"/>for the specified type.
        /// </returns>
        public static EntitySet<T> Where<T>()
        {
            return new EntitySet<T>();
        }
    }
}