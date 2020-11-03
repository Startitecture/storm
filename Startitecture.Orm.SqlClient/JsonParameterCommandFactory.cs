// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonParameterCommandFactory.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.SqlClient
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text.Json;

    using JetBrains.Annotations;

    using Microsoft.Data.SqlClient;

    using Startitecture.Core;
    using Startitecture.Orm.Common;
    using Startitecture.Resources;

    /// <summary>
    /// A table command factory for performing JSON-based table insert or merge commands.
    /// </summary>
    public class JsonParameterCommandFactory : IDbTableCommandFactory
    {
        /// <summary>
        /// The database context.
        /// </summary>
        private readonly IDatabaseContext databaseContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonParameterCommandFactory"/> class.
        /// </summary>
        /// <param name="databaseContext">
        /// The context provider.
        /// </param>
        public JsonParameterCommandFactory([NotNull] IDatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext ?? throw new ArgumentNullException(nameof(databaseContext));
        }

        /// <inheritdoc />
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

            if (!(this.databaseContext.Connection is SqlConnection sqlConnection))
            {
                throw new OperationException(this.databaseContext, ErrorMessages.DatabaseContextConnectionIsNotSqlConnection);
            }

#pragma warning disable CA2100 // Review SQL queries for security vulnerabilities
            var sqlCommand = new SqlCommand(tableCommand.CommandText, sqlConnection);
#pragma warning restore CA2100 // Review SQL queries for security vulnerabilities
            this.databaseContext.AssociateTransaction(sqlCommand);

            try
            {
                var serializationOptions = new JsonSerializerOptions();
                serializationOptions.Converters.Add(new MoneyConverter());

                var tableParameter = sqlCommand.Parameters.AddWithValue(
                    $"{this.databaseContext.RepositoryAdapter.NameQualifier.AddParameterPrefix(tableCommand.ParameterName)}",
                    JsonSerializer.Serialize(items.ToList(), serializationOptions));

                tableParameter.SqlDbType = SqlDbType.NVarChar;
                tableParameter.Size = 4000;
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