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
    using System.Data;

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
        /// <param name="structuredCommand">
        /// The structured command.
        /// </param>
        /// <param name="dataTable">
        /// The data table to include with the command.
        /// </param>
        /// <param name="transaction">
        /// The transaction to use with the command.
        /// </param>
        /// <returns>
        /// An <see cref="System.Data.IDbCommand"/> that will execute the structured command.
        /// </returns>
        IDbCommand CreateCommand(IStructuredCommand structuredCommand, DataTable dataTable, IDbTransaction transaction);
    }
}