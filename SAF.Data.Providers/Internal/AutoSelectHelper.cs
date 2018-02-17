// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutoSelectHelper.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The auto select helper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Internal
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;

    using JetBrains.Annotations;

    using SAF.Data.Providers.DatabaseTypes;

    using Startitecture.Orm.Model;
    using Startitecture.Orm.Sql;

    /// <summary>
    /// The auto select helper.
    /// </summary>
    internal class AutoSelectHelper
    {
        #region Static Fields

        /// <summary>
        /// The FROM regex.
        /// </summary>
        private static readonly Regex FromRegex = new Regex(
            @"\A\s*FROM\s", 
            RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Multiline);

        /// <summary>
        /// The SELECT regex.
        /// </summary>
        private static readonly Regex SelectRegex = new Regex(
            @"\A\s*(SELECT|EXECUTE|CALL)\s", 
            RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Multiline);

        #endregion

        /// <summary>
        /// The database type.
        /// </summary>
        private readonly DatabaseType databaseType;

        /// <summary>
        /// The entity definition.
        /// </summary>
        private readonly IEntityDefinition entityDefinition;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoSelectHelper"/> class.
        /// </summary>
        /// <param name="databaseType">
        /// The database type.
        /// </param>
        /// <param name="entityDefinition">
        /// The entity definition.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="databaseType"/> or <paramref name="entityDefinition"/> is null.
        /// </exception>
        public AutoSelectHelper([NotNull] DatabaseType databaseType, [NotNull] IEntityDefinition entityDefinition)
        {
            if (databaseType == null)
            {
                throw new ArgumentNullException(nameof(databaseType));
            }

            if (entityDefinition == null)
            {
                throw new ArgumentNullException(nameof(entityDefinition));
            }

            this.databaseType = databaseType;
            this.entityDefinition = entityDefinition;
        }

        #region Public Methods and Operators

        /// <summary>
        /// Adds a select clause to the SQL statement.
        /// </summary>
        /// <param name="sql">
        /// The SQL statement.
        /// </param>
        /// <returns>
        /// The SQL statement as a <see cref="string"/>.
        /// </returns>
        public string AddSelectClause(string sql)
        {
            if (sql.StartsWith(";"))
            {
                return sql.Substring(1);
            }

            if (SelectRegex.IsMatch(sql))
            {
                return sql;
            }

            string tableName = this.entityDefinition.GetQualifiedName(); //// this.databaseType.EscapeTableName(this.pocoData.TableInfo.TableName);
            var queryColumns = from c in this.entityDefinition.DirectAttributes
                               select tableName + "." + this.databaseType.EscapeSqlIdentifier(c.PhysicalName);

            string columns = this.entityDefinition.DirectAttributes.Any() ? "NULL" : string.Join(", ", queryColumns.ToArray());

            return FromRegex.IsMatch(sql) ? $"SELECT {columns} {sql}" : $"SELECT {columns} FROM {tableName} {sql}";
        }

        #endregion
    }
}