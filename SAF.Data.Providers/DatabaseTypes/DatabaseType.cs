// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DatabaseType.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Base class for DatabaseType handlers - provides default/common handling for different database engines.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.DatabaseTypes
{
    using System;
    using System.Data;
    using System.Linq;

    using SAF.Core;
    using SAF.Data.Providers.Internal;

    /// <summary>
    /// Base class for DatabaseType handlers - provides default/common handling for different database engines.
    /// </summary>
    internal abstract class DatabaseType
    {
        #region Public Methods and Operators

        /// <summary>
        /// Look at the type and provider name being used and instantiate a suitable DatabaseType instance.
        /// </summary>
        /// <param name="typeName">
        /// The client type to resolve.
        /// </param>
        /// <param name="providerName">
        /// The name of the data provider.
        /// </param>
        /// <returns>
        /// The <see cref="DatabaseType"/> associated with the <paramref name="typeName"/> or <paramref name="providerName"/>.
        /// </returns>
        public static DatabaseType Resolve(string typeName, string providerName)
        {
            // Try using type name first (more reliable)
            if (typeName.StartsWith("MySql"))
            {
                return Singleton<MySqlDatabaseType>.Instance;
            }

            if (typeName.StartsWith("SqlCe"))
            {
                return Singleton<SqlServerCeDatabaseType>.Instance;
            }

            if (typeName.StartsWith("Npgsql") || typeName.StartsWith("PgSql"))
            {
                return Singleton<PostgreSqlDatabaseType>.Instance;
            }

            if (typeName.StartsWith("Oracle"))
            {
                return Singleton<OracleDatabaseType>.Instance;
            }

            if (typeName.StartsWith("SQLite"))
            {
                return Singleton<SQLiteDatabaseType>.Instance;
            }

            if (typeName.StartsWith("System.Data.SqlClient."))
            {
                return Singleton<SqlServerDatabaseType>.Instance;
            }

            if (typeName.StartsWith("Firebird"))
            {
                return Singleton<FirebirdDatabaseType>.Instance;
            }

            // This can happen during unit tests.
            if (string.IsNullOrWhiteSpace(providerName))
            {
                return Singleton<SqlServerDatabaseType>.Instance;
            }

            // Try again with provider name
            if (providerName.IndexOf("MySql", StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                return Singleton<MySqlDatabaseType>.Instance;
            }

            if (providerName.IndexOf("SqlServerCe", StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                return Singleton<SqlServerCeDatabaseType>.Instance;
            }

            if (providerName.IndexOf("pgsql", StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                return Singleton<PostgreSqlDatabaseType>.Instance;
            }

            if (providerName.IndexOf("Oracle", StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                return Singleton<OracleDatabaseType>.Instance;
            }

            if (providerName.IndexOf("SQLite", StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                return Singleton<SQLiteDatabaseType>.Instance;
            }

            if (providerName.IndexOf("Firebird", StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                return Singleton<FirebirdDatabaseType>.Instance;
            }

            // Assume SQL Server
            return Singleton<SqlServerDatabaseType>.Instance;
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
        public virtual string BuildPageQuery(long skip, long take, SqlPageStatement pageStatement, ref object[] args)
        {
            string sql = $"{pageStatement.Sql}\nLIMIT @{args.Length} OFFSET @{args.Length + 1}";
            args = args.Concat(new object[] { take, skip }).ToArray();
            return sql;
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
        public virtual string EscapeSqlIdentifier(string str)
        {
            return $"[{str}]";
        }

        /// <summary>
        /// Escape a table name into a suitable format for the associated database provider.
        /// </summary>
        /// <param name="tableName">
        /// The name of the table (as specified by the client program, or as attributes on the associated POCO class.
        /// </param>
        /// <returns>
        /// The escaped table name.
        /// </returns>
        public virtual string EscapeTableName(string tableName)
        {
            // Assume table names with "dot" are already escaped
            return tableName.IndexOf('.') >= 0 ? tableName : this.EscapeSqlIdentifier(tableName);
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
        public virtual object ExecuteInsert(Database database, IDbCommand command, string primaryKeyName)
        {
            command.CommandText += ";\nSELECT @@IDENTITY AS NewID;";
            return database.ExecuteScalarHelper(command);
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
        public virtual string GetAutoIncrementExpression(TableInfo tableInfo)
        {
            return null;
        }

        /// <summary>
        /// Returns an SQL Statement that can check for the existence of a row in the database.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> containing the existence SQL statement.
        /// </returns>
        public virtual string GetExistsSql()
        {
            return "SELECT COUNT(*) FROM {0} WHERE {1}";
        }

        /// <summary>
        /// Returns an SQL expression that can be used to specify the return value of auto incremented columns.
        /// </summary>
        /// <param name="primaryKeyName">
        /// The primary key of the row being inserted.
        /// </param>
        /// <returns>
        /// An expression describing how to return the new primary key value.
        /// </returns>
        /// <remarks>
        /// See the SQLServer database provider for an example of how this method is used.
        /// </remarks>
        public virtual string GetInsertOutputClause(string primaryKeyName)
        {
            return string.Empty;
        }

        /// <summary>
        /// Returns the prefix used to delimit parameters in SQL query strings.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string to get the parameter prefix for.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> containing the prefix for database parameters.
        /// </returns>
        public virtual string GetParameterPrefix(string connectionString)
        {
            return "@";
        }

        /// <summary>
        /// Converts a supplied C# object value into a value suitable for passing to the database.
        /// </summary>
        /// <param name="value">
        /// The value to convert.
        /// </param>
        /// <returns>
        /// The converted value.
        /// </returns>
        public virtual object MapParameterValue(object value)
        {
            // Cast bools to integer
            if (value is bool)
            {
                return ((bool)value) ? 1 : 0;
            }

            // Leave it
            return value;
        }

        /// <summary>
        /// Called immediately before a command is executed, allowing for modification of the IDbCommand before it's passed to the
        /// database provider.
        /// </summary>
        /// <param name="command">
        /// The command to execute.
        /// </param>
        public virtual void PreExecute(IDbCommand command)
        {
        }

        #endregion
    }
}