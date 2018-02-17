// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StructuredSqlCommandProvider.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers
{
    using System;
    using System.Data;
    using System.Data.SqlClient;

    using JetBrains.Annotations;

    using SAF.StringResources;

    using Startitecture.Core;

    /// <summary>
    /// The structured SQL command provider.
    /// </summary>
    public class StructuredSqlCommandProvider : IStructuredCommandProvider
    {
        /// <summary>
        /// The context provider.
        /// </summary>
        private readonly IDatabaseContextProvider contextProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="StructuredSqlCommandProvider"/> class.
        /// </summary>
        /// <param name="contextProvider">
        /// The context provider.
        /// </param>
        public StructuredSqlCommandProvider(IDatabaseContextProvider contextProvider)
        {
            this.contextProvider = contextProvider;
        }

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
        /// The underlying <see cref="IDatabaseContextProvider.DatabaseContext"/> does not contain a <see cref="SqlConnection"/>.
        /// </exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities",
            Justification = "structuredCommand.CommandText is built with parameterized input.")]
        public IDbCommand CreateCommand(
            [NotNull] IStructuredCommand structuredCommand,
            [NotNull] DataTable dataTable,
            [NotNull] IDbTransaction transaction)
        {
            if (structuredCommand == null)
            {
                throw new ArgumentNullException(nameof(structuredCommand));
            }

            if (dataTable == null)
            {
                throw new ArgumentNullException(nameof(dataTable));
            }

            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }

            var sqlConnection = this.contextProvider.DatabaseContext.Connection as SqlConnection;

            if (sqlConnection == null)
            {
                throw new OperationException(this.contextProvider, ErrorMessages.DatabaseContextConnectionIsNotSqlConnection);
            }

            var sqlTransaction = transaction as SqlTransaction;

            var sqlCommand = sqlTransaction == null
                                 ? new SqlCommand(structuredCommand.CommandText, sqlConnection)
                                 : new SqlCommand(structuredCommand.CommandText, sqlConnection, sqlTransaction);

            try
            {
                var tableParameter = sqlCommand.Parameters.AddWithValue(structuredCommand.Parameter, dataTable);
                tableParameter.SqlDbType = SqlDbType.Structured;
                tableParameter.TypeName = structuredCommand.StructureTypeName;
            }
            catch
            {
                sqlCommand.Dispose();
                throw;
            }

            return sqlCommand;
        }
    }
}