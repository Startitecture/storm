// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IStructuredCommandProvider.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The StructuredCommandProvider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Common
{
    using System.Collections.Generic;
    using System.Data;

    using JetBrains.Annotations;

    /// <summary>
    /// The StructuredCommandProvider interface.
    /// </summary>
    public interface IStructuredCommandProvider
    {
        /// <summary>
        /// Gets the database context for the structured command.
        /// </summary>
        IDatabaseContext DatabaseContext { get; }

        /// <summary>
        /// Creates an <see cref="IDbCommand"/> for the specified <paramref name="structuredCommand"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of items in the structured command.
        /// </typeparam>
        /// <param name="structuredCommand">
        /// The structured command.
        /// </param>
        /// <param name="items">
        /// The items to pass to the command.
        /// </param>
        /// <param name="transaction">
        /// The transaction to use with the command.
        /// </param>
        /// <returns>
        /// An <see cref="IDbCommand"/> that will execute the structured command.
        /// </returns>
        IDbCommand CreateCommand<T>([NotNull] IStructuredCommand structuredCommand, [NotNull] IEnumerable<T> items, IDbTransaction transaction);
    }
}