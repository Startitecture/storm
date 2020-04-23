// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionMethods.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model
{
    using System;

    using JetBrains.Annotations;

    /// <summary>
    /// The extension methods.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Gets a <see cref="QueryContext{TItem}"/> as a SELECT statement.
        /// </summary>
        /// <param name="selection">
        /// The selection to use to create the context.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item being queried.
        /// </typeparam>
        /// <returns>
        /// A new <see cref="QueryContext{TItem}"/> for the specified <paramref name="selection"/>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="selection"/> is null.
        /// </exception>
        public static QueryContext<TItem> AsSelect<TItem>([NotNull] this ItemSelection<TItem> selection)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            return new QueryContext<TItem>(selection, StatementOutputType.Select, 0);
        }

        /// <summary>
        /// Gets a <see cref="QueryContext{TItem}"/> as a CONTAINS statement.
        /// </summary>
        /// <param name="selection">
        /// The selection to use to create the context.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item being queried.
        /// </typeparam>
        /// <returns>
        /// A new <see cref="QueryContext{TItem}"/> for the specified <paramref name="selection"/>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="selection"/> is null.
        /// </exception>
        public static QueryContext<TItem> AsContains<TItem>([NotNull] this ItemSelection<TItem> selection)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            return new QueryContext<TItem>(selection, StatementOutputType.Contains, 0);
        }

        /// <summary>
        /// Gets a <see cref="QueryContext{TItem}"/> as a DELETE statement.
        /// </summary>
        /// <param name="selection">
        /// The selection to use to create the context.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item being deleted.
        /// </typeparam>
        /// <returns>
        /// A new <see cref="QueryContext{TItem}"/> for the specified <paramref name="selection"/>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="selection"/> is null.
        /// </exception>
        public static QueryContext<TItem> AsDelete<TItem>([NotNull] this ItemSelection<TItem> selection)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            return new QueryContext<TItem>(selection, StatementOutputType.Delete, 0);
        }
    }
}