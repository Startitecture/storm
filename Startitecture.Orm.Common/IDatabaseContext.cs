// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDatabaseContext.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface for classes that contain database contexts.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Common
{
    using System;
    using System.Collections.Generic;
    using System.Data;

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
        /// Gets or sets the timeout value for the next (and only next) SQL statement.
        /// </summary>
        int OnetimeCommandTimeout { get; set; }

        /// <summary>
        /// Gets the statement compiler for the database context.
        /// </summary>
        IRepositoryAdapter RepositoryAdapter { get; }

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
        /// Gets a value mapper for the specified types.
        /// </summary>
        /// <param name="sourceType">
        /// The source type.
        /// </param>
        /// <param name="destinationType">
        /// The destination type.
        /// </param>
        /// <returns>
        /// An <see cref="IValueMapper"/> for the specified conversion, or <c>null</c> if no mapper has been registered.
        /// </returns>
        IValueMapper GetValueMapper(Type sourceType, Type destinationType);
    }
}