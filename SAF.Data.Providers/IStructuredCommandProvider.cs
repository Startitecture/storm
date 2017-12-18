// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IStructuredCommandProvider.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   The StructuredCommandProvider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers
{
    using System.Data;
    using System.Data.SqlClient;

    using SAF.Core;

    /// <summary>
    /// The StructuredCommandProvider interface.
    /// </summary>
    public interface IStructuredCommandProvider
    {
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
        /// An <see cref="IDbCommand"/> that will execute the structured command.
        /// </returns>
        /// <exception cref="OperationException">
        /// The underlying <see cref="IDatabaseContextProvider.DatabaseContext"/> does not contain a <see cref="SqlConnection"/>.
        /// </exception>
        IDbCommand CreateCommand(IStructuredCommand structuredCommand, DataTable dataTable, IDbTransaction transaction);
    }
}