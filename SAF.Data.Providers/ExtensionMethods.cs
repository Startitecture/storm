// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionMethods.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Contains extension methods for the common repository provider library.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers
{
    using System;
    using System.Linq.Expressions;

    using JetBrains.Annotations;

    using Startitecture.Orm.Common;
    using Startitecture.Orm.Query;
    using Startitecture.Orm.Sql;

    /// <summary>
    /// Contains extension methods for the common repository provider library.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Selects items based on the data item type of the current repository.
        /// </summary>
        /// <param name="repository">
        /// The current repository.
        /// </param>
        /// <param name="selectExpressions">
        /// The column selection expressions.
        /// </param>
        /// <typeparam name="TEntity">
        /// The type of entity stored in the repository.
        /// </typeparam>
        /// <typeparam name="TDataItem">
        /// The type of data item that represents the entity.
        /// </typeparam>
        /// <returns>
        /// An <see cref="Startitecture.Orm.Query.ItemSelection{TItem}"/> for the specified <typeparamref name="TDataItem"/>
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="repository"/> or <paramref name="selectExpressions"/> is null.
        /// </exception>
        public static ItemSelection<TDataItem> Select<TEntity, TDataItem>(
            [NotNull] this EntityRepository<TEntity, TDataItem> repository,
            [NotNull] params Expression<Func<TDataItem, object>>[] selectExpressions) 
            where TDataItem : class, ITransactionContext, new()
        {
            if (repository == null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            if (selectExpressions == null)
            {
                throw new ArgumentNullException(nameof(selectExpressions));
            }

            return Query.From(selectExpressions);
        }
    }
}
