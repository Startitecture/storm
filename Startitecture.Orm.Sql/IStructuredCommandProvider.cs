// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IStructuredCommandProvider.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The StructuredCommandProvider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Sql
{
    using System.Data;
    using System.Data.SqlClient;

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
        /// <exception cref="Startitecture.Core.OperationException">
        /// The underlying <see cref="SAF.Data.Providers.IDatabaseContextProvider.DatabaseContext"/> does not contain a <see cref="SqlConnection"/>.
        /// </exception>
        IDbCommand CreateCommand(IStructuredCommand structuredCommand, DataTable dataTable, IDbTransaction transaction);
    }
}