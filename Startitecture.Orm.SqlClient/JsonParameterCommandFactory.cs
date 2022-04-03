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

    using Microsoft.Data.SqlClient;

    using Startitecture.Core;
    using Startitecture.Orm.Common;
    using Startitecture.Resources;

    /// <summary>
    /// A table command factory for performing JSON-based table insert or merge commands.
    /// </summary>
    public class JsonParameterCommandFactory : IDbTableCommandFactory
    {
        /// <inheritdoc />
        /// <exception cref="OperationException">
        /// <paramref name="databaseContext"/> does not have a <see cref="IDatabaseContext.Connection"/> of type <see cref="SqlConnection"/>.
        /// </exception>
        public IDbCommand Create<T>(IDatabaseContext databaseContext, string commandText, string parameterName, IEnumerable<T> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            if (databaseContext.Connection is not SqlConnection sqlConnection)
            {
                throw new OperationException(databaseContext.Connection, ErrorMessages.DatabaseContextConnectionIsNotSqlConnection);
            }

            if (string.IsNullOrWhiteSpace(commandText))
            {
                throw new ArgumentException($"'{nameof(commandText)}' cannot be null or whitespace.", nameof(commandText));
            }

            if (string.IsNullOrWhiteSpace(parameterName))
            {
                throw new ArgumentException($"'{nameof(parameterName)}' cannot be null or whitespace.", nameof(parameterName));
            }

            var sqlCommand = new SqlCommand(commandText, sqlConnection);

            try
            {
                databaseContext.AssociateTransaction(sqlCommand);
                var serializationOptions = new JsonSerializerOptions();
                serializationOptions.Converters.Add(new MoneyConverter());

                var tableParameter = sqlCommand.Parameters.AddWithValue(
                    $"{databaseContext.RepositoryAdapter.NameQualifier.AddParameterPrefix(parameterName)}",
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