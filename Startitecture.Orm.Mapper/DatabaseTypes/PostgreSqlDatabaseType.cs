// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostgreSQLDatabaseType.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The PostgreSQL database type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Mapper.DatabaseTypes
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
            return $"\"{str}\"";
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
                command.CommandText += $"returning {this.EscapeSqlIdentifier(primaryKeyName)} as NewID";
                return command.ExecuteScalar(); //// Database.ExecuteScalarHelper(command);
            }

            command.ExecuteNonQuery();
            ////Database.ExecuteNonQueryHelper(command);
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
            return value is bool ? value : base.MapParameterValue(value);
        }

        #endregion
    }
}