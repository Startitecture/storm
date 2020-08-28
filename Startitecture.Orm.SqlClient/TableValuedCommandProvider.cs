// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TableValuedCommandProvider.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Creates ITableCommand instances for SQL Server using table-valued parameters (TVPs).
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.SqlClient
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Reflection;

    using Common;

    using Core;

    using JetBrains.Annotations;

    using Microsoft.Data.SqlClient;

    using Startitecture.Orm.Schema;
    using Startitecture.Resources;

    /// <summary>
    /// Creates <see cref="ITableCommand"/> instances for SQL Server using table-valued parameters (TVPs).
    /// </summary>
    public class TableValuedCommandProvider : ITableCommandProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableValuedCommandProvider"/> class.
        /// </summary>
        /// <param name="databaseContext">
        /// The context provider.
        /// </param>
        public TableValuedCommandProvider([NotNull] IDatabaseContext databaseContext)
        {
            this.DatabaseContext = databaseContext ?? throw new ArgumentNullException(nameof(databaseContext));
        }

        /// <inheritdoc />
        public IDatabaseContext DatabaseContext { get; }

        /// <summary>
        /// Creates an <see cref="IDbCommand"/> for the specified <paramref name="tableCommand"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of items in the structured command.
        /// </typeparam>
        /// <param name="tableCommand">
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
        /// <exception cref="OperationException">
        /// The underlying <see cref="IRepositoryProvider.DatabaseContext"/> does not contain a <see cref="SqlConnection"/>.
        /// </exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Security",
            "CA2100:Review SQL queries for security vulnerabilities",
            Justification = "tableCommand.CommandText is built with parameterized input.")]
        public IDbCommand CreateCommand<T>(ITableCommand tableCommand, IEnumerable<T> items, IDbTransaction transaction)
        {
            if (tableCommand == null)
            {
                throw new ArgumentNullException(nameof(tableCommand));
            }

            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            if (!(this.DatabaseContext.Connection is SqlConnection sqlConnection))
            {
                throw new OperationException(this.DatabaseContext, ErrorMessages.DatabaseContextConnectionIsNotSqlConnection);
            }

            var sqlCommand = !(transaction is SqlTransaction sqlTransaction)
                                 ? new SqlCommand(tableCommand.CommandText, sqlConnection)
                                 : new SqlCommand(tableCommand.CommandText, sqlConnection, sqlTransaction);

            DataTable dataTable = null;

            try
            {
                var dataTableLoader = new DataTableLoader<T>(this.DatabaseContext.RepositoryAdapter.DefinitionProvider);
                dataTable = dataTableLoader.Load(items);
                var tableParameter = sqlCommand.Parameters.AddWithValue(
                    $"{this.DatabaseContext.RepositoryAdapter.NameQualifier.AddParameterPrefix(tableCommand.ParameterName)}",
                    dataTable);

                tableParameter.SqlDbType = SqlDbType.Structured;
                tableParameter.TypeName = typeof(T).GetCustomAttribute<TableTypeAttribute>()?.TypeName ?? typeof(T).Name;
            }
            catch
            {
                dataTable?.Dispose();
                sqlCommand.Dispose();
                throw;
            }

            sqlCommand.Disposed += (sender, args) => dataTable.Dispose();
            return sqlCommand;
        }
    }
}