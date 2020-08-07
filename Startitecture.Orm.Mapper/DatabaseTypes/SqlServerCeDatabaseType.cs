// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlServerCEDatabaseType.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The SQL Server CE database type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Mapper.DatabaseTypes
{
    using System.Linq;

    /// <summary>
    /// The SQL Server CE database type.
    /// </summary>
    internal class SqlServerCeDatabaseType : DatabaseType
    {
        /////// <summary>
        /////// Builds a SQL query suitable for performing page based queries to the database.
        /////// </summary>
        /////// <param name="skip">
        /////// The number of rows that should be skipped by the query.
        /////// </param>
        /////// <param name="take">
        /////// The number of rows that should be returned by the query.
        /////// </param>
        /////// <param name="pageStatement">
        /////// The page statement.
        /////// </param>
        /////// <param name="args">
        /////// Arguments to any embedded parameters in the SQL query.
        /////// </param>
        /////// <returns>
        /////// The final SQL query that should be executed.
        /////// </returns>
        ////public override string BuildPageQuery(long skip, long take, SqlPageStatement pageStatement, ref object[] args)
        ////{
        ////    var sqlPage = $"{pageStatement.Sql}\nOFFSET @{args.Length} ROWS FETCH NEXT @{args.Length + 1} ROWS ONLY";
        ////    args = args.Concat(new object[] { skip, take }).ToArray();
        ////    return sqlPage;
        ////}

        /// <summary>
        /// Performs an insert operation
        /// </summary>
        /// <param name="databaseContext">
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
        public override object ExecuteInsert(DatabaseContext databaseContext, System.Data.IDbCommand command, string primaryKeyName)
        {
            command.ExecuteNonQuery();
            //// Database.ExecuteNonQueryHelper(command);
            return databaseContext.ExecuteScalar<object>("SELECT @@@IDENTITY AS NewID;");
        }
    }
}