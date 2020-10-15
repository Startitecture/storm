// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonCommandFactory.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   A command provider for inserting JSON objects into PostgreSQL database tables.
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
    /// A command provider for inserting JSON objects into PostgreSQL database tables.
    /// </summary>
    public class JsonCommandFactory : IDbTableCommandFactory
    {
        private readonly IDatabaseContext databaseContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonCommandFactory" /> class.
        /// </summary>
        /// <param name="databaseContext">
        /// The database context.
        /// </param>
        public JsonCommandFactory([NotNull] IDatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext ?? throw new ArgumentNullException(nameof(databaseContext));
        }

        /// <inheritdoc />
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

            if (!(this.databaseContext.Connection is NpgsqlConnection sqlConnection))
            {
                throw new OperationException(this.databaseContext, $"This operation requires a connection type of {nameof(NpgsqlConnection)}.");
            }

            var npgsqlCommand = new NpgsqlCommand(tableCommand.CommandText, sqlConnection);
            this.databaseContext.AssociateTransaction(npgsqlCommand);

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