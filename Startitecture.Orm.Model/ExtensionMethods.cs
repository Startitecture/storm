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
        /// Gets a <see cref="StatementContext"/> as a SELECT statement.
        /// </summary>
        /// <param name="selection">
        /// The selection to use to create the context.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of entity being queried.
        /// </typeparam>
        /// <returns>
        /// A new <see cref="StatementContext"/> for the specified <paramref name="selection"/>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="selection"/> is null.
        /// </exception>
        public static StatementContext AsSelect<TItem>([NotNull] this EntitySelection<TItem> selection)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            var outputType = selection.ParentExpression == null ? StatementOutputType.Select : StatementOutputType.CteSelect;
            return new StatementContext(outputType);
        }

        /// <summary>
        /// Gets a <see cref="StatementContext"/> as a CONTAINS statement.
        /// </summary>
        /// <param name="selection">
        /// The selection to use to create the context.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of entity being queried.
        /// </typeparam>
        /// <returns>
        /// A new <see cref="StatementContext"/> for the specified <paramref name="selection"/>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="selection"/> is null.
        /// </exception>
        public static StatementContext AsContains<TItem>([NotNull] this EntitySelection<TItem> selection)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            return new StatementContext(StatementOutputType.Contains);
        }

        /// <summary>
        /// Gets a <see cref="StatementContext"/> as a DELETE statement.
        /// </summary>
        /// <param name="selection">
        /// The selection to use to create the context.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of entity being deleted.
        /// </typeparam>
        /// <returns>
        /// A new <see cref="StatementContext"/> for the specified <paramref name="selection"/>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="selection"/> is null.
        /// </exception>
        public static StatementContext AsDelete<TItem>([NotNull] this EntitySelection<TItem> selection)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            return new StatementContext(StatementOutputType.Delete);
        }
    }
}