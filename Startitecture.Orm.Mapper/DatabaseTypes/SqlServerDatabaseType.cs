// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlServerDatabaseType.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The SQL Server database type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Mapper.DatabaseTypes
{
    using System;
    using System.Data;
    using System.Linq;

    using Startitecture.Orm.Mapper.Internal;

    /// <summary>
    /// The SQL Server database type.
    /// </summary>
    internal class SqlServerDatabaseType : DatabaseType
    {
        #region Public Methods and Operators

        /// <summary>
        /// The build page query.
        /// </summary>
        /// <param name="skip">
        /// The skip.
        /// </param>
        /// <param name="take">
        /// The take.
        /// </param>
        /// <param name="pageStatement">
        /// The page statement.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string BuildPageQuery(long skip, long take, SqlPageStatement pageStatement, ref object[] args)
        {
            pageStatement.SqlSelectRemoved = PagingHelper.OrderByRegex.Replace(pageStatement.SqlSelectRemoved, string.Empty, 1);

            if (PagingHelper.DistinctRegex.IsMatch(pageStatement.SqlSelectRemoved))
            {
                pageStatement.SqlSelectRemoved = "peta_inner.* FROM (SELECT " + pageStatement.SqlSelectRemoved + ") peta_inner";
            }

            string sqlPage =
                string.Format(
                    "SELECT * FROM (SELECT ROW_NUMBER() OVER ({0}) peta_rn, {1}) peta_paged WHERE peta_rn>@{2} AND peta_rn<=@{3}", 
                    pageStatement.SqlOrderBy ?? "ORDER BY (SELECT NULL)", 
                    pageStatement.SqlSelectRemoved, 
                    args.Length, 
                    args.Length + 1);

            args = args.Concat(new object[] { skip, skip + take }).ToArray();

            return sqlPage;
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
            return database.ExecuteScalarHelper(command);
        }

        /// <summary>
        /// Returns an SQL Statement that can check for the existence of a row in the database.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> containing the existence SQL statement.
        /// </returns>
        public override string GetExistsSql()
        {
            return "IF EXISTS (SELECT 1 FROM {0} WHERE {1}) SELECT 1 ELSE SELECT 0";
        }

        /// <summary>
        /// Returns an SQL expression that can be used to specify the return value of auto incremented columns.
        /// </summary>
        /// <param name="primaryKeyName">
        /// The primary key of the row being inserted.
        /// </param>
        /// <returns>
        /// An expression describing how to return the new primary key value
        /// </returns>
        /// <remarks>
        /// See the SQLServer database provider for an example of how this method is used.
        /// </remarks>
        public override string GetInsertOutputClause(string primaryKeyName)
        {
            return String.Format(" OUTPUT INSERTED.[{0}]", primaryKeyName);
        }

        #endregion
    }
}