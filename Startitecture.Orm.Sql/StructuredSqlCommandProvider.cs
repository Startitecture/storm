// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StructuredSqlCommandProvider.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Sql
{
    using System;
    using System.Data;
    using System.Data.SqlClient;

    using Core;

    using JetBrains.Annotations;

    using Startitecture.Orm.Common;
    using Startitecture.Orm.Model;
    using Startitecture.Resources;

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
        public StructuredSqlCommandProvider([NotNull] IDatabaseContextProvider contextProvider)
        {
            this.contextProvider = contextProvider ?? throw new ArgumentNullException(nameof(contextProvider));
            this.EntityDefinitionProvider = contextProvider.DatabaseContext.DefinitionProvider;
        }

        /// <inheritdoc />
        public IEntityDefinitionProvider EntityDefinitionProvider { get; }

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

            if (!(this.contextProvider.DatabaseContext.Connection is SqlConnection sqlConnection))
            {
                throw new OperationException(this.contextProvider, ErrorMessages.DatabaseContextConnectionIsNotSqlConnection);
            }

            var sqlCommand = !(transaction is SqlTransaction sqlTransaction)
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