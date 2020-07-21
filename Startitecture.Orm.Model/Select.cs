﻿// --------------------------------------------------------------------------------------------------------------------
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
        /// Creates a query from the specified item type.
        /// </summary>
        /// <param name="selections">
        /// The properties to return.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item to query.
        /// </typeparam>
        /// <returns>
        /// A new <see cref="EntitySelection{T}"/> for the specified type.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="selections"/> is null.
        /// </exception>
        public static EntitySelection<TItem> From<TItem>([NotNull] params Expression<Func<TItem, object>>[] selections)
        {
            if (selections == null)
            {
                throw new ArgumentNullException(nameof(selections));
            }

            return new EntitySelection<TItem>().Select(selections);
        }
    }
}