// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITableCommandProvider.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   An interface for a table command provider, which abstracts the underlying database from the command.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Common
{
    using System.Collections.Generic;
    using System.Data;

    using JetBrains.Annotations;

    /// <summary>
    /// An interface for a table command provider, which abstracts the underlying database from the command.
    /// </summary>
    public interface ITableCommandProvider
    {
        /// <summary>
        /// Gets the database context for the table command.
        /// </summary>
        IDatabaseContext DatabaseContext { get; }

        /// <summary>
        /// Creates an <see cref="IDbCommand"/> for the specified <paramref name="tableCommand"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of items in the table command.
        /// </typeparam>
        /// <param name="tableCommand">
        /// The table command.
        /// </param>
        /// <param name="items">
        /// The items to pass to the command.
        /// </param>
        /// <param name="transaction">
        /// The transaction to use with the command.
        /// </param>
        /// <returns>
        /// An <see cref="IDbCommand"/> that will execute the table command.
        /// </returns>
        IDbCommand CreateCommand<T>([NotNull] ITableCommand tableCommand, [NotNull] IEnumerable<T> items, IDbTransaction transaction);
    }
}