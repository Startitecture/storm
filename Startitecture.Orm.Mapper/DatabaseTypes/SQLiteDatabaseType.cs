// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SQLiteDatabaseType.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The SQLite database type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Mapper.DatabaseTypes
{
    using System.Data;

    /// <summary>
    /// The SQLite database type.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    internal class SQLiteDatabaseType : DatabaseType
    {
        #region Public Methods and Operators

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
                command.CommandText += ";\nSELECT last_insert_rowid();";
                return database.ExecuteScalarHelper(command);
            }

            database.ExecuteNonQueryHelper(command);
            return -1;
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
        /// Converts a supplied C# object value into a value suitable for passing to the database.
        /// </summary>
        /// <param name="value">
        /// The value to convert.
        /// </param>
        /// <returns>
        /// The converted value.
        /// </returns>
        public override object MapParameterValue(object value)
        {
            if (value is uint)
            {
                return (long)((uint)value);
            }

            return base.MapParameterValue(value);
        }

        #endregion
    }
}