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

    using Microsoft.Data.SqlClient;
    using Startitecture.Core;
    using Startitecture.Orm.Common;
    using Startitecture.Orm.Schema;
    using Startitecture.Resources;

    /// <summary>
    /// Creates <see cref="IDbCommand"/> instances for SQL Server using table-valued parameters (TVPs).
    /// </summary>
    public class TableValuedParameterCommandFactory : IDbTableCommandFactory
    {
        /// <inheritdoc/>
        /// <exception cref="OperationException">
        /// <paramref name="databaseContext"/> does not have a <see cref="IDatabaseContext.Connection"/> of type <see cref="SqlConnection"/>.
        /// </exception>
        public IDbCommand Create<T>(IDatabaseContext databaseContext, string commandText, string parameterName, IEnumerable<T> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            if (string.IsNullOrWhiteSpace(commandText))
            {
                throw new ArgumentException($"'{nameof(commandText)}' cannot be null or whitespace.", nameof(commandText));
            }

            if (string.IsNullOrWhiteSpace(parameterName))
            {
                throw new ArgumentException($"'{nameof(parameterName)}' cannot be null or whitespace.", nameof(parameterName));
            }

            if (databaseContext.Connection is not SqlConnection sqlConnection)
            {
                throw new OperationException(databaseContext, ErrorMessages.DatabaseContextConnectionIsNotSqlConnection);
            }

            var sqlCommand = new SqlCommand(commandText, sqlConnection);
            DataTable dataTable = null;

            try
            {
                databaseContext.AssociateTransaction(sqlCommand);
                var dataTableLoader = new DataTableLoader<T>(databaseContext.RepositoryAdapter.DefinitionProvider);
                dataTable = dataTableLoader.Load(items);
                var tableParameter = sqlCommand.Parameters.AddWithValue(
                    $"{databaseContext.RepositoryAdapter.NameQualifier.AddParameterPrefix(parameterName)}",
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