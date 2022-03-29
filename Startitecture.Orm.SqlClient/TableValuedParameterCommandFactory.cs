// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TableValuedParameterCommandFactory.cs" company="Startitecture">
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
    using JetBrains.Annotations;
    using Microsoft.Data.SqlClient;
    using Startitecture.Core;
    using Startitecture.Orm.Common;
    using Startitecture.Orm.Schema;
    using Startitecture.Resources;

    /// <summary>
    /// Creates <see cref="ITableCommand"/> instances for SQL Server using table-valued parameters (TVPs).
    /// </summary>
    public class TableValuedParameterCommandFactory : IDbTableCommandFactory
    {
        /// <summary>
        /// The database context.
        /// </summary>
        private readonly IDatabaseContext databaseContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="TableValuedParameterCommandFactory"/> class.
        /// </summary>
        /// <param name="databaseContext">
        /// The context provider.
        /// </param>
        public TableValuedParameterCommandFactory([NotNull] IDatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext ?? throw new ArgumentNullException(nameof(databaseContext));
        }

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
        public IDbCommand Create<T>(ITableCommand tableCommand, IEnumerable<T> items)
        {
            if (tableCommand == null)
            {
                throw new ArgumentNullException(nameof(tableCommand));
            }

            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            if (this.databaseContext.Connection is not SqlConnection sqlConnection)
            {
                throw new OperationException(this.databaseContext, ErrorMessages.DatabaseContextConnectionIsNotSqlConnection);
            }

            var sqlCommand = new SqlCommand(tableCommand.CommandText, sqlConnection);
            this.databaseContext.AssociateTransaction(sqlCommand);
            DataTable dataTable = null;

            try
            {
                var dataTableLoader = new DataTableLoader<T>(this.databaseContext.RepositoryAdapter.DefinitionProvider);
                dataTable = dataTableLoader.Load(items);
                var tableParameter = sqlCommand.Parameters.AddWithValue(
                    $"{this.databaseContext.RepositoryAdapter.NameQualifier.AddParameterPrefix(tableCommand.ParameterName)}",
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

            sqlCommand.Disposed += (_, _) => dataTable.Dispose();
            return sqlCommand;
        }
    }
}