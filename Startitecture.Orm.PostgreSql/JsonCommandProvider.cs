// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonCommandProvider.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The JSON command provider.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.PostgreSql
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    using JetBrains.Annotations;

    using Npgsql;

    using NpgsqlTypes;

    using Startitecture.Core;
    using Startitecture.Orm.Common;

    /// <summary>
    /// The JSON command provider.
    /// </summary>
    public class JsonCommandProvider : ITableCommandProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonCommandProvider" /> class.
        /// </summary>
        /// <param name="databaseContext">
        /// The database context.
        /// </param>
        public JsonCommandProvider([NotNull] IDatabaseContext databaseContext)
        {
            this.DatabaseContext = databaseContext ?? throw new ArgumentNullException(nameof(databaseContext));
        }

        /// <inheritdoc />
        public IDatabaseContext DatabaseContext { get; }

        /// <inheritdoc />
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

            if (!(this.DatabaseContext.Connection is NpgsqlConnection sqlConnection))
            {
                throw new OperationException(this.DatabaseContext, $"This operation requires a connection type of {nameof(NpgsqlConnection)}.");
            }

            var npgsqlCommand = !(transaction is NpgsqlTransaction sqlTransaction)
                                    ? new NpgsqlCommand(tableCommand.CommandText, sqlConnection)
                                    : new NpgsqlCommand(tableCommand.CommandText, sqlConnection, sqlTransaction);

            try
            {
                var parameter = new NpgsqlParameter(tableCommand.ParameterName, NpgsqlDbType.Jsonb)
                                    {
                                        Value = items
                                    };

                npgsqlCommand.Parameters.Add(parameter);
            }
            catch
            {
                npgsqlCommand.Dispose();
                throw;
            }

            return npgsqlCommand;
        }
    }
}