// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OracleDatabaseType.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The Oracle database type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Mapper.DatabaseTypes
{
    using System;
    using System.Data;

    using Startitecture.Core;
    using Startitecture.Orm.Mapper.Internal;
    using Startitecture.Orm.Schema;
    using Startitecture.Resources;

    /// <summary>
    /// The Oracle database type.
    /// </summary>
    internal class OracleDatabaseType : DatabaseType
    {
        /// <summary>
        /// Returns the prefix used to delimit parameters in SQL query strings.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string to get the parameter prefix for.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> containing the prefix for database parameters.
        /// </returns>
        public override string GetParameterPrefix(string connectionString)
        {
            return ":";
        }

        /// <summary>
        /// Called immediately before a command is executed, allowing for modification of the IDbCommand before it's passed to the
        /// database provider.
        /// </summary>
        /// <param name="command">
        /// The command to execute.
        /// </param>
        public override void PreExecute(IDbCommand command)
        {
            command.GetType().GetProperty("BindByName")?.SetValue(command, true, null);
        }

        /// <summary>
        /// Builds a SQL query suitable for performing page based queries to the database.
        /// </summary>
        /// <param name="skip">
        /// The number of rows that should be skipped by the query.
        /// </param>
        /// <param name="take">
        /// The number of rows that should be returned by the query.
        /// </param>
        /// <param name="pageStatement">
        /// The page statement.
        /// </param>
        /// <param name="args">
        /// Arguments to any embedded parameters in the SQL query.
        /// </param>
        /// <returns>
        /// The final SQL query that should be executed.
        /// </returns>
        public override string BuildPageQuery(long skip, long take, SqlPageStatement pageStatement, ref object[] args)
        {
            if (pageStatement.SqlSelectRemoved.StartsWith("*"))
            {
                throw new InvalidOperationException(ErrorMessages.OracleQueryMustAliasForPagedQuery);
            }

            // Same deal as SQL Server
            return Singleton<SqlServerDatabaseType>.Instance.BuildPageQuery(skip, take, pageStatement, ref args);
        }

        /// <summary>
        /// Escapes a SQL identifier into a format suitable for the associated database provider.
        /// </summary>
        /// <param name="str">
        /// The SQL identifier to be escaped.
        /// </param>
        /// <returns>
        /// The escaped identifier.
        /// </returns>
        public override string EscapeSqlIdentifier(string str)
        {
            return $"\"{str.ToUpperInvariant()}\"";
        }

        /// <summary>
        /// Return an SQL expression that can be used to populate the primary key column of an auto-increment column.
        /// </summary>
        /// <param name="tableInfo">
        /// Table info describing the table.
        /// </param>
        /// <returns>
        /// A SQL expression for populating the primary key column.
        /// </returns>
        /// <remarks>
        /// See the Oracle database type for an example of how this method is used.
        /// </remarks>
        public override string GetAutoIncrementExpression(TableInfo tableInfo)
        {
            return string.IsNullOrEmpty(tableInfo.SequenceName) ? null : $"{tableInfo.SequenceName}.nextval";
        }

        /// <summary>
        /// Performs an insert operation
        /// </summary>
        /// <param name="database">
        /// The calling database object.
        /// </param>
        /// <param name="command">
        /// The insert command to be executed.
        /// </param>
        /// <param name="primaryKeyName">
        /// The primary key of the table being inserted into.
        /// </param>
        /// <returns>
        /// The ID of the newly inserted record.
        /// </returns>
        public override object ExecuteInsert(Database database, IDbCommand command, string primaryKeyName)
        {
            if (primaryKeyName != null)
            {
                command.CommandText += $" returning {this.EscapeSqlIdentifier(primaryKeyName)} into :newid";
                var param = command.CreateParameter();
                param.ParameterName = ":newid";
                param.Value = DBNull.Value;
                param.Direction = ParameterDirection.ReturnValue;
                param.DbType = DbType.Int64;
                command.Parameters.Add(param);
                database.ExecuteNonQueryHelper(command);
                return param.Value;
            }

            database.ExecuteNonQueryHelper(command);
            return -1;
        }
    }
}