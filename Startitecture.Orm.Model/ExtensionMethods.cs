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
        /// Gets a <see cref="QueryContext"/> as a SELECT statement.
        /// </summary>
        /// <param name="selection">
        /// The selection to use to create the context.
        /// </param>
        /// <param name="entityDefinition">
        /// The entity definition for the <typeparamref name="TItem"/>.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item being queried.
        /// </typeparam>
        /// <returns>
        /// A new <see cref="QueryContext"/> for the specified <paramref name="selection"/>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="selection"/> is null.
        /// </exception>
        public static QueryContext AsSelect<TItem>([NotNull] this ItemSelection<TItem> selection, IEntityDefinition entityDefinition)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            var outputType = selection.Page.Size + selection.Page.RowOffset > 0 ? StatementOutputType.PageSelect : StatementOutputType.Select;
            return new QueryContext(selection, entityDefinition, outputType, 0);
        }

        /// <summary>
        /// Gets a <see cref="QueryContext"/> as a CONTAINS statement.
        /// </summary>
        /// <param name="selection">
        /// The selection to use to create the context.
        /// </param>
        /// <param name="entityDefinition">
        /// The entity definition for the <typeparamref name="TItem"/>.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item being queried.
        /// </typeparam>
        /// <returns>
        /// A new <see cref="QueryContext"/> for the specified <paramref name="selection"/>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="selection"/> is null.
        /// </exception>
        public static QueryContext AsContains<TItem>([NotNull] this ItemSelection<TItem> selection, IEntityDefinition entityDefinition)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            return new QueryContext(selection, entityDefinition, StatementOutputType.Contains, 0);
        }

        /// <summary>
        /// Gets a <see cref="QueryContext"/> as a DELETE statement.
        /// </summary>
        /// <param name="selection">
        /// The selection to use to create the context.
        /// </param>
        /// <param name="entityDefinition">
        /// The entity definition for the <typeparamref name="TItem"/>.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item being deleted.
        /// </typeparam>
        /// <returns>
        /// A new <see cref="QueryContext"/> for the specified <paramref name="selection"/>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="selection"/> is null.
        /// </exception>
        public static QueryContext AsDelete<TItem>([NotNull] this ItemSelection<TItem> selection, IEntityDefinition entityDefinition)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            return new QueryContext(selection, entityDefinition, StatementOutputType.Delete, 0);
        }
    }
}