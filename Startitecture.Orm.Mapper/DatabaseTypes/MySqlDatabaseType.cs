// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MySqlDatabaseType.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The MySQL database type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Mapper.DatabaseTypes
{
    using System;

    /// <summary>
    /// The MySQL database type.
    /// </summary>
    internal class MySqlDatabaseType : DatabaseType
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
            return string.Format("`{0}`", str);
        }

        /// <summary>
        /// Returns an SQL Statement that can check for the existence of a row in the database.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> containing the existence SQL statement.
        /// </returns>
        public override string GetExistsSql()
        {
            return "SELECT EXISTS (SELECT 1 FROM {0} WHERE {1})";
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
        public override string GetParameterPrefix(string connectionString)
        {
            if (connectionString != null && connectionString.IndexOf("Allow User Variables=true", StringComparison.CurrentCulture) >= 0)
            {
                return "?";
            }

            return "@";
        }

        #endregion
    }
}