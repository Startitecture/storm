// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDatabaseContext.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Defines the IDatabaseContext type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Common
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    using Startitecture.Orm.Model;
    using Startitecture.Orm.Query;

    /// <summary>
    /// Provides an interface for classes that contain database contexts.
    /// </summary>
    public interface IDatabaseContext : IDisposable
    {
        /// <summary>
        /// Gets or sets the timeout value for all SQL statements.
        /// </summary>
        int CommandTimeout { get; set; }

        /// <summary>
        /// Gets the currently open shared connection (or null if none)
        /// </summary>
        IDbConnection Connection { get; }

        /// <summary>
        /// Gets or sets a value indicating whether to automatically create the "SELECT columns" part of any query that looks like it 
        /// needs it.
        /// </summary>
        bool EnableAutoSelect { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether parameters named <code>?myparam</code> are populated from properties of the passed
        /// in argument values.
        /// </summary>
        bool EnableNamedParameters { get; set; }

        /// <summary>
        /// Gets or sets the timeout value for the next (and only next) SQL statement.
        /// </summary>
        int OnetimeCommandTimeout { get; set; }

        /// <summary>
        /// Gets the definition provider for the database context.
        /// </summary>
        IEntityDefinitionProvider DefinitionProvider { get; }

        /// <summary>
        /// Open a connection that will be used for all subsequent queries.
        /// </summary>
        /// <remarks>
        /// Calls to Open/CloseSharedConnection are reference counted and should be balanced
        /// </remarks>
        void OpenSharedConnection();

        /// <summary>
        /// Starts a transaction scope, see GetTransaction() for recommended usage
        /// </summary>
        /// <returns>
        /// The <see cref="IDbTransaction"/> that was opened with this call.
        /// </returns>
        IDbTransaction BeginTransaction();

        /// <summary>
        /// Aborts the entire outer most transaction scope
        /// </summary>
        /// <remarks>
        /// Called automatically by Transaction.Dispose() if the transaction wasn't completed.
        /// </remarks>
        void AbortTransaction();

        /// <summary>
        /// Marks the current transaction scope as complete.
        /// </summary>
        void CompleteTransaction();

        /// <summary>
        /// Executes a non-query command
        /// </summary>
        /// <param name="sql">
        /// The SQL statement to execute
        /// </param>
        /// <param name="args">
        /// Arguments to any embedded parameters in the SQL
        /// </param>
        /// <returns>
        /// The number of rows affected
        /// </returns>
        int Execute(string sql, params object[] args);

        /// <summary>
        /// Executes a query and return the first column of the first row in the result set.
        /// </summary>
        /// <typeparam name="T">
        /// The type that the result value should be cast to
        /// </typeparam>
        /// <param name="sql">
        /// The SQL query to execute
        /// </param>
        /// <param name="args">
        /// Arguments to any embedded parameters in the SQL
        /// </param>
        /// <returns>
        /// The scalar value cast to T
        /// </returns>
        T ExecuteScalar<T>(string sql, params object[] args);

        /// <summary>
        /// Runs an SQL query, returning the results as an IEnumerable collection.
        /// </summary>
        /// <typeparam name="T">
        /// The Type representing a row in the result set.
        /// </typeparam>
        /// <param name="sql">
        /// The SQL query.
        /// </param>
        /// <param name="args">
        /// Arguments to any embedded parameters in the SQL statement.
        /// </param>
        /// <returns>
        /// An enumerable collection of result records.
        /// </returns>
        /// <remarks>
        /// For some DB providers, care should be taken to not start a new Query before finishing with and disposing the previous one.
        /// In cases where this is an issue, consider using Fetch which returns the results as a List rather than an IEnumerable.
        /// </remarks>
        IEnumerable<T> Query<T>(string sql, params object[] args);

        /// <summary>
        /// Runs a query and returns the result set as a typed list
        /// </summary>
        /// <typeparam name="T">
        /// The Type representing a row in the result set
        /// </typeparam>
        /// <param name="sql">
        /// The SQL query to execute
        /// </param>
        /// <param name="args">
        /// Arguments to any embedded parameters in the SQL
        /// </param>
        /// <returns>
        /// A List holding the results of the query
        /// </returns>
        IEnumerable<T> Fetch<T>(string sql, params object[] args);

        /// <summary>
        /// Retrieves a page of records (without the total count)
        /// </summary>
        /// <typeparam name="T">
        /// The Type representing a row in the result set
        /// </typeparam>
        /// <param name="page">
        /// The 1 based page number to retrieve
        /// </param>
        /// <param name="itemsPerPage">
        /// The number of records per page
        /// </param>
        /// <param name="sql">
        /// The base SQL query
        /// </param>
        /// <param name="args">
        /// Arguments to any embedded parameters in the SQL statement
        /// </param>
        /// <returns>
        /// A List of results
        /// </returns>
        /// <remarks>
        /// PetaPoco will automatically modify the supplied SELECT statement to only retrieve the
        /// records for the specified page.
        /// </remarks>
        IEnumerable<T> Fetch<T>(long page, long itemsPerPage, string sql, params object[] args);

        /// <summary>
        /// Retrieves a page of records and the total number of available records.
        /// </summary>
        /// <typeparam name="T">
        /// The Type representing a row in the result set.
        /// </typeparam>
        /// <param name="page">
        /// The 1 based page number to retrieve.
        /// </param>
        /// <param name="itemsPerPage">
        /// The number of records per page.
        /// </param>
        /// <param name="sqlCount">
        /// The SQL to retrieve the total number of records.
        /// </param>
        /// <param name="countArgs">
        /// Arguments to any embedded parameters in the <paramref name="sqlCount"/> statement.
        /// </param>
        /// <param name="sqlPage">
        /// The SQL To retrieve a single page of results.
        /// </param>
        /// <param name="pageArgs">
        /// Arguments to any embedded parameters in the <paramref name="sqlPage"/> statement.
        /// </param>
        /// <returns>
        /// A Page of results.
        /// </returns>
        /// <remarks>
        /// This method allows separate SQL statements to be explicitly provided for the two parts of the page query.
        /// The page and itemsPerPage parameters are not used directly and are used simply to populate the returned Page object.
        /// </remarks>
        Page<T> FetchPage<T>(
            long page,
            long itemsPerPage,
            string sqlCount,
            object[] countArgs,
            string sqlPage,
            object[] pageArgs);

        /// <summary>
        /// Retrieves a page of records and the total number of available records
        /// </summary>
        /// <typeparam name="T">
        /// The Type representing a row in the result set
        /// </typeparam>
        /// <param name="page">
        /// The 1 based page number to retrieve
        /// </param>
        /// <param name="itemsPerPage">
        /// The number of records per page
        /// </param>
        /// <param name="sql">
        /// The base SQL query
        /// </param>
        /// <param name="args">
        /// Arguments to any embedded parameters in the SQL statement
        /// </param>
        /// <returns>
        /// A Page of results
        /// </returns>
        /// <remarks>
        /// PetaPoco will automatically modify the supplied SELECT statement to only retrieve the
        /// records for the specified page.  It will also execute a second query to retrieve the
        /// total number of records in the result set.
        /// </remarks>
        Page<T> FetchPage<T>(long page, long itemsPerPage, string sql, params object[] args);

        /// <summary>
        /// Runs a query that should always return at least one return
        /// </summary>
        /// <typeparam name="T">
        /// The Type representing a row in the result set
        /// </typeparam>
        /// <param name="sql">
        /// The SQL query
        /// </param>
        /// <param name="args">
        /// Arguments to any embedded parameters in the SQL statement
        /// </param>
        /// <returns>
        /// The first record in the result set
        /// </returns>
        T First<T>(string sql, params object[] args);

        /// <summary>
        /// Runs a query and returns the first record, or the default value if no matching records
        /// </summary>
        /// <typeparam name="T">
        /// The Type representing a row in the result set
        /// </typeparam>
        /// <param name="sql">
        /// The SQL query
        /// </param>
        /// <param name="args">
        /// Arguments to any embedded parameters in the SQL statement
        /// </param>
        /// <returns>
        /// The first record in the result set, or default(T) if no matching rows
        /// </returns>
        T FirstOrDefault<T>(string sql, params object[] args);

        /// <summary>
        /// Runs a query that should always return a single row.
        /// </summary>
        /// <typeparam name="T">
        /// The Type representing a row in the result set
        /// </typeparam>
        /// <param name="sql">
        /// The SQL query
        /// </param>
        /// <param name="args">
        /// Arguments to any embedded parameters in the SQL statement
        /// </param>
        /// <returns>
        /// The single record matching the specified primary key value
        /// </returns>
        /// <remarks>
        /// Throws an exception if there are zero or more than one matching record
        /// </remarks>
        T Single<T>(string sql, params object[] args);

        /// <summary>
        /// Runs a query that should always return either a single row, or no rows
        /// </summary>
        /// <typeparam name="T">
        /// The Type representing a row in the result set
        /// </typeparam>
        /// <param name="sql">
        /// The SQL query
        /// </param>
        /// <param name="args">
        /// Arguments to any embedded parameters in the SQL statement
        /// </param>
        /// <returns>
        /// The single record matching the specified primary key value, or default(T) if no matching rows
        /// </returns>
        T SingleOrDefault<T>(string sql, params object[] args);

        /// <summary>
        /// Retrieves a range of records from result set
        /// </summary>
        /// <typeparam name="T">
        /// The Type representing a row in the result set
        /// </typeparam>
        /// <param name="skip">
        /// The number of rows at the start of the result set to skip over
        /// </param>
        /// <param name="take">
        /// The number of rows to retrieve
        /// </param>
        /// <param name="sql">
        /// The base SQL query
        /// </param>
        /// <param name="args">
        /// Arguments to any embedded parameters in the SQL statement
        /// </param>
        /// <returns>
        /// A List of results
        /// </returns>
        /// <remarks>
        /// PetaPoco will automatically modify the supplied SELECT statement to only retrieve the
        /// records for the specified range.
        /// </remarks>
        IEnumerable<T> SkipTake<T>(long skip, long take, string sql, params object[] args);

/*
        /// <summary>
        /// Performs an SQL Insert
        /// </summary>
        /// <typeparam name="T">
        /// The type of item being inserted.
        /// </typeparam>
        /// <param name="tableName">
        /// The name of the table to insert into
        /// </param>
        /// <param name="primaryKeyName">
        /// The name of the primary key column of the table
        /// </param>
        /// <param name="poco">
        /// The POCO object that specifies the column values to be inserted
        /// </param>
        /// <returns>
        /// The auto allocated primary key of the new record
        /// </returns>
        object Insert<T>(string tableName, string primaryKeyName, T poco);
*/

        /// <summary>
        /// Performs an SQL Insert
        /// </summary>
        /// <typeparam name="T">
        /// The type of item being inserted.
        /// </typeparam>
        /// <param name="poco">
        /// The POCO object that specifies the column values to be inserted
        /// </param>
        /// <returns>
        /// The auto allocated primary key of the new record, or null for non-auto-increment tables
        /// </returns>
        /// <remarks>
        /// The name of the table, it's primary key and whether it's an auto-allocated primary key are retrieved
        /// from the POCO's attributes
        /// </remarks>
        object Insert<T>(T poco);
    }
}