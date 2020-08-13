﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TableValuedCommandProvider.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.SqlClient
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    using Common;

    using Core;

    using JetBrains.Annotations;

    using Microsoft.Data.SqlClient;

    using Startitecture.Resources;

    /// <summary>
    /// The structured SQL command provider.
    /// </summary>
    public class TableValuedCommandProvider : IStructuredCommandProvider
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
        /// <exception cref="OperationException">
        /// The underlying <see cref="IDatabaseContextProvider.DatabaseContext"/> does not contain a <see cref="SqlConnection"/>.
        /// </exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Security",
            "CA2100:Review SQL queries for security vulnerabilities",
            Justification = "structuredCommand.CommandText is built with parameterized input.")]
        public IDbCommand CreateCommand<T>(IStructuredCommand structuredCommand, IEnumerable<T> items, IDbTransaction transaction)
        {
            if (structuredCommand == null)
            {
                throw new ArgumentNullException(nameof(structuredCommand));
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
                                 ? new SqlCommand(structuredCommand.CommandText, sqlConnection)
                                 : new SqlCommand(structuredCommand.CommandText, sqlConnection, sqlTransaction);

            DataTable dataTable = null;

            try
            {
                var dataTableLoader = new DataTableLoader<T>(this.DatabaseContext.RepositoryAdapter.DefinitionProvider);
                dataTable = dataTableLoader.Load(items);
                var tableParameter = sqlCommand.Parameters.AddWithValue(
                    $"{this.DatabaseContext.RepositoryAdapter.NameQualifier.AddParameterPrefix(structuredCommand.Parameter)}",
                    dataTable);

                tableParameter.SqlDbType = SqlDbType.Structured;
                tableParameter.TypeName = structuredCommand.StructureTypeName;
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