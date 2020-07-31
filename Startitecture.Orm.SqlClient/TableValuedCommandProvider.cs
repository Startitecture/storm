// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TableValuedCommandProvider.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.SqlClient
{
    using System;
    using System.Data;

    using Common;

    using Core;

    using JetBrains.Annotations;

    using Microsoft.Data.SqlClient;

    using Startitecture.Orm.Model;
    using Startitecture.Resources;

    /// <summary>
    /// The structured SQL command provider.
    /// </summary>
    public class TableValuedCommandProvider : IStructuredCommandProvider
    {
        /// <summary>
        /// The context provider.
        /// </summary>
        private readonly IDatabaseContextProvider contextProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="TableValuedCommandProvider"/> class.
        /// </summary>
        /// <param name="contextProvider">
        /// The context provider.
        /// </param>
        public TableValuedCommandProvider([NotNull] IDatabaseContextProvider contextProvider)
        {
            this.contextProvider = contextProvider ?? throw new ArgumentNullException(nameof(contextProvider));
            this.EntityDefinitionProvider = contextProvider.DatabaseContext.DefinitionProvider;
            this.NameQualifier = new TransactSqlQualifier();
        }

        /// <inheritdoc />
        public IEntityDefinitionProvider EntityDefinitionProvider { get; }

        /// <inheritdoc />
        public INameQualifier NameQualifier { get; }

        /// <summary>
        /// Creates an <see cref="System.Data.IDbCommand"/> for the specified <paramref name="structuredCommand"/>.
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
        /// <exception cref="Startitecture.Core.OperationException">
        /// The underlying <see cref="Startitecture.Orm.Common.IDatabaseContextProvider.DatabaseContext"/> does not contain a <see cref="SqlConnection"/>.
        /// </exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Security",
            "CA2100:Review SQL queries for security vulnerabilities",
            Justification = "structuredCommand.CommandText is built with parameterized input.")]
        public IDbCommand CreateCommand([NotNull] IStructuredCommand structuredCommand, [NotNull] DataTable dataTable, IDbTransaction transaction)
        {
            if (structuredCommand == null)
            {
                throw new ArgumentNullException(nameof(structuredCommand));
            }

            if (dataTable == null)
            {
                throw new ArgumentNullException(nameof(dataTable));
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
                var tableParameter = sqlCommand.Parameters.AddWithValue($@"{structuredCommand.Parameter}", dataTable);
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