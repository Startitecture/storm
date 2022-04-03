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

    using Npgsql;

    using NpgsqlTypes;

    using Startitecture.Core;
    using Startitecture.Orm.Common;

    /// <summary>
    /// A command provider for inserting JSON objects into PostgreSQL database tables.
    /// </summary>
    public class JsonCommandFactory : IDbTableCommandFactory
    {
        /// <exception cref="OperationException">
        /// <paramref name="databaseContext"/> does not have a <see cref="IDatabaseContext.Connection"/> of the type <see cref="NpgsqlConnection"/>.
        /// </exception>
        /// <inheritdoc />
        public IDbCommand Create<T>(IDatabaseContext databaseContext, string commandText, string parameterName, IEnumerable<T> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            if (databaseContext.Connection is not NpgsqlConnection sqlConnection)
            {
                throw new OperationException(databaseContext.Connection, $"This operation requires a connection type of {nameof(NpgsqlConnection)}.");
            }

            if (string.IsNullOrWhiteSpace(commandText))
            {
                throw new ArgumentException($"'{nameof(commandText)}' cannot be null or whitespace.", nameof(commandText));
            }

            if (string.IsNullOrWhiteSpace(parameterName))
            {
                throw new ArgumentException($"'{nameof(parameterName)}' cannot be null or whitespace.", nameof(parameterName));
            }

            var npgsqlCommand = new NpgsqlCommand(commandText, sqlConnection);

            try
            {
                databaseContext.AssociateTransaction(npgsqlCommand);

                var parameter = new NpgsqlParameter(parameterName, NpgsqlDbType.Jsonb)
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