// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostgreSQLDatabaseType.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   The PostgreSQL database type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.DatabaseTypes
{
    using System.Data;

    /// <summary>
    /// The PostgreSQL database type.
    /// </summary>
    internal class PostgreSqlDatabaseType : DatabaseType
    {
        #region Public Methods and Operators

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
            return string.Format("\"{0}\"", str);
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
                command.CommandText += string.Format("returning {0} as NewID", this.EscapeSqlIdentifier(primaryKeyName));
                return database.ExecuteScalarHelper(command);
            }

            database.ExecuteNonQueryHelper(command);
            return -1;
        }

        /// <summary>
        /// The map parameter value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public override object MapParameterValue(object value)
        {
            // Don't map bools to ints in PostgreSQL
            if (value is bool)
            {
                return value;
            }

            return base.MapParameterValue(value);
        }

        #endregion
    }
}