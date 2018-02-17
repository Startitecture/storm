// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Query.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers
{
    using System;
    using System.Linq.Expressions;

    using JetBrains.Annotations;

    using Startitecture.Orm.Common;
    using Startitecture.Orm.Sql;

    /// <summary>
    /// A static class for creating Transact-SQL queries.
    /// </summary>
    public static class Query
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
        /// A new <see cref="SqlSelection{TItem}"/> for the specified type.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="selections"/> is null.
        /// </exception>
        public static SqlSelection<TItem> From<TItem>([NotNull] params Expression<Func<TItem, object>>[] selections)
            where TItem : ITransactionContext, new()
        {
            if (selections == null)
            {
                throw new ArgumentNullException(nameof(selections));
            }

            return new TItem().ToExampleSelection(selections);
        }
    }
}